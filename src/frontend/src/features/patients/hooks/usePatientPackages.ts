import { useQuery } from '@tanstack/react-query'
import { patientPackagesApi } from '../api/patient-packages-api'

export function usePatientPackages(patientId: string, status?: string) {
  return useQuery({
    queryKey: ['patient-packages', patientId, status],
    queryFn: async () => {
      const response = await patientPackagesApi.getByPatient(patientId, status)
      return response.data
    },
    enabled: !!patientId,
  })
}
