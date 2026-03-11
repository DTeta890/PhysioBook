import { useMemo, useCallback } from 'react'
import { format } from 'date-fns'
import { ChevronLeft, ChevronRight, Calendar } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { CalendarWeekView } from './CalendarWeekView'
import { CalendarDayView } from './CalendarDayView'
import { CalendarMonthView } from './CalendarMonthView'
import { useCalendarStore } from '../hooks/useCalendarStore'
import { useAppointments, useMoveAppointment, useResizeAppointment } from '../hooks/useAppointments'
import { getWeekDays, getMonthDays } from '../utils/calendar-utils'
import { mockTherapists } from '../utils/mock-data'
import { useCalendarSignalR } from '../hooks/useCalendarSignalR'
import { Toast } from '@/shared/components/Toast'
import { cn } from '@/shared/utils/cn'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import type { CalendarView } from '../types'

const VIEW_OPTIONS: CalendarView[] = ['day', 'week', 'month']

export function CalendarPage() {
  const { t } = useTranslation()
  useCalendarSignalR()
  const {
    currentDate,
    view,
    selectedDayIndex,
    setSelectedDayIndex,
    setView,
    setCurrentDate,
    goToToday,
    goForward,
    goBack,
  } = useCalendarStore()

  const weekDays = useMemo(() => getWeekDays(currentDate), [currentDate])
  const monthDays = useMemo(() => getMonthDays(currentDate), [currentDate])

  // Determine date range for fetching appointments based on view
  const { startDate, endDate } = useMemo(() => {
    if (view === 'day') {
      const dayStr = format(currentDate, 'yyyy-MM-dd')
      return { startDate: dayStr, endDate: dayStr }
    }
    if (view === 'month') {
      return {
        startDate: format(monthDays[0], 'yyyy-MM-dd'),
        endDate: format(monthDays[monthDays.length - 1], 'yyyy-MM-dd'),
      }
    }
    // week
    return {
      startDate: format(weekDays[0], 'yyyy-MM-dd'),
      endDate: format(weekDays[6], 'yyyy-MM-dd'),
    }
  }, [view, currentDate, weekDays, monthDays])

  const { data: appointments, isLoading } = useAppointments(startDate, endDate)
  const moveAppointment = useMoveAppointment()
  const resizeAppointment = useResizeAppointment()

  // Format the date range display based on view
  const dateRangeLabel = useMemo(() => {
    if (view === 'day') {
      return format(currentDate, 'MMMM d, yyyy')
    }
    if (view === 'month') {
      return format(currentDate, 'MMMM yyyy')
    }
    // week
    return `${format(weekDays[0], 'MMM d')} - ${format(weekDays[6], 'MMM d, yyyy')}`
  }, [view, currentDate, weekDays])

  const handleMonthDayClick = useCallback(
    (date: Date) => {
      setCurrentDate(date)
      setView('day')
    },
    [setCurrentDate, setView],
  )

  const viewLabelKey = (v: CalendarView) => {
    if (v === 'day') return 'calendar.day'
    if (v === 'week') return 'calendar.week'
    return 'calendar.month'
  }

  return (
    <div className="flex h-full flex-col">
      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-2 border-b border-gray-200 bg-white px-4 py-3">
        <div className="flex items-center gap-2">
          <Calendar className="h-5 w-5 text-green-600" />
          <h1 className="text-lg font-bold text-gray-900">{t('nav.calendar')}</h1>
        </div>

        <div className="flex items-center gap-2">
          {/* View switcher */}
          <div className="flex rounded-lg border border-gray-300">
            {VIEW_OPTIONS.map((v) => (
              <button
                key={v}
                onClick={() => setView(v)}
                className={cn(
                  'px-2.5 py-1 text-xs font-medium transition-colors first:rounded-l-lg last:rounded-r-lg',
                  v === view
                    ? 'bg-green-600 text-white'
                    : 'bg-white text-gray-700 hover:bg-gray-50',
                )}
              >
                {t(viewLabelKey(v))}
              </button>
            ))}
          </div>

          {/* Navigation */}
          <button
            onClick={goBack}
            className="rounded-lg p-1.5 text-gray-500 hover:bg-gray-100"
          >
            <ChevronLeft className="h-5 w-5" />
          </button>

          <button
            onClick={goToToday}
            className={cn(
              'rounded-lg px-3 py-1.5 text-xs font-medium',
              'border border-gray-300 text-gray-700 hover:bg-gray-50',
            )}
          >
            {t('calendar.today', 'Today')}
          </button>

          <button
            onClick={goForward}
            className="rounded-lg p-1.5 text-gray-500 hover:bg-gray-100"
          >
            <ChevronRight className="h-5 w-5" />
          </button>

          <span className="ml-2 text-sm font-medium text-gray-600">
            {dateRangeLabel}
          </span>
        </div>
      </div>

      {/* Calendar content */}
      <div className="flex-1 overflow-hidden">
        {isLoading ? (
          <div className="flex h-full items-center justify-center">
            <LoadingSpinner />
          </div>
        ) : view === 'day' ? (
          <CalendarDayView
            date={currentDate}
            therapists={mockTherapists}
            appointments={appointments ?? []}
            onAppointmentMove={(params) => moveAppointment.mutate(params)}
            onAppointmentResize={(params) => resizeAppointment.mutate(params)}
          />
        ) : view === 'month' ? (
          <CalendarMonthView
            currentDate={currentDate}
            appointments={appointments ?? []}
            onDayClick={handleMonthDayClick}
          />
        ) : (
          <CalendarWeekView
            weekDays={weekDays}
            therapists={mockTherapists}
            appointments={appointments ?? []}
            selectedDayIndex={selectedDayIndex}
            onDaySelect={setSelectedDayIndex}
            onAppointmentMove={(params) => moveAppointment.mutate(params)}
            onAppointmentResize={(params) => resizeAppointment.mutate(params)}
          />
        )}
      </div>

      {/* Toast notifications */}
      <Toast />
    </div>
  )
}
