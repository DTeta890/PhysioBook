import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { therapistApi } from '../api/therapist-api'
import type { CreateTherapistRequest, UpdateTherapistRequest } from '../types'

const THERAPISTS_KEY = ['therapists'] as const

export function useTherapists() {
  return useQuery({
    queryKey: THERAPISTS_KEY,
    queryFn: async () => {
      const response = await therapistApi.getAll()
      return response.data
    },
  })
}

export function useTherapist(id: string) {
  return useQuery({
    queryKey: [...THERAPISTS_KEY, id],
    queryFn: async () => {
      const response = await therapistApi.getById(id)
      return response.data
    },
    enabled: !!id,
  })
}

export function useCreateTherapist() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateTherapistRequest) => therapistApi.create(data),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: THERAPISTS_KEY })
    },
  })
}

export function useUpdateTherapist() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateTherapistRequest }) =>
      therapistApi.update(id, data),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: THERAPISTS_KEY })
    },
  })
}

export function useDeleteTherapist() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => therapistApi.delete(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: THERAPISTS_KEY })
    },
  })
}
