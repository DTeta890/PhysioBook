export interface User {
  id: string
  email: string
  name: string
  role: 'SuperAdmin' | 'ClinicAdmin' | 'Therapist' | 'Receptionist'
  tenantId: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: User
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
}
