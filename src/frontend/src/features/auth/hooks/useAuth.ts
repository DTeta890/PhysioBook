import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { User } from '@/features/auth/types'
import { setAccessToken } from '@/lib/api-client'

interface AuthState {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  tenantId: string | null
  isAuthenticated: boolean
  login: (tokens: { accessToken: string; refreshToken: string }, user: User) => void
  logout: () => void
  setUser: (user: User) => void
  setTokens: (accessToken: string, refreshToken: string) => void
  setTenantId: (tenantId: string) => void
}

export const useAuth = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      tenantId: null,
      isAuthenticated: false,

      login: (tokens, user) => {
        setAccessToken(tokens.accessToken)
        set({
          user,
          accessToken: tokens.accessToken,
          refreshToken: tokens.refreshToken,
          tenantId: user.tenantId,
          isAuthenticated: true,
        })
      },

      logout: () => {
        setAccessToken(null)
        set({
          user: null,
          accessToken: null,
          refreshToken: null,
          isAuthenticated: false,
        })
      },

      setUser: (user) => set({ user }),

      setTokens: (accessToken, refreshToken) => {
        setAccessToken(accessToken)
        set({ accessToken, refreshToken })
      },

      setTenantId: (tenantId) => set({ tenantId }),
    }),
    {
      name: 'physiobook-auth',
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        tenantId: state.tenantId,
        isAuthenticated: state.isAuthenticated,
      }),
    },
  ),
)

// Rehydrate access token on app start
const state = useAuth.getState()
if (state.accessToken) {
  setAccessToken(state.accessToken)
}
