import { cn } from '@/shared/utils/cn'
import type { Appointment, CalendarConfig } from '../types'
import { getAppointmentPosition, formatTime } from '../utils/calendar-utils'

interface AppointmentBlockProps {
  appointment: Appointment
  therapistColor: string | null
  config: CalendarConfig
}

const statusColors: Record<Appointment['status'], string> = {
  scheduled: 'bg-blue-500',
  confirmed: 'bg-green-500',
  in_progress: 'bg-yellow-500',
  completed: 'bg-gray-400',
  cancelled: 'bg-red-500',
  no_show: 'bg-orange-500',
}

export function AppointmentBlock({
  appointment,
  therapistColor,
  config,
}: AppointmentBlockProps) {
  const { top, height } = getAppointmentPosition(
    appointment.startTime,
    appointment.endTime,
    config,
  )

  const bgColor =
    appointment.color ?? therapistColor ?? '#3B82F6'

  const isCancelledOrNoShow =
    appointment.status === 'cancelled' || appointment.status === 'no_show'

  const isCompact = height < 60

  return (
    <div
      className={cn(
        'absolute left-0.5 right-0.5 overflow-hidden rounded px-1.5 py-0.5 text-xs cursor-pointer transition-shadow hover:shadow-md border border-white/20',
        isCancelledOrNoShow && 'opacity-50',
      )}
      style={{
        top: `${top}px`,
        height: `${height}px`,
        backgroundColor: bgColor,
        color: '#fff',
      }}
      title={`${appointment.patientName} - ${appointment.treatmentTypeName} (${formatTime(appointment.startTime)} - ${formatTime(appointment.endTime)})`}
    >
      <div className="flex items-center gap-1">
        <span
          className={cn(
            'inline-block h-1.5 w-1.5 shrink-0 rounded-full',
            statusColors[appointment.status],
          )}
        />
        <span
          className={cn(
            'truncate font-semibold leading-tight',
            isCancelledOrNoShow && 'line-through',
          )}
        >
          {appointment.patientName}
        </span>
      </div>
      {!isCompact && (
        <>
          <div className="truncate leading-tight text-white/90">
            {appointment.treatmentTypeName}
          </div>
          <div className="truncate leading-tight text-white/80">
            {formatTime(appointment.startTime)} -{' '}
            {formatTime(appointment.endTime)}
          </div>
        </>
      )}
    </div>
  )
}
