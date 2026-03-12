export interface Patient {
  id: string
  firstName: string
  lastName: string
  fullName: string
  email: string | null
  phone: string | null
  dateOfBirth: string | null
  gender: string | null
  address: string | null
  city: string | null
  emergencyContactName: string | null
  emergencyContactPhone: string | null
  medicalHistory: string | null
  allergies: string | null
  notes: string | null
  isActive: boolean
  createdAt: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface CreatePatientRequest {
  firstName: string
  lastName: string
  email?: string
  phone?: string
  dateOfBirth?: string
  gender?: string
  address?: string
  city?: string
  emergencyContactName?: string
  emergencyContactPhone?: string
  medicalHistory?: string
  allergies?: string
  notes?: string
}

export interface UpdatePatientRequest extends CreatePatientRequest {
  isActive: boolean
}

export interface PatientListParams {
  search?: string
  isActive?: boolean
  page?: number
  pageSize?: number
}
