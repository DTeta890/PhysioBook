import { apiClient } from '@/lib/api-client'
import type { TreatmentType, CreateTreatmentTypeRequest, UpdateTreatmentTypeRequest } from '../types'

export const treatmentTypeApi = {
  getAll: () => apiClient.get<TreatmentType[]>('/treatmenttypes'),
  getById: (id: string) => apiClient.get<TreatmentType>(`/treatmenttypes/${id}`),
  create: (data: CreateTreatmentTypeRequest) =>
    apiClient.post<TreatmentType>('/treatmenttypes', data),
  update: (id: string, data: UpdateTreatmentTypeRequest) =>
    apiClient.put<TreatmentType>(`/treatmenttypes/${id}`, data),
  delete: (id: string) => apiClient.delete<void>(`/treatmenttypes/${id}`),
}
