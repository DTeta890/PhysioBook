const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api/v1'

export interface ApiResponse<T> {
  data: T
  errors: string[]
  meta: Record<string, unknown>
}

interface RequestOptions extends Omit<RequestInit, 'body'> {
  body?: unknown
}

let accessToken: string | null = null
let refreshPromise: Promise<string | null> | null = null

export function setAccessToken(token: string | null) {
  accessToken = token
}

export function getAccessToken() {
  return accessToken
}

async function tryRefreshToken(): Promise<string | null> {
  // Avoid concurrent refresh calls
  if (refreshPromise) return refreshPromise

  refreshPromise = (async () => {
    try {
      // Import dynamically to avoid circular dependency
      const { useAuth } = await import('@/features/auth/hooks/useAuth')
      const state = useAuth.getState()
      if (!state.refreshToken) return null

      const response = await fetch(`${BASE_URL}/auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: state.refreshToken }),
      })

      if (!response.ok) {
        state.logout()
        return null
      }

      const result = (await response.json()) as ApiResponse<{
        accessToken: string
        refreshToken: string
        user: unknown
      }>

      state.setTokens(result.data.accessToken, result.data.refreshToken)
      return result.data.accessToken
    } catch {
      return null
    } finally {
      refreshPromise = null
    }
  })()

  return refreshPromise
}

async function request<T>(
  endpoint: string,
  options: RequestOptions = {},
): Promise<ApiResponse<T>> {
  const { body, headers: customHeaders, ...rest } = options

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...Object.fromEntries(
      Object.entries(customHeaders ?? {}).filter(
        (entry): entry is [string, string] => typeof entry[1] === 'string',
      ),
    ),
  }

  if (accessToken) {
    headers['Authorization'] = `Bearer ${accessToken}`
  }

  let response = await fetch(`${BASE_URL}${endpoint}`, {
    ...rest,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  })

  // Auto-refresh on 401
  if (response.status === 401 && accessToken) {
    const newToken = await tryRefreshToken()
    if (newToken) {
      accessToken = newToken
      headers['Authorization'] = `Bearer ${newToken}`
      response = await fetch(`${BASE_URL}${endpoint}`, {
        ...rest,
        headers,
        body: body ? JSON.stringify(body) : undefined,
      })
    }
  }

  if (!response.ok) {
    const error = await response.json().catch(() => ({
      errors: [`HTTP ${response.status}: ${response.statusText}`],
    }))
    throw new ApiError(
      error.errors?.[0] ?? 'An unknown error occurred',
      response.status,
      error.errors ?? [],
    )
  }

  return response.json() as Promise<ApiResponse<T>>
}

export class ApiError extends Error {
  status: number
  errors: string[]

  constructor(message: string, status: number, errors: string[]) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.errors = errors
  }
}

export const apiClient = {
  get: <T>(endpoint: string, options?: RequestOptions) =>
    request<T>(endpoint, { ...options, method: 'GET' }),

  post: <T>(endpoint: string, body?: unknown, options?: RequestOptions) =>
    request<T>(endpoint, { ...options, method: 'POST', body }),

  put: <T>(endpoint: string, body?: unknown, options?: RequestOptions) =>
    request<T>(endpoint, { ...options, method: 'PUT', body }),

  patch: <T>(endpoint: string, body?: unknown, options?: RequestOptions) =>
    request<T>(endpoint, { ...options, method: 'PATCH', body }),

  delete: <T>(endpoint: string, options?: RequestOptions) =>
    request<T>(endpoint, { ...options, method: 'DELETE' }),
}
