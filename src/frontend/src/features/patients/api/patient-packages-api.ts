import { apiClient } from '@/lib/api-client'
import type { PatientPackage } from '../types'

export const patientPackagesApi = {
  getByPatient: (patientId: string, status?: string) => {
    const params = new URLSearchParams()
    if (status) params.set('status', status)
    const query = params.toString()
    return apiClient.get<PatientPackage[]>(
      `/patientpackages/by-patient/${patientId}${query ? `?${query}` : ''}`,
    )
  },
}
