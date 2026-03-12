import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { patientApi } from '../api/patient-api'
import type { CreatePatientRequest, UpdatePatientRequest } from '../types'

const PATIENTS_KEY = ['patients'] as const

export function usePatients(search?: string, isActive?: boolean, page?: number, pageSize?: number) {
  return useQuery({
    queryKey: [...PATIENTS_KEY, { search, isActive, page, pageSize }],
    queryFn: async () => {
      const response = await patientApi.getAll({ search, isActive, page, pageSize })
      return response.data
    },
  })
}

export function usePatient(id: string) {
  return useQuery({
    queryKey: [...PATIENTS_KEY, id],
    queryFn: async () => {
      const response = await patientApi.getById(id)
      return response.data
    },
    enabled: !!id,
  })
}

export function useCreatePatient() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreatePatientRequest) => patientApi.create(data),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: PATIENTS_KEY })
    },
  })
}

export function useUpdatePatient() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdatePatientRequest }) =>
      patientApi.update(id, data),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: PATIENTS_KEY })
    },
  })
}

export function useDeletePatient() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => patientApi.delete(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: PATIENTS_KEY })
    },
  })
}
