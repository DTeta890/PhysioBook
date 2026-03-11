import { useDraggable } from '@dnd-kit/core'
import { CSS } from '@dnd-kit/utilities'
import { AppointmentBlock } from './AppointmentBlock'
import { ResizeHandle } from './ResizeHandle'
import type { Appointment, DraggableAppointmentData } from '../types'

interface DraggableAppointmentProps {
  appointment: Appointment
  top: number
  height: number
  onResizeStart: (appointmentId: string) => void
  onResizeMove: (deltaY: number) => void
  onResizeEnd: () => void
  isResizing: boolean
}

export function DraggableAppointment({
  appointment,
  top,
  height,
  onResizeStart,
  onResizeMove,
  onResizeEnd,
  isResizing,
}: DraggableAppointmentProps) {
  const dragData: DraggableAppointmentData = {
    appointmentId: appointment.id,
    therapistId: appointment.therapistId,
    startTime: appointment.startTime,
    endTime: appointment.endTime,
    durationMinutes: appointment.durationMinutes,
  }

  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: `appointment-${appointment.id}`,
    data: dragData,
    disabled: isResizing,
  })

  const style = transform
    ? { transform: CSS.Translate.toString(transform) }
    : undefined

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
      className="absolute left-0 right-0 z-10"
    >
      <AppointmentBlock
        appointment={appointment}
        top={top}
        height={height}
        isDragging={isDragging}
      />
      {!isDragging && (
        <ResizeHandle
          appointmentId={appointment.id}
          top={top + height - 6}
          onResizeStart={onResizeStart}
          onResizeMove={onResizeMove}
          onResizeEnd={onResizeEnd}
        />
      )}
    </div>
  )
}
