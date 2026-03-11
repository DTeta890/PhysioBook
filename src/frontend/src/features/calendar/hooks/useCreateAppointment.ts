import { useMutation, useQueryClient } from '@tanstack/react-query'
import type { Appointment } from '../types'

export interface CreateAppointmentParams {
  therapistId: string
  patientName: string
  patientPhone?: string
  treatmentTypeId: string
  startTime: string
  endTime: string
  notes?: string
  color?: string
}

/**
 * Create a new appointment.
 * Currently uses mock implementation; will switch to apiClient when backend is connected.
 */
export function useCreateAppointment() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (params: CreateAppointmentParams): Promise<Appointment> => {
      // TODO: Replace with apiClient.post<Appointment>('/appointments', params)
      await new Promise((resolve) => setTimeout(resolve, 150))

      const startDate = new Date(params.startTime)
      const endDate = new Date(params.endTime)
      const durationMinutes = Math.round((endDate.getTime() - startDate.getTime()) / 60000)

      const newAppointment: Appointment = {
        id: `apt-${Date.now()}`,
        patientName: params.patientName,
        therapistId: params.therapistId,
        treatmentType: params.treatmentTypeId,
        startTime: params.startTime,
        endTime: params.endTime,
        durationMinutes,
        status: 'scheduled',
        color: params.color ?? '#22c55e',
        notes: params.notes,
      }

      return newAppointment
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['appointments'] })
    },
  })
}
