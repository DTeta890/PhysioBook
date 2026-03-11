import { useState, useCallback, useRef } from 'react'
import { format, isSameDay } from 'date-fns'
import { DndContext, PointerSensor, TouchSensor, useSensor, useSensors } from '@dnd-kit/core'
import type { DragStartEvent, DragEndEvent, Active } from '@dnd-kit/core'
import { useTranslation } from 'react-i18next'
import { cn } from '@/shared/utils/cn'
import { CalendarDayColumn } from './CalendarDayColumn'
import { CalendarDragOverlay } from './CalendarDragOverlay'
import {
  getTimeSlots,
  getGridHeight,
  buildDateTimeFromSlot,
  calculateEndTime,
  checkConflict,
  DEFAULT_CONFIG,
} from '../utils/calendar-utils'
import { useToastStore } from '@/shared/stores/useToastStore'
import type {
  Appointment,
  Therapist,
  DraggableAppointmentData,
  TimeSlotData,
  MoveAppointmentParams,
  ResizeAppointmentParams,
} from '../types'

interface CalendarWeekViewProps {
  weekDays: Date[]
  therapists: Therapist[]
  appointments: Appointment[]
  selectedDayIndex: number
  onDaySelect: (index: number) => void
  onAppointmentMove: (params: MoveAppointmentParams) => void
  onAppointmentResize: (params: ResizeAppointmentParams) => void
}

