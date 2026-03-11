export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  role: 'owner' | 'admin' | 'therapist' | 'receptionist'
  isTherapist: boolean
  specialization: string | null
  phone: string | null
  avatarUrl: string | null
  tenantId: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAt: string
  user: User
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}
