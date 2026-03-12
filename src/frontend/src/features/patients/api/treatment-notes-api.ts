import { apiClient } from '@/lib/api-client'
import type { TreatmentNote, PagedResult } from '../types'

export const treatmentNotesApi = {
  getByPatient: (patientId: string, page = 1, pageSize = 20) =>
    apiClient.get<PagedResult<TreatmentNote>>(
      `/treatmentnotes/by-patient/${patientId}?page=${page}&pageSize=${pageSize}`,
    ),
}
