import { apiClient } from '@/lib/api-client'
import type { AuthResponse, ChangePasswordRequest, User } from '@/features/auth/types'

export const authApi = {
  login: (email: string, password: string, tenantId: string) =>
    apiClient.post<AuthResponse>('/auth/login', { email, password }, {
      headers: { 'X-Tenant-Id': tenantId },
    }),

  refresh: (refreshToken: string) =>
    apiClient.post<AuthResponse>('/auth/refresh', { refreshToken }),

  logout: (refreshToken: string) =>
    apiClient.post<void>('/auth/logout', { refreshToken }),

  getMe: () =>
    apiClient.get<User>('/auth/me'),

  changePassword: (data: ChangePasswordRequest) =>
    apiClient.put<void>('/auth/change-password', data),
}
