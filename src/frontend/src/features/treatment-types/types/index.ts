export interface TreatmentType {
  id: string
  name: string
  description: string | null
  durationMinutes: number
  price: number
  color: string | null
  isActive: boolean
  createdAt: string
}

export interface CreateTreatmentTypeRequest {
  name: string
  description?: string
  durationMinutes: number
  price: number
  color?: string
}

export interface UpdateTreatmentTypeRequest {
  name: string
  description?: string
  durationMinutes: number
  price: number
  color?: string
  isActive: boolean
}
