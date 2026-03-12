import { apiClient } from '@/lib/api-client'
import type { Patient, PagedResult, CreatePatientRequest, UpdatePatientRequest, PatientListParams } from '../types'

export const patientApi = {
  getAll: (params?: PatientListParams) => {
    const searchParams = new URLSearchParams()
    if (params?.search) searchParams.set('search', params.search)
    if (params?.isActive !== undefined) searchParams.set('isActive', String(params.isActive))
    if (params?.page) searchParams.set('page', String(params.page))
    if (params?.pageSize) searchParams.set('pageSize', String(params.pageSize))
    const query = searchParams.toString()
    return apiClient.get<PagedResult<Patient>>(`/patients${query ? `?${query}` : ''}`)
  },
  getById: (id: string) => apiClient.get<Patient>(`/patients/${id}`),
  create: (data: CreatePatientRequest) => apiClient.post<Patient>('/patients', data),
  update: (id: string, data: UpdatePatientRequest) => apiClient.put<Patient>(`/patients/${id}`, data),
  delete: (id: string) => apiClient.delete<void>(`/patients/${id}`),
}
