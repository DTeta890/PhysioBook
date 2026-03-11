import { DragOverlay } from '@dnd-kit/core'
import type { Active } from '@dnd-kit/core'
import { AppointmentBlock } from './AppointmentBlock'
import type { Appointment, DraggableAppointmentData } from '../types'
import { DEFAULT_CONFIG, getAppointmentPosition } from '../utils/calendar-utils'

interface CalendarDragOverlayProps {
  activeItem: Active | null
  appointments: Appointment[]
}

export function CalendarDragOverlay({ activeItem, appointments }: CalendarDragOverlayProps) {
  if (!activeItem) return <DragOverlay />

  const data = activeItem.data.current as DraggableAppointmentData | undefined
  if (!data) return <DragOverlay />

  const appointment = appointments.find((a) => a.id === data.appointmentId)
  if (!appointment) return <DragOverlay />

  const { height } = getAppointmentPosition(appointment, DEFAULT_CONFIG)

  return (
    <DragOverlay dropAnimation={null}>
      <div style={{ width: 180, height: `${height}px` }}>
        <AppointmentBlock
          appointment={appointment}
          top={0}
          height={height}
          isOverlay
        />
      </div>
    </DragOverlay>
  )
}
