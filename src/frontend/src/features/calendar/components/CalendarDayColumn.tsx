import { isSameDay, parseISO } from 'date-fns'
import type { Appointment, CalendarConfig } from '../types'
import type { Therapist } from '@/features/therapists/types'
import { AppointmentBlock } from './AppointmentBlock'
import { getTimeSlots } from '../utils/calendar-utils'

interface CalendarDayColumnProps {
  therapist: Therapist
  day: Date
  appointments: Appointment[]
  config: CalendarConfig
}

export function CalendarDayColumn({
  therapist,
  day,
  appointments,
  config,
}: CalendarDayColumnProps) {
  const slots = getTimeSlots(config)
  const dayAppointments = appointments.filter(
    (a) =>
      a.therapistId === therapist.id &&
      isSameDay(parseISO(a.startTime), day),
  )

  const pixelsPerSlot = 20
  const totalHeight = slots.length * pixelsPerSlot

  return (
    <div className="relative min-w-[180px] flex-1 border-r border-gray-200 last:border-r-0">
      {/* Slot grid lines */}
      <div style={{ height: `${totalHeight}px` }}>
        {slots.map((slot, i) => (
          <div
            key={slot}
            className={
              i % (60 / config.slotDurationMinutes) === 0
                ? 'border-t border-gray-200'
                : 'border-t border-gray-100'
            }
            style={{ height: `${pixelsPerSlot}px` }}
          />
        ))}
      </div>
      {/* Appointment blocks */}
      {dayAppointments.map((appointment) => (
        <AppointmentBlock
          key={appointment.id}
          appointment={appointment}
          therapistColor={therapist.color}
          config={config}
        />
      ))}
    </div>
  )
}
