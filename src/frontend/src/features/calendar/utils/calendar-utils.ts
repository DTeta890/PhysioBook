import {
  format,
  addDays,
  startOfWeek,
  endOfWeek,
  startOfMonth,
  endOfMonth,
  eachDayOfInterval,
  parseISO,
  addMinutes,
} from 'date-fns'
import type { Appointment, CalendarConfig } from '../types'

export const DEFAULT_CONFIG: CalendarConfig = {
  startHour: 8,
  endHour: 20,
  slotDurationMinutes: 15,
  pixelsPerSlot: 20,
}

/**
 * Get all days of the week for a given date (Monday-based week)
 */
export function getWeekDays(date: Date): Date[] {
  const weekStart = startOfWeek(date, { weekStartsOn: 1 })
  return Array.from({ length: 7 }, (_, i) => addDays(weekStart, i))
}

/**
 * Get time slots for a day based on config
 * Returns array of time strings like ["08:00", "08:15", "08:30", ...]
 */
export function getTimeSlots(config: CalendarConfig = DEFAULT_CONFIG): string[] {
  const slots: string[] = []
  const totalMinutes = (config.endHour - config.startHour) * 60
  const slotCount = totalMinutes / config.slotDurationMinutes

  for (let i = 0; i < slotCount; i++) {
    const minutesFromStart = i * config.slotDurationMinutes
    const hour = config.startHour + Math.floor(minutesFromStart / 60)
    const minute = minutesFromStart % 60
    slots.push(`${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`)
  }

  return slots
}

/**
 * Get the top position and height of an appointment block in pixels
 */
export function getAppointmentPosition(
  appointment: Appointment,
  config: CalendarConfig = DEFAULT_CONFIG,
): { top: number; height: number } {
  const start = parseISO(appointment.startTime)
  const end = parseISO(appointment.endTime)

  const startMinutesFromDayStart = start.getHours() * 60 + start.getMinutes()
  const endMinutesFromDayStart = end.getHours() * 60 + end.getMinutes()
  const configStartMinutes = config.startHour * 60

  const startOffset = startMinutesFromDayStart - configStartMinutes
  const duration = endMinutesFromDayStart - startMinutesFromDayStart

  const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
  const top = startOffset * pixelsPerMinute
  const height = duration * pixelsPerMinute

  return { top, height }
}

/**
 * Format a time string for display (e.g., "09:00" or "9:00 AM")
 */
export function formatTime(timeStr: string): string {
  return timeStr
}

/**
 * Format hour for the time gutter (e.g., 9 -> "09:00")
 */
export function formatHour(hour: number): string {
  return `${String(hour).padStart(2, '0')}:00`
}

/**
 * Check if two time ranges overlap
 */
export function hasOverlap(
  start1: string,
  end1: string,
  start2: string,
  end2: string,
): boolean {
  const s1 = parseISO(start1).getTime()
  const e1 = parseISO(end1).getTime()
  const s2 = parseISO(start2).getTime()
  const e2 = parseISO(end2).getTime()

  return s1 < e2 && s2 < e1
}

/**
 * Given a time slot string (e.g., "09:00") and a date string (e.g., "2026-03-11"),
 * returns a full ISO datetime string
 */
export function buildDateTimeFromSlot(dayDate: string, timeSlot: string): string {
  return `${dayDate}T${timeSlot}:00`
}

/**
 * Calculate the new end time given a start time and duration in minutes
 */
export function calculateEndTime(startTime: string, durationMinutes: number): string {
  const start = parseISO(startTime)
  const end = addMinutes(start, durationMinutes)
  return format(end, "yyyy-MM-dd'T'HH:mm:ss")
}

/**
 * Get the total height of the calendar grid in pixels
 */
export function getGridHeight(config: CalendarConfig = DEFAULT_CONFIG): number {
  const totalSlots = ((config.endHour - config.startHour) * 60) / config.slotDurationMinutes
  return totalSlots * config.pixelsPerSlot
}

/**
 * Snap a pixel offset to the nearest slot boundary
 */
export function snapToSlot(pixelOffset: number, config: CalendarConfig = DEFAULT_CONFIG): number {
  return Math.round(pixelOffset / config.pixelsPerSlot) * config.pixelsPerSlot
}

/**
 * Convert a pixel offset from the top of the grid to a time string
 */
export function pixelOffsetToTime(
  pixelOffset: number,
  config: CalendarConfig = DEFAULT_CONFIG,
): string {
  const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
  const minutesFromStart = Math.round(pixelOffset / pixelsPerMinute)
  const totalMinutes = config.startHour * 60 + minutesFromStart
  const hour = Math.floor(totalMinutes / 60)
  const minute = totalMinutes % 60
  return `${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`
}

/**
 * Check if an appointment would conflict with existing appointments for a therapist.
 * Excludes the appointment being moved (by id).
 */
export function checkConflict(
  appointments: Appointment[],
  therapistId: string,
  startTime: string,
  endTime: string,
  excludeAppointmentId?: string,
): boolean {
  return appointments
    .filter(
      (apt) =>
        apt.therapistId === therapistId &&
        apt.id !== excludeAppointmentId &&
        apt.status !== 'cancelled',
    )
    .some((apt) => hasOverlap(startTime, endTime, apt.startTime, apt.endTime))
}

/**
 * Returns all days to display in a month grid, including padding days
 * from the previous and next months to fill complete weeks (Monday-start).
 */
export function getMonthDays(date: Date): Date[] {
  const monthStart = startOfMonth(date)
  const monthEnd = endOfMonth(date)
  const calendarStart = startOfWeek(monthStart, { weekStartsOn: 1 })
  const calendarEnd = endOfWeek(monthEnd, { weekStartsOn: 1 })
  return eachDayOfInterval({ start: calendarStart, end: calendarEnd })
}

/**
 * Filter appointments that fall on a specific day.
 */
export function getAppointmentsForDay(appointments: Appointment[], date: Date): Appointment[] {
  const dayStr = format(date, 'yyyy-MM-dd')
  return appointments.filter((apt) => apt.startTime.startsWith(dayStr))
}
