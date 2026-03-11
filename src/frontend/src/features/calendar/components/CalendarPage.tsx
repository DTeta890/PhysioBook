import { useMemo } from 'react'
import { format } from 'date-fns'
import { ChevronLeft, ChevronRight, Calendar } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { CalendarWeekView } from './CalendarWeekView'
import { useCalendarStore } from '../hooks/useCalendarStore'
import { useAppointments, useMoveAppointment, useResizeAppointment } from '../hooks/useAppointments'
import { getWeekDays } from '../utils/calendar-utils'
import { mockTherapists } from '../utils/mock-data'
import { Toast } from '@/shared/components/Toast'
import { cn } from '@/shared/utils/cn'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'

export function CalendarPage() {
  const { t } = useTranslation()
  const { currentDate, selectedDayIndex, setSelectedDayIndex, goToToday, goForward, goBack } =
    useCalendarStore()

  const weekDays = useMemo(() => getWeekDays(currentDate), [currentDate])

  const startDate = format(weekDays[0], 'yyyy-MM-dd')
  const endDate = format(weekDays[6], 'yyyy-MM-dd')

  const { data: appointments, isLoading } = useAppointments(startDate, endDate)
  const moveAppointment = useMoveAppointment()
  const resizeAppointment = useResizeAppointment()

  return (
    <div className="flex h-full flex-col">
      {/* Header */}
      <div className="flex items-center justify-between border-b border-gray-200 bg-white px-4 py-3">
        <div className="flex items-center gap-2">
          <Calendar className="h-5 w-5 text-green-600" />
          <h1 className="text-lg font-bold text-gray-900">{t('nav.calendar')}</h1>
        </div>

        <div className="flex items-center gap-2">
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
            {format(weekDays[0], 'MMM d')} - {format(weekDays[6], 'MMM d, yyyy')}
          </span>
        </div>
      </div>

      {/* Calendar content */}
      <div className="flex-1 overflow-hidden">
        {isLoading ? (
          <div className="flex h-full items-center justify-center">
            <LoadingSpinner />
          </div>
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
