import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { walkinApi } from '../api/walkin-api'
import type { CheckInRequest, ConvertRequest } from '../types'

const WALKINS_KEY = ['walkins'] as const

export function useWalkInQueue(status?: string, date?: string) {
  return useQuery({
    queryKey: [...WALKINS_KEY, { status, date }],
    queryFn: async () => {
      const response = await walkinApi.getQueue(status, date)
      return response.data
    },
    refetchInterval: 30000,
  })
}

export function useWalkInStats(from: string, to: string) {
  return useQuery({
    queryKey: [...WALKINS_KEY, 'stats', { from, to }],
    queryFn: async () => {
      const response = await walkinApi.getStats(from, to)
      return response.data
    },
    enabled: !!from && !!to,
  })
}

export function useCheckIn() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CheckInRequest) => {
      const response = await walkinApi.checkIn(data)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: WALKINS_KEY })
    },
  })
}

export function useCallWalkIn() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, therapistId }: { id: string; therapistId: string }) => {
      const response = await walkinApi.call(id, therapistId)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: WALKINS_KEY })
    },
  })
}

export function useCompleteWalkIn() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (id: string) => {
      const response = await walkinApi.complete(id)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: WALKINS_KEY })
    },
  })
}

export function useCancelWalkIn() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, reason }: { id: string; reason?: string }) => {
      const response = await walkinApi.cancel(id, reason)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: WALKINS_KEY })
    },
  })
}

export function useConvertToAppointment() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: ConvertRequest }) => {
      const response = await walkinApi.convertToAppointment(id, data)
      return response.data
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: WALKINS_KEY })
    },
  })
}
