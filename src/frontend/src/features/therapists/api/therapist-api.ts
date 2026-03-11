import { apiClient } from '@/lib/api-client'
import type { Therapist, CreateTherapistRequest, UpdateTherapistRequest } from '../types'

export const therapistApi = {
  getAll: () => apiClient.get<Therapist[]>('/therapists'),
  getById: (id: string) => apiClient.get<Therapist>(`/therapists/${id}`),
  create: (data: CreateTherapistRequest) => apiClient.post<Therapist>('/therapists', data),
  update: (id: string, data: UpdateTherapistRequest) => apiClient.put<Therapist>(`/therapists/${id}`, data),
  delete: (id: string) => apiClient.delete<void>(`/therapists/${id}`),
}
