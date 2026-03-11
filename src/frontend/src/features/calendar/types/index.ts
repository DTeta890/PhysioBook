export interface Appointment {
  id: string
  patientName: string
  therapistId: string
  treatmentType: string
  startTime: string // ISO 8601 datetime string
  endTime: string // ISO 8601 datetime string
  durationMinutes: number
  status: AppointmentStatus
  color: string
  notes?: string
}

export type AppointmentStatus = 'scheduled' | 'confirmed' | 'in-progress' | 'completed' | 'cancelled' | 'no-show'

export interface CalendarConfig {
  startHour: number
  endHour: number
  slotDurationMinutes: number
  pixelsPerSlot: number
}

export type CalendarView = 'day' | 'week' | 'month'

export interface Therapist {
  id: string
  firstName: string
  lastName: string
  color: string
  specialization: string | null
}

export interface TimeSlotData {
  therapistId: string
  timeSlot: string // e.g. "09:00"
  dayDate: string // ISO date string e.g. "2026-03-11"
}

export interface DraggableAppointmentData {
  appointmentId: string
  therapistId: string
  startTime: string
  endTime: string
  durationMinutes: number
}

export interface MoveAppointmentParams {
  appointmentId: string
  newTherapistId: string
  newStartTime: string
  newEndTime: string
}

export interface ResizeAppointmentParams {
  appointmentId: string
  newEndTime: string
  newDurationMinutes: number
}
