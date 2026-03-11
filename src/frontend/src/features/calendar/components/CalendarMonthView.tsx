import { useMemo } from 'react'
import { format, isSameMonth, isSameDay, parseISO } from 'date-fns'
import { useTranslation } from 'react-i18next'
import { cn } from '@/shared/utils/cn'
import { getMonthDays, getAppointmentsForDay } from '../utils/calendar-utils'
import type { Appointment } from '../types'

interface CalendarMonthViewProps {
  currentDate: Date
  appointments: Appointment[]
  onDayClick: (date: Date) => void
}

const WEEKDAY_KEYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'] as const
const MAX_VISIBLE_APPOINTMENTS = 3

export function CalendarMonthView({
  currentDate,
  appointments,
  onDayClick,
}: CalendarMonthViewProps) {
  const { t } = useTranslation()
  const today = new Date()

  const monthDays = useMemo(() => getMonthDays(currentDate), [currentDate])

  const weeks = useMemo(() => {
    const result: Date[][] = []
    for (let i = 0; i < monthDays.length; i += 7) {
      result.push(monthDays.slice(i, i + 7))
    }
    return result
  }, [monthDays])

  return (
    <div className="flex h-full flex-col bg-white">
      {/* Weekday headers */}
      <div className="grid grid-cols-7 border-b border-gray-200">
        {WEEKDAY_KEYS.map((day) => (
          <div
            key={day}
            className="px-2 py-2 text-center text-xs font-medium text-gray-500"
          >
            <span className="hidden sm:inline">{day}</span>
            <span className="sm:hidden">{day.charAt(0)}</span>
          </div>
        ))}
      </div>

      {/* Month grid */}
      <div className="grid flex-1 grid-cols-7">
        {weeks.map((week, weekIndex) =>
          week.map((day, dayIndex) => {
            const isCurrentMonth = isSameMonth(day, currentDate)
            const isCurrentDay = isSameDay(day, today)
            const dayAppointments = getAppointmentsForDay(appointments, day)
            const visibleAppointments = dayAppointments.slice(0, MAX_VISIBLE_APPOINTMENTS)
            const remainingCount = dayAppointments.length - MAX_VISIBLE_APPOINTMENTS

            return (
              <button
                key={day.toISOString()}
                onClick={() => onDayClick(day)}
                className={cn(
                  'flex min-h-[80px] flex-col border-b border-r border-gray-200 p-1 text-left transition-colors hover:bg-gray-50',
                  'md:min-h-[100px] md:p-2',
                  !isCurrentMonth && 'bg-gray-50',
                  dayIndex === 6 && 'border-r-0',
                  weekIndex === weeks.length - 1 && 'border-b-0',
                )}
              >
                {/* Day number */}
                <div className="mb-0.5 flex items-center justify-center md:justify-start">
                  <span
                    className={cn(
                      'flex h-6 w-6 items-center justify-center rounded-full text-xs font-medium',
                      'md:h-7 md:w-7 md:text-sm',
                      isCurrentDay && 'bg-green-600 text-white',
                      !isCurrentDay && isCurrentMonth && 'text-gray-900',
                      !isCurrentDay && !isCurrentMonth && 'text-gray-400',
                    )}
                  >
                    {format(day, 'd')}
                  </span>
                </div>

                {/* Appointment indicators */}
                <div className="flex flex-1 flex-col gap-0.5 overflow-hidden">
                  {/* Mobile: dots only */}
                  <div className="flex flex-wrap gap-0.5 md:hidden">
                    {dayAppointments.slice(0, 5).map((apt) => (
                      <div
                        key={apt.id}
                        className="h-1.5 w-1.5 rounded-full"
                        style={{ backgroundColor: apt.color }}
                      />
                    ))}
                    {dayAppointments.length > 5 && (
                      <span className="text-[9px] text-gray-400">
                        +{dayAppointments.length - 5}
                      </span>
                    )}
                  </div>

                  {/* Desktop: compact appointment bars */}
                  <div className="hidden flex-col gap-0.5 md:flex">
                    {visibleAppointments.map((apt) => {
                      const startFormatted = format(parseISO(apt.startTime), 'HH:mm')
                      return (
                        <div
                          key={apt.id}
                          className="truncate rounded px-1 py-0.5 text-[10px] leading-tight"
                          style={{
                            backgroundColor: `${apt.color}20`,
                            borderLeft: `2px solid ${apt.color}`,
                          }}
                        >
                          <span className="font-medium text-gray-700">{startFormatted}</span>{' '}
                          <span className="text-gray-600">{apt.patientName}</span>
                        </div>
                      )
                    })}
                    {remainingCount > 0 && (
                      <span className="px-1 text-[10px] font-medium text-gray-500">
                        {t('calendar.moreAppointments', { count: remainingCount })}
                      </span>
                    )}
                  </div>
                </div>
              </button>
            )
          }),
        )}
      </div>
    </div>
  )
}
