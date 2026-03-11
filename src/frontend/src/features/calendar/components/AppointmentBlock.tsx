import { cn } from '@/shared/utils/cn'
import { Clock, User } from 'lucide-react'
import { format, parseISO } from 'date-fns'
import type { Appointment } from '../types'

interface AppointmentBlockProps {
  appointment: Appointment
  top: number
  height: number
  isDragging?: boolean
  isOverlay?: boolean
}

const statusClasses: Record<string, string> = {
  scheduled: 'border-l-4',
  confirmed: 'border-l-4',
  'in-progress': 'border-l-4 ring-2 ring-offset-1',
  completed: 'border-l-4 opacity-60',
  cancelled: 'border-l-4 opacity-40 line-through',
  'no-show': 'border-l-4 opacity-50',
}

export function AppointmentBlock({
  appointment,
  top,
  height,
  isDragging = false,
  isOverlay = false,
}: AppointmentBlockProps) {
  const startFormatted = format(parseISO(appointment.startTime), 'HH:mm')
  const endFormatted = format(parseISO(appointment.endTime), 'HH:mm')
  const isCompact = height < 40
  const isTiny = height < 28

  return (
    <div
      className={cn(
        'absolute left-0.5 right-0.5 overflow-hidden rounded-md px-1.5 py-0.5',
        'cursor-grab select-none transition-shadow',
        statusClasses[appointment.status] ?? 'border-l-4',
        isDragging && 'opacity-30',
        isOverlay && 'shadow-xl ring-2 ring-green-400 scale-[1.02]',
        !isDragging && !isOverlay && 'hover:shadow-md hover:z-10',
      )}
      style={{
        top: isOverlay ? 0 : `${top}px`,
        height: isOverlay ? '100%' : `${Math.max(height - 1, 16)}px`,
        backgroundColor: `${appointment.color}20`,
        borderLeftColor: appointment.color,
        position: isOverlay ? 'relative' : 'absolute',
      }}
    >
      {isTiny ? (
        <div className="flex items-center gap-1 truncate text-[10px] font-medium text-gray-800">
          <span>{startFormatted}</span>
          <span className="truncate">{appointment.patientName}</span>
        </div>
      ) : isCompact ? (
        <div className="flex flex-col gap-0">
          <div className="flex items-center gap-1 truncate text-[11px] font-semibold text-gray-900">
            <span>{appointment.patientName}</span>
          </div>
          <div className="flex items-center gap-1 text-[10px] text-gray-600">
            <Clock className="h-2.5 w-2.5" />
            <span>{startFormatted} - {endFormatted}</span>
          </div>
        </div>
      ) : (
        <div className="flex flex-col gap-0.5">
          <div className="flex items-center gap-1 truncate text-xs font-semibold text-gray-900">
            <User className="h-3 w-3 shrink-0" />
            <span className="truncate">{appointment.patientName}</span>
          </div>
          <div className="flex items-center gap-1 text-[10px] text-gray-600">
            <Clock className="h-2.5 w-2.5 shrink-0" />
            <span>{startFormatted} - {endFormatted}</span>
          </div>
          <div className="truncate text-[10px] text-gray-500">
            {appointment.treatmentType}
          </div>
        </div>
      )}
    </div>
  )
}
