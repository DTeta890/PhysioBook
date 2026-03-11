import { format } from 'date-fns'
import { cn } from '@/shared/utils/cn'
import { DroppableTimeSlot } from './DroppableTimeSlot'
import { DraggableAppointment } from './DraggableAppointment'
import { CurrentTimeIndicator } from './CurrentTimeIndicator'
import {
  getTimeSlots,
  getAppointmentPosition,
  getGridHeight,
  checkConflict,
  DEFAULT_CONFIG,
} from '../utils/calendar-utils'
import type { Appointment, Therapist, DraggableAppointmentData } from '../types'

interface CalendarDayColumnProps {
  therapist: Therapist
  dayDate: Date
  appointments: Appointment[]
  allAppointments: Appointment[]
  isToday: boolean
  activeAppointmentData: DraggableAppointmentData | null
  onResizeStart: (appointmentId: string) => void
  onResizeMove: (deltaY: number) => void
  onResizeEnd: () => void
  resizingAppointmentId: string | null
}

export function CalendarDayColumn({
  therapist,
  dayDate,
  appointments,
  allAppointments,
  isToday,
  activeAppointmentData,
  onResizeStart,
  onResizeMove,
  onResizeEnd,
  resizingAppointmentId,
}: CalendarDayColumnProps) {
  const config = DEFAULT_CONFIG
  const timeSlots = getTimeSlots(config)
  const gridHeight = getGridHeight(config)
  const dayDateStr = format(dayDate, 'yyyy-MM-dd')

  return (
    <div className="relative min-w-[180px] flex-1 border-r border-gray-200 last:border-r-0">
      {/* Column header */}
      <div
        className={cn(
          'sticky top-0 z-30 border-b border-gray-200 bg-white px-2 py-2 text-center',
          isToday && 'bg-green-50',
        )}
      >
        <div className="text-xs font-medium text-gray-500">
          {therapist.firstName} {therapist.lastName}
        </div>
        <div
          className="mx-auto mt-0.5 h-1 w-6 rounded-full"
          style={{ backgroundColor: therapist.color }}
        />
      </div>

      {/* Time slots grid */}
      <div className="relative" style={{ height: `${gridHeight}px` }}>
        {/* Droppable time slots */}
        {timeSlots.map((slot) => {
          const isHourStart = slot.endsWith(':00')

          // Check if dropping the active appointment here would cause conflict
          let slotHasConflict = false
          if (activeAppointmentData) {
            const slotStart = `${dayDateStr}T${slot}:00`
            const durationMs = activeAppointmentData.durationMinutes * 60 * 1000
            const slotEnd = new Date(new Date(slotStart).getTime() + durationMs)
              .toISOString()
              .slice(0, 19)
            slotHasConflict = checkConflict(
              allAppointments,
              therapist.id,
              slotStart,
              slotEnd,
              activeAppointmentData.appointmentId,
            )
          }

          return (
            <DroppableTimeSlot
              key={`${therapist.id}-${dayDateStr}-${slot}`}
              therapistId={therapist.id}
              timeSlot={slot}
              dayDate={dayDateStr}
              pixelsPerSlot={config.pixelsPerSlot}
              isHourStart={isHourStart}
              hasConflict={slotHasConflict}
              isActiveTarget={activeAppointmentData !== null}
            />
          )
        })}

        {/* Appointment blocks */}
        {appointments.map((appointment) => {
          const { top, height } = getAppointmentPosition(appointment, config)
          return (
            <DraggableAppointment
              key={appointment.id}
              appointment={appointment}
              top={top}
              height={height}
              onResizeStart={onResizeStart}
              onResizeMove={onResizeMove}
              onResizeEnd={onResizeEnd}
              isResizing={resizingAppointmentId === appointment.id}
            />
          )
        })}

        {/* Current time indicator */}
        {isToday && <CurrentTimeIndicator config={config} />}
      </div>
    </div>
  )
}
