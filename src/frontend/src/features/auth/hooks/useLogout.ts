import { useCallback } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useAuth } from './useAuth'
import { authApi } from '@/features/auth/api/auth-api'

export function useLogout() {
  const navigate = useNavigate()
  const refreshToken = useAuth((s) => s.refreshToken)
  const logout = useAuth((s) => s.logout)

  return useCallback(async () => {
    if (refreshToken) {
      try {
        await authApi.logout(refreshToken)
      } catch {
        // Ignore errors on logout — clear local state regardless
      }
    }
    logout()
    void navigate({ to: '/login' })
  }, [refreshToken, logout, navigate])
}
