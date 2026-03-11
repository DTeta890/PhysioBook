export interface Appointment {
  id: string
  therapistId: string
  patientId: string | null
  patientName: string | null
  patientPhone: string | null
  treatmentTypeId: string
  treatmentTypeName: string
  startTime: string // ISO datetime
  endTime: string
  status:
    | 'scheduled'
    | 'confirmed'
    | 'in_progress'
    | 'completed'
    | 'cancelled'
    | 'no_show'
  notes: string | null
  isWalkIn: boolean
  color: string | null
}

export interface CalendarConfig {
  startHour: number // e.g., 8
  endHour: number // e.g., 20
  slotDurationMinutes: number // e.g., 15
}

export type CalendarView = 'week' | 'day' | 'month'
