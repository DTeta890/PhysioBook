const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api/v1'

interface ApiResponse<T> {
  data: T
  errors: string[]
  meta: Record<string, unknown>
}

interface RequestOptions extends Omit<RequestInit, 'body'> {
  body?: unknown
}

let accessToken: string | null = null

export function setAccessToken(token: string | null) {
  accessToken = token
}

export function getAccessToken() {
  return accessToken
}

async function refreshToken(): Promise<string | null> {
  // TODO: implement refresh token rotation
  // POST /api/v1/auth/refresh with the stored refresh token
  return null
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
    const newToken = await refreshToken()
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
    throw new Error(error.errors?.[0] ?? 'An unknown error occurred')
  }

  return response.json() as Promise<ApiResponse<T>>
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
