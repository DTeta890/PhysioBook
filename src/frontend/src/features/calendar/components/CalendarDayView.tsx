import { useState, useCallback, useRef } from 'react'
import { format, isSameDay } from 'date-fns'
import { DndContext, PointerSensor, TouchSensor, useSensor, useSensors } from '@dnd-kit/core'
import type { DragStartEvent, DragEndEvent, Active } from '@dnd-kit/core'
import { useTranslation } from 'react-i18next'
import { cn } from '@/shared/utils/cn'
import { CalendarDayColumn } from './CalendarDayColumn'
import { CalendarDragOverlay } from './CalendarDragOverlay'
import { AppointmentFormModal } from './AppointmentFormModal'
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

interface CalendarDayViewProps {
  date: Date
  therapists: Therapist[]
  appointments: Appointment[]
  onAppointmentMove: (params: MoveAppointmentParams) => void
  onAppointmentResize: (params: ResizeAppointmentParams) => void
}

export function CalendarDayView({
  date,
  therapists,
  appointments,
  onAppointmentMove,
  onAppointmentResize,
}: CalendarDayViewProps) {
  const { t } = useTranslation()
  const config = DEFAULT_CONFIG
  const timeSlots = getTimeSlots(config)
  const gridHeight = getGridHeight(config)
  const today = new Date()
  const isToday = isSameDay(date, today)
  const dayStr = format(date, 'yyyy-MM-dd')

  const [activeItem, setActiveItem] = useState<Active | null>(null)
  const [activeAppointmentData, setActiveAppointmentData] =
    useState<DraggableAppointmentData | null>(null)
  const [slotToCreate, setSlotToCreate] = useState<TimeSlotData | null>(null)
  const [selectedTherapistIndex, setSelectedTherapistIndex] = useState(0)

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

  // Filter hours for time gutter
  const hourLabels = timeSlots.filter((s) => s.endsWith(':00'))

  // Filter appointments for this day
  const dayAppointments = appointments.filter((a) => a.startTime.startsWith(dayStr))

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
      const snappedDuration = Math.max(
        config.slotDurationMinutes,
        Math.round(newDuration / config.slotDurationMinutes) * config.slotDurationMinutes,
      )

      const appointment = resizingAppointmentRef.current
      const newEndTime = calculateEndTime(appointment.startTime, snappedDuration)

      const endHour = parseInt(newEndTime.slice(11, 13), 10)
      if (endHour > config.endHour) return

      const hasConflict = checkConflict(
        appointments,
        appointment.therapistId,
        appointment.startTime,
        newEndTime,
        appointment.id,
      )

      if (hasConflict) return

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
        {/* Day header */}
        <div
          className={cn(
            'border-b border-gray-200 bg-white px-4 py-2',
            isToday && 'bg-green-50',
          )}
        >
          <span className="text-sm font-medium text-gray-700">
            {format(date, 'EEEE, MMMM d, yyyy')}
          </span>
          {isToday && (
            <span className="ml-2 rounded bg-green-600 px-1.5 py-0.5 text-[10px] font-medium text-white">
              {t('calendar.today', 'Today')}
            </span>
          )}
        </div>

        {/* Therapist tabs (mobile) */}
        <div className="flex gap-1 overflow-x-auto border-b border-gray-200 bg-white px-2 py-1 md:hidden">
          {therapists.map((therapist, index) => (
            <button
              key={therapist.id}
              onClick={() => setSelectedTherapistIndex(index)}
              className={cn(
                'shrink-0 rounded-lg px-3 py-1.5 text-xs font-medium transition-colors',
                index === selectedTherapistIndex
                  ? 'bg-green-600 text-white'
                  : 'text-gray-600 hover:bg-gray-100',
              )}
            >
              {therapist.firstName} {therapist.lastName}
            </button>
          ))}
        </div>

        {/* Calendar grid */}
        <div className="overflow-auto">
          {/* Desktop view: all therapists side by side */}
          <div className="hidden md:block">
            <div className="flex">
              {/* Time gutter */}
              <div className="sticky left-0 z-20 w-14 shrink-0 border-r border-gray-200 bg-white">
                <div className="h-[42px] border-b border-gray-200" />
                <div className="relative" style={{ height: `${gridHeight}px` }}>
                  {hourLabels.map((slot) => {
                    const hour = parseInt(slot.split(':')[0], 10)
                    const minutesFromStart = (hour - config.startHour) * 60
                    const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
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
                    dayDate={date}
                    appointments={columnAppointments}
                    allAppointments={appointments}
                    isToday={isToday}
                    activeAppointmentData={activeAppointmentData}
                    onResizeStart={handleResizeStart}
                    onResizeMove={handleResizeMove}
                    onResizeEnd={handleResizeEnd}
                    resizingAppointmentId={resizingAppointmentId}
                    onSlotClick={setSlotToCreate}
                  />
                )
              })}
            </div>
          </div>

          {/* Mobile view: single therapist */}
          <div className="md:hidden">
            <div className="flex">
              {/* Time gutter */}
              <div className="sticky left-0 z-20 w-14 shrink-0 border-r border-gray-200 bg-white">
                <div className="h-[42px] border-b border-gray-200" />
                <div className="relative" style={{ height: `${gridHeight}px` }}>
                  {hourLabels.map((slot) => {
                    const hour = parseInt(slot.split(':')[0], 10)
                    const minutesFromStart = (hour - config.startHour) * 60
                    const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
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

              {/* Single therapist column */}
              {therapists[selectedTherapistIndex] && (
                <CalendarDayColumn
                  key={`${therapists[selectedTherapistIndex].id}-${dayStr}-mobile`}
                  therapist={therapists[selectedTherapistIndex]}
                  dayDate={date}
                  appointments={dayAppointments.filter(
                    (a) => a.therapistId === therapists[selectedTherapistIndex].id,
                  )}
                  allAppointments={appointments}
                  isToday={isToday}
                  activeAppointmentData={activeAppointmentData}
                  onResizeStart={handleResizeStart}
                  onResizeMove={handleResizeMove}
                  onResizeEnd={handleResizeEnd}
                  resizingAppointmentId={resizingAppointmentId}
                  onSlotClick={setSlotToCreate}
                />
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Drag overlay */}
      <CalendarDragOverlay activeItem={activeItem} appointments={appointments} />

      {/* Quick-create appointment modal */}
      {slotToCreate && (
        <AppointmentFormModal
          slotData={slotToCreate}
          onClose={() => setSlotToCreate(null)}
          onSuccess={() => setSlotToCreate(null)}
        />
      )}
    </DndContext>
  )
}
