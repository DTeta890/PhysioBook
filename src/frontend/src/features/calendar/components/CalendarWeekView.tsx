import { useMemo, useState } from 'react'
import { format, isToday } from 'date-fns'
import { useTranslation } from 'react-i18next'
import { cn } from '@/shared/utils/cn'
import type { Therapist } from '@/features/therapists/types'
import type { Appointment, CalendarConfig } from '../types'
import { getWeekDays, getTimeSlots } from '../utils/calendar-utils'
import { CalendarDayColumn } from './CalendarDayColumn'
import { CurrentTimeIndicator } from './CurrentTimeIndicator'
import { useCalendarStore } from '../hooks/useCalendarStore'

interface CalendarWeekViewProps {
  therapists: Therapist[]
  appointments: Appointment[]
  config: CalendarConfig
}

const DAY_KEYS = [
  'monday',
  'tuesday',
  'wednesday',
  'thursday',
  'friday',
  'saturday',
] as const

export function CalendarWeekView({
  therapists,
  appointments,
  config,
}: CalendarWeekViewProps) {
  const { t } = useTranslation()
  const { currentDate, selectedDayIndex, setSelectedDayIndex } =
    useCalendarStore()

  const weekDays = useMemo(() => getWeekDays(currentDate), [currentDate])
  const timeSlots = useMemo(() => getTimeSlots(config), [config])
  const selectedDay = weekDays[selectedDayIndex]

  // For mobile: therapist selector
  const [selectedTherapistIndex, setSelectedTherapistIndex] = useState(0)
  const activeTherapists = therapists.filter((th) => th.isActive)

  const pixelsPerSlot = 20
  const totalHeight = timeSlots.length * pixelsPerSlot

  const showCurrentTimeLine = selectedDay && isToday(selectedDay)

  return (
    <div className="flex flex-col">
      {/* Day tabs */}
      <div className="flex border-b border-gray-200 bg-white">
        {weekDays.map((day, i) => (
          <button
            key={i}
            type="button"
            onClick={() => setSelectedDayIndex(i)}
            className={cn(
              'flex-1 px-2 py-2 text-center text-sm font-medium transition-colors',
              i === selectedDayIndex
                ? 'border-b-2 border-primary-600 text-primary-700 bg-primary-50'
                : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50',
              isToday(day) && i !== selectedDayIndex && 'text-primary-600',
            )}
          >
            <div className="text-xs">
              {t(`calendar.${DAY_KEYS[i]}`)}
            </div>
            <div
              className={cn(
                'text-sm',
                isToday(day) &&
                  'inline-flex h-6 w-6 items-center justify-center rounded-full bg-primary-600 text-white',
              )}
            >
              {format(day, 'd')}
            </div>
          </button>
        ))}
      </div>

      {/* Mobile therapist selector */}
      <div className="flex overflow-x-auto border-b border-gray-200 bg-gray-50 lg:hidden">
        {activeTherapists.map((therapist, i) => (
          <button
            key={therapist.id}
            type="button"
            onClick={() => setSelectedTherapistIndex(i)}
            className={cn(
              'shrink-0 px-3 py-1.5 text-xs font-medium transition-colors',
              i === selectedTherapistIndex
                ? 'border-b-2 border-primary-600 text-primary-700 bg-white'
                : 'text-gray-600 hover:text-gray-900',
            )}
          >
            <div className="flex items-center gap-1.5">
              <span
                className="inline-block h-2 w-2 rounded-full"
                style={{
                  backgroundColor: therapist.color ?? '#3B82F6',
                }}
              />
              {therapist.firstName} {therapist.lastName[0]}.
            </div>
          </button>
        ))}
      </div>

      {/* Calendar grid */}
      <div className="overflow-auto">
        <div className="flex min-w-0">
          {/* Time gutter */}
          <div className="sticky left-0 z-10 w-14 shrink-0 bg-white border-r border-gray-200">
            {timeSlots.map((slot, i) => (
              <div
                key={slot}
                className="relative pr-2 text-right"
                style={{ height: `${pixelsPerSlot}px` }}
              >
                {i % (60 / config.slotDurationMinutes) === 0 && (
                  <span className="absolute -top-2 right-2 text-xs text-gray-500">
                    {slot}
                  </span>
                )}
              </div>
            ))}
          </div>

          {/* Desktop: all therapist columns */}
          <div className="hidden flex-1 lg:flex relative">
            {/* Therapist headers */}
            {activeTherapists.length > 0 ? (
              <div className="flex flex-1">
                {activeTherapists.map((therapist) => (
                  <div
                    key={therapist.id}
                    className="relative min-w-[180px] flex-1"
                  >
                    {/* Header */}
                    <div className="sticky top-0 z-10 border-b border-r border-gray-200 bg-white px-2 py-1.5 text-center">
                      <div className="flex items-center justify-center gap-1.5">
                        <span
                          className="inline-block h-2.5 w-2.5 rounded-full"
                          style={{
                            backgroundColor:
                              therapist.color ?? '#3B82F6',
                          }}
                        />
                        <span className="text-sm font-medium text-gray-900">
                          {therapist.firstName} {therapist.lastName}
                        </span>
                      </div>
                      {therapist.specialization && (
                        <div className="truncate text-xs text-gray-500">
                          {therapist.specialization}
                        </div>
                      )}
                    </div>
                    {/* Day column */}
                    <div className="relative">
                      <CalendarDayColumn
                        therapist={therapist}
                        day={selectedDay}
                        appointments={appointments}
                        config={config}
                      />
                      {showCurrentTimeLine && (
                        <CurrentTimeIndicator config={config} />
                      )}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="flex flex-1 items-center justify-center py-20 text-gray-500">
                {t('calendar.noTherapists')}
              </div>
            )}
          </div>

          {/* Mobile: single therapist column */}
          <div className="flex flex-1 lg:hidden">
            {activeTherapists.length > 0 && activeTherapists[selectedTherapistIndex] ? (
              <div className="relative min-w-0 flex-1">
                <div className="sticky top-0 z-10 border-b border-gray-200 bg-white px-2 py-1.5 text-center">
                  <div className="flex items-center justify-center gap-1.5">
                    <span
                      className="inline-block h-2.5 w-2.5 rounded-full"
                      style={{
                        backgroundColor:
                          activeTherapists[selectedTherapistIndex].color ??
                          '#3B82F6',
                      }}
                    />
                    <span className="text-sm font-medium text-gray-900">
                      {activeTherapists[selectedTherapistIndex].firstName}{' '}
                      {activeTherapists[selectedTherapistIndex].lastName}
                    </span>
                  </div>
                </div>
                <div className="relative" style={{ height: `${totalHeight}px` }}>
                  <CalendarDayColumn
                    therapist={activeTherapists[selectedTherapistIndex]}
                    day={selectedDay}
                    appointments={appointments}
                    config={config}
                  />
                  {showCurrentTimeLine && (
                    <CurrentTimeIndicator config={config} />
                  )}
                </div>
              </div>
            ) : (
              <div className="flex flex-1 items-center justify-center py-20 text-gray-500">
                {t('calendar.noTherapists')}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