export function CalendarWeekView({
  weekDays,
  therapists,
  appointments,
  selectedDayIndex,
  onDaySelect,
  onAppointmentMove,
  onAppointmentResize,
}: CalendarWeekViewProps) {
  const { t } = useTranslation()
  const config = DEFAULT_CONFIG
  const timeSlots = getTimeSlots(config)
  const gridHeight = getGridHeight(config)
  const today = new Date()

  const [activeItem, setActiveItem] = useState<Active | null>(null)
  const [activeAppointmentData, setActiveAppointmentData] =
    useState<DraggableAppointmentData | null>(null)

  // Resize state
  const [resizingAppointmentId, setResizingAppointmentId] = useState<string | null>(null)
  const resizeStartHeightRef = useRef(0)
  const resizingAppointmentRef = useRef<Appointment | null>(null)

  const showToast = useToastStore((s) => s.show)

  // DnD sensors
  const pointerSensor = useSensor(PointerSensor, {
    activationConstraint: { distance: 5 },
  })
  const touchSensor = useSensor(TouchSensor, {
    activationConstraint: { delay: 200, tolerance: 5 },
  })
  const sensors = useSensors(pointerSensor, touchSensor)

  // Filter hours for time gutter (only show on-the-hour labels)
  const hourLabels = timeSlots.filter((s) => s.endsWith(':00'))

  // On mobile, show single selected day; on desktop show all weekdays
  const selectedDay = weekDays[selectedDayIndex]

  const handleDragStart = useCallback((event: DragStartEvent) => {
    const data = event.active.data.current as DraggableAppointmentData | undefined
    setActiveItem(event.active)
    setActiveAppointmentData(data ?? null)
  }, [])

  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      setActiveItem(null)
      setActiveAppointmentData(null)

      const { active, over } = event
      if (!over) return

      const dragData = active.data.current as DraggableAppointmentData | undefined
      const dropData = over.data.current as TimeSlotData | undefined
      if (!dragData || !dropData) return

      const newStartTime = buildDateTimeFromSlot(dropData.dayDate, dropData.timeSlot)
      const newEndTime = calculateEndTime(newStartTime, dragData.durationMinutes)

      // Check for conflicts
      const hasConflict = checkConflict(
        appointments,
        dropData.therapistId,
        newStartTime,
        newEndTime,
        dragData.appointmentId,
      )

      if (hasConflict) {
        showToast(t('calendar.conflictError'), 'error')
        return
      }

      // Check if anything actually changed
      if (
        dragData.startTime === newStartTime &&
        dragData.therapistId === dropData.therapistId
      ) {
        return
      }

      onAppointmentMove({
        appointmentId: dragData.appointmentId,
        newTherapistId: dropData.therapistId,
        newStartTime,
        newEndTime,
      })

      showToast(t('calendar.appointmentMoved'), 'success')
    },
    [appointments, onAppointmentMove, showToast, t],
  )

  const handleDragCancel = useCallback(() => {
    setActiveItem(null)
    setActiveAppointmentData(null)
  }, [])

  // Resize handlers
  const handleResizeStart = useCallback(
    (appointmentId: string) => {
      const appointment = appointments.find((a) => a.id === appointmentId)
      if (!appointment) return
      setResizingAppointmentId(appointmentId)
      resizingAppointmentRef.current = appointment
      const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
      resizeStartHeightRef.current = appointment.durationMinutes * pixelsPerMinute
    },
    [appointments, config],
  )

  const handleResizeMove = useCallback(
    (deltaY: number) => {
      if (!resizingAppointmentRef.current) return

      const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
      const newHeight = resizeStartHeightRef.current + deltaY
      const newDuration = Math.round(newHeight / pixelsPerMinute)
      // Snap to slot duration increments, minimum 1 slot
      const snappedDuration = Math.max(
        config.slotDurationMinutes,
        Math.round(newDuration / config.slotDurationMinutes) * config.slotDurationMinutes,
      )

      const appointment = resizingAppointmentRef.current
      const newEndTime = calculateEndTime(appointment.startTime, snappedDuration)

      // Don't exceed calendar end
      const endHour = parseInt(newEndTime.slice(11, 13), 10)
      if (endHour > config.endHour) return

      // Check for conflicts
      const hasConflict = checkConflict(
        appointments,
        appointment.therapistId,
        appointment.startTime,
        newEndTime,
        appointment.id,
      )

      if (hasConflict) return

      // Live preview via optimistic update
      onAppointmentResize({
        appointmentId: appointment.id,
        newEndTime,
        newDurationMinutes: snappedDuration,
      })
    },
    [appointments, config, onAppointmentResize],
  )

  const handleResizeEnd = useCallback(() => {
    if (resizingAppointmentRef.current) {
      showToast(t('calendar.appointmentResized'), 'success')
    }
    setResizingAppointmentId(null)
    resizingAppointmentRef.current = null
  }, [showToast, t])

  return (
    <DndContext
      sensors={sensors}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
      onDragCancel={handleDragCancel}
    >
      <div className="flex flex-col">
        {/* Day tabs (mobile) */}
        <div className="flex gap-1 overflow-x-auto border-b border-gray-200 bg-white px-2 py-1 md:hidden">
          {weekDays.map((day, index) => (
            <button
              key={day.toISOString()}
              onClick={() => onDaySelect(index)}
              className={cn(
                'shrink-0 rounded-lg px-3 py-1.5 text-xs font-medium transition-colors',
                index === selectedDayIndex
                  ? 'bg-green-600 text-white'
                  : 'text-gray-600 hover:bg-gray-100',
                isSameDay(day, today) && index !== selectedDayIndex && 'text-green-600',
              )}
            >
              <div>{format(day, 'EEE')}</div>
              <div className="text-sm font-bold">{format(day, 'd')}</div>
            </button>
          ))}
        </div>

        {/* Calendar grid */}
        <div className="overflow-auto">
          {/* Desktop: all days; Mobile: selected day only */}
          {/* Desktop view */}
          <div className="hidden md:block">
            {weekDays.map((day, dayIndex) => {
              const dayStr = format(day, 'yyyy-MM-dd')
              const dayAppointments = appointments.filter((a) =>
                a.startTime.startsWith(dayStr),
              )
              const isCurrentDay = isSameDay(day, today)

              if (dayAppointments.length === 0 && !isCurrentDay && dayIndex > 4) {
                // Skip empty weekend days on desktop
                return null
              }

              return (
                <div key={day.toISOString()} className="mb-4">
                  {/* Day header */}
                  <div
                    className={cn(
                      'sticky top-0 z-30 border-b border-gray-200 bg-white px-4 py-2',
                      isCurrentDay && 'bg-green-50',
                    )}
                  >
                    <span className="text-sm font-medium text-gray-700">
                      {format(day, 'EEEE, MMM d')}
                    </span>
                    {isCurrentDay && (
                      <span className="ml-2 rounded bg-green-600 px-1.5 py-0.5 text-[10px] font-medium text-white">
                        Today
                      </span>
                    )}
                  </div>

                  {/* Therapist columns for this day */}
                  <div className="flex">
                    {/* Time gutter */}
                    <div
                      className="sticky left-0 z-20 w-14 shrink-0 border-r border-gray-200 bg-white"
                    >
                      {/* Spacer for column header */}
                      <div className="h-[42px] border-b border-gray-200" />
                      <div className="relative" style={{ height: `${gridHeight}px` }}>
                        {hourLabels.map((slot) => {
                          const hour = parseInt(slot.split(':')[0], 10)
                          const minutesFromStart = (hour - config.startHour) * 60
                          const pixelsPerMinute =
                            config.pixelsPerSlot / config.slotDurationMinutes
                          const top = minutesFromStart * pixelsPerMinute

                          return (
                            <div
                              key={slot}
                              className="absolute right-1 text-[10px] text-gray-400"
                              style={{ top: `${top - 6}px` }}
                            >
                              {slot}
                            </div>
                          )
                        })}
                      </div>
                    </div>

                    {/* Therapist columns */}
                    {therapists.map((therapist) => {
                      const columnAppointments = dayAppointments.filter(
                        (a) => a.therapistId === therapist.id,
                      )

                      return (
                        <CalendarDayColumn
                          key={`${therapist.id}-${dayStr}`}
                          therapist={therapist}
                          dayDate={day}
                          appointments={columnAppointments}
                          allAppointments={appointments}
                          isToday={isCurrentDay}
                          activeAppointmentData={activeAppointmentData}
                          onResizeStart={handleResizeStart}
                          onResizeMove={handleResizeMove}
                          onResizeEnd={handleResizeEnd}
                          resizingAppointmentId={resizingAppointmentId}
                        />
                      )
                    })}
                  </div>
                </div>
              )
            })}
          </div>

          {/* Mobile view: single day */}
          <div className="md:hidden">
            {selectedDay && (
              <div>
                <div className="flex">
                  {/* Time gutter */}
                  <div className="sticky left-0 z-20 w-14 shrink-0 border-r border-gray-200 bg-white">
                    <div className="h-[42px] border-b border-gray-200" />
                    <div className="relative" style={{ height: `${gridHeight}px` }}>
                      {hourLabels.map((slot) => {
                        const hour = parseInt(slot.split(':')[0], 10)
                        const minutesFromStart = (hour - config.startHour) * 60
                        const pixelsPerMinute =
                          config.pixelsPerSlot / config.slotDurationMinutes
                        const top = minutesFromStart * pixelsPerMinute

                        return (
                          <div
                            key={slot}
                            className="absolute right-1 text-[10px] text-gray-400"
                            style={{ top: `${top - 6}px` }}
                          >
                            {slot}
                          </div>
                        )
                      })}
                    </div>
                  </div>

                  {/* Therapist columns for selected day */}
                  {therapists.map((therapist) => {
                    const dayStr = format(selectedDay, 'yyyy-MM-dd')
                    const columnAppointments = appointments.filter(
                      (a) =>
                        a.therapistId === therapist.id &&
                        a.startTime.startsWith(dayStr),
                    )

                    return (
                      <CalendarDayColumn
                        key={`${therapist.id}-${dayStr}`}
                        therapist={therapist}
                        dayDate={selectedDay}
                        appointments={columnAppointments}
                        allAppointments={appointments}
                        isToday={isSameDay(selectedDay, today)}
                        activeAppointmentData={activeAppointmentData}
                        onResizeStart={handleResizeStart}
                        onResizeMove={handleResizeMove}
                        onResizeEnd={handleResizeEnd}
                        resizingAppointmentId={resizingAppointmentId}
                      />
                    )
                  })}
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Drag overlay */}
      <CalendarDragOverlay activeItem={activeItem} appointments={appointments} />
    </DndContext>
  )
}
