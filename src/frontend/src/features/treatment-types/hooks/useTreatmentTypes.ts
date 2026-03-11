import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { treatmentTypeApi } from '../api/treatment-type-api'
import type { CreateTreatmentTypeRequest, UpdateTreatmentTypeRequest } from '../types'

const QUERY_KEY = ['treatment-types'] as const

export function useTreatmentTypes() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: async () => {
      const response = await treatmentTypeApi.getAll()
      return response.data
    },
  })
}

export function useTreatmentType(id: string) {
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: async () => {
      const response = await treatmentTypeApi.getById(id)
      return response.data
    },
    enabled: !!id,
  })
}

export function useCreateTreatmentType() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateTreatmentTypeRequest) => {
      const response = await treatmentTypeApi.create(data)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useUpdateTreatmentType() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: UpdateTreatmentTypeRequest }) => {
      const response = await treatmentTypeApi.update(id, data)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useDeleteTreatmentType() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (id: string) => {
      await treatmentTypeApi.delete(id)
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}
