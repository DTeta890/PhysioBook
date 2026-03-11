import { useMemo } from 'react'
import { format } from 'date-fns'
import { ChevronLeft, ChevronRight } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { Button } from '@/shared/components/Button'
import { useCalendarStore } from '../hooks/useCalendarStore'
import { CalendarWeekView } from './CalendarWeekView'
import { getWeekDays } from '../utils/calendar-utils'
import { getMockAppointments, MOCK_THERAPISTS } from '../utils/mock-data'
import type { CalendarConfig } from '../types'

const DEFAULT_CONFIG: CalendarConfig = {
  startHour: 8,
  endHour: 20,
  slotDurationMinutes: 15,
}

export function CalendarPage() {
  const { t } = useTranslation()
  const { currentDate, goToPrev, goToNext, goToToday } = useCalendarStore()

  const weekDays = useMemo(() => getWeekDays(currentDate), [currentDate])
  const weekStart = weekDays[0]
  const weekEnd = weekDays[weekDays.length - 1]

  // Mock data - will be replaced with TanStack Query hooks when API is ready
  const appointments = useMemo(() => getMockAppointments(), [])
  const therapists = MOCK_THERAPISTS

  return (
    <div className="flex h-full flex-col">
      {/* Header / Toolbar */}
      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-gray-200 bg-white px-4 py-3">
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" onClick={goToPrev}>
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button variant="outline" size="sm" onClick={goToNext}>
            <ChevronRight className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="sm" onClick={goToToday}>
            {t('calendar.today')}
          </Button>
        </div>

        <h2 className="text-lg font-semibold text-gray-900">
          {format(weekStart, 'MMM d')} - {format(weekEnd, 'MMM d, yyyy')}
        </h2>

        <div className="flex items-center gap-1">
          <Button variant="secondary" size="sm">
            {t('calendar.weekView')}
          </Button>
        </div>
      </div>

      {/* Calendar body */}
      <div className="flex-1 overflow-hidden">
        <CalendarWeekView
          therapists={therapists}
          appointments={appointments}
          config={DEFAULT_CONFIG}
        />
      </div>
    </div>
  )
}
