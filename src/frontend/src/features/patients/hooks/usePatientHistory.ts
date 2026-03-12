import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { treatmentNotesApi } from '../api/treatment-notes-api'
import { patientPackagesApi } from '../api/patient-packages-api'
import type { AppointmentSummary, TreatmentNote, PatientPackage, TimelineEvent } from '../types'

export function usePatientAppointments(patientId: string) {
  return useQuery({
    queryKey: ['patient-appointments', patientId],
    queryFn: async () => {
      const response = await apiClient.get<AppointmentSummary[]>(
        `/appointments?patientId=${patientId}`,
      )
      return response.data
    },
    enabled: !!patientId,
  })
}

export function usePatientTreatmentNotes(patientId: string, page = 1, pageSize = 50) {
  return useQuery({
    queryKey: ['patient-treatment-notes', patientId, page, pageSize],
    queryFn: async () => {
      const response = await treatmentNotesApi.getByPatient(patientId, page, pageSize)
      return response.data
    },
    enabled: !!patientId,
  })
}

export function usePatientTimeline(patientId: string) {
  const appointmentsQuery = usePatientAppointments(patientId)
  const notesQuery = usePatientTreatmentNotes(patientId)
  const packagesQuery = useQuery({
    queryKey: ['patient-packages-timeline', patientId],
    queryFn: async () => {
      const response = await patientPackagesApi.getByPatient(patientId)
      return response.data
    },
    enabled: !!patientId,
  })

  const isLoading = appointmentsQuery.isLoading || notesQuery.isLoading || packagesQuery.isLoading
  const isError = appointmentsQuery.isError || notesQuery.isError || packagesQuery.isError

  const events: TimelineEvent[] = []

  if (appointmentsQuery.data) {
    for (const appointment of appointmentsQuery.data) {
      events.push({
        id: appointment.id,
        type: 'appointment',
        date: appointment.startTime,
        data: appointment,
      })
    }
  }

  if (notesQuery.data) {
    const notes = notesQuery.data.items ?? (notesQuery.data as unknown as TreatmentNote[])
    for (const note of notes) {
      events.push({
        id: note.id,
        type: 'treatment_note',
        date: note.appointmentDate,
        data: note,
      })
    }
  }

  if (packagesQuery.data) {
    const packages = packagesQuery.data as PatientPackage[]
    for (const pkg of packages) {
      events.push({
        id: pkg.id,
        type: 'package_purchase',
        date: pkg.purchasedAt,
        data: pkg,
      })
    }
  }

  // Sort by date descending
  events.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())

  return {
    data: events,
    isLoading,
    isError,
  }
}
