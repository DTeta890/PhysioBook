import { startOfWeek, addDays, format, parseISO } from 'date-fns'
import type { CalendarConfig } from '../types'

/**
 * Get array of dates for the current week (Mon-Sat, 6 days for clinic).
 */
export function getWeekDays(date: Date): Date[] {
  const monday = startOfWeek(date, { weekStartsOn: 1 })
  return Array.from({ length: 6 }, (_, i) => addDays(monday, i))
}

/**
 * Get time slots for a day based on config.
 * Returns array like ["08:00", "08:15", "08:30", ...]
 */
export function getTimeSlots(config: CalendarConfig): string[] {
  const slots: string[] = []
  const totalMinutes =
    (config.endHour - config.startHour) * 60
  const slotCount = totalMinutes / config.slotDurationMinutes

  for (let i = 0; i <= slotCount; i++) {
    const minutesFromStart = i * config.slotDurationMinutes
    const hour = config.startHour + Math.floor(minutesFromStart / 60)
    const minute = minutesFromStart % 60
    slots.push(
      `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`,
    )
  }

  return slots
}

/**
 * Calculate position of an appointment in the grid.
 * Returns top offset in px and height in px.
 * Each slot is 20px tall.
 */
export function getAppointmentPosition(
  startTime: string,
  endTime: string,
  config: CalendarConfig,
): { top: number; height: number } {
  const start = parseISO(startTime)
  const end = parseISO(endTime)

  const startMinutes = start.getHours() * 60 + start.getMinutes()
  const endMinutes = end.getHours() * 60 + end.getMinutes()

  const configStartMinutes = config.startHour * 60
  const pixelsPerSlot = 20
  const pixelsPerMinute = pixelsPerSlot / config.slotDurationMinutes

  const top = (startMinutes - configStartMinutes) * pixelsPerMinute
  const height = (endMinutes - startMinutes) * pixelsPerMinute

  return { top, height: Math.max(height, pixelsPerSlot) }
}

/**
 * Format time for display, e.g. "09:30"
 */
export function formatTime(date: Date | string): string {
  const d = typeof date === 'string' ? parseISO(date) : date
  return format(d, 'HH:mm')
}

/**
 * Check if two time ranges overlap.
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
