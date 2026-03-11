import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import type { Appointment, MoveAppointmentParams, ResizeAppointmentParams } from '../types'
import { mockAppointments } from '../utils/mock-data'

// In-memory store for mock data mutations (will be replaced by API calls)
const localAppointments = [...mockAppointments]

function getLocalAppointments(): Appointment[] {
  return [...localAppointments]
}

function moveLocalAppointment(params: MoveAppointmentParams): Appointment {
  const index = localAppointments.findIndex((a) => a.id === params.appointmentId)
  if (index === -1) throw new Error('Appointment not found')

  const updated: Appointment = {
    ...localAppointments[index],
    therapistId: params.newTherapistId,
    startTime: params.newStartTime,
    endTime: params.newEndTime,
  }
  localAppointments[index] = updated
  return updated
}

function resizeLocalAppointment(params: ResizeAppointmentParams): Appointment {
  const index = localAppointments.findIndex((a) => a.id === params.appointmentId)
  if (index === -1) throw new Error('Appointment not found')

  const updated: Appointment = {
    ...localAppointments[index],
    endTime: params.newEndTime,
    durationMinutes: params.newDurationMinutes,
  }
  localAppointments[index] = updated
  return updated
}

/**
 * Fetch appointments for a date range.
 * Currently uses mock data; will switch to apiClient when backend is connected.
 */
export function useAppointments(_startDate: string, _endDate: string) {
  return useQuery({
    queryKey: ['appointments', _startDate, _endDate],
    queryFn: async () => {
      // TODO: Replace with apiClient.get<Appointment[]>(`/appointments?start=${startDate}&end=${endDate}`)
      // Simulate network delay
      await new Promise((resolve) => setTimeout(resolve, 100))
      return getLocalAppointments()
    },
  })
}

/**
 * Move an appointment to a new time slot / therapist.
 * Includes optimistic update pattern.
 */
export function useMoveAppointment() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (params: MoveAppointmentParams) => {
      // TODO: Replace with apiClient.put<Appointment>(`/appointments/${params.appointmentId}/move`, params)
      await new Promise((resolve) => setTimeout(resolve, 50))
      return moveLocalAppointment(params)
    },
    onMutate: async (params) => {
      // Cancel outgoing queries
      await queryClient.cancelQueries({ queryKey: ['appointments'] })

      // Snapshot previous value
      const previousQueries = queryClient.getQueriesData<Appointment[]>({
        queryKey: ['appointments'],
      })

      // Optimistically update
      queryClient.setQueriesData<Appointment[]>(
        { queryKey: ['appointments'] },
        (old) =>
          old?.map((apt) =>
            apt.id === params.appointmentId
              ? {
                  ...apt,
                  therapistId: params.newTherapistId,
                  startTime: params.newStartTime,
                  endTime: params.newEndTime,
                }
              : apt,
          ),
      )

      return { previousQueries }
    },
    onError: (_err, _params, context) => {
      // Rollback on error
      if (context?.previousQueries) {
        for (const [queryKey, data] of context.previousQueries) {
          queryClient.setQueryData(queryKey, data)
        }
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] })
    },
  })
}

/**
 * Resize an appointment (change duration / end time).
 * Includes optimistic update pattern.
 */
export function useResizeAppointment() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (params: ResizeAppointmentParams) => {
      // TODO: Replace with apiClient.put<Appointment>(`/appointments/${params.appointmentId}/resize`, params)
      await new Promise((resolve) => setTimeout(resolve, 50))
      return resizeLocalAppointment(params)
    },
    onMutate: async (params) => {
      await queryClient.cancelQueries({ queryKey: ['appointments'] })

      const previousQueries = queryClient.getQueriesData<Appointment[]>({
        queryKey: ['appointments'],
      })

      queryClient.setQueriesData<Appointment[]>(
        { queryKey: ['appointments'] },
        (old) =>
          old?.map((apt) =>
            apt.id === params.appointmentId
              ? {
                  ...apt,
                  endTime: params.newEndTime,
                  durationMinutes: params.newDurationMinutes,
                }
              : apt,
          ),
      )

      return { previousQueries }
    },
    onError: (_err, _params, context) => {
      if (context?.previousQueries) {
        for (const [queryKey, data] of context.previousQueries) {
          queryClient.setQueryData(queryKey, data)
        }
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] })
    },
  })
}

