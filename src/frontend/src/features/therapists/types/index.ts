export interface Therapist {
  id: string
  email: string
  firstName: string
  lastName: string
  phone: string | null
  specialization: string | null
  color: string | null
  avatarUrl: string | null
  isActive: boolean
  createdAt: string
}

export interface CreateTherapistRequest {
  email: string
  firstName: string
  lastName: string
  password: string
  phone?: string
  specialization?: string
  color?: string
}

export interface UpdateTherapistRequest {
  firstName: string
  lastName: string
  phone?: string
  specialization?: string
  color?: string
  isActive: boolean
}
