import { useDroppable } from '@dnd-kit/core'
import { cn } from '@/shared/utils/cn'
import type { TimeSlotData } from '../types'

interface DroppableTimeSlotProps {
  therapistId: string
  timeSlot: string
  dayDate: string
  pixelsPerSlot: number
  isHourStart: boolean
  hasConflict: boolean
  isActiveTarget: boolean
}

export function DroppableTimeSlot({
  therapistId,
  timeSlot,
  dayDate,
  pixelsPerSlot,
  isHourStart,
  hasConflict,
  isActiveTarget,
}: DroppableTimeSlotProps) {
  const slotData: TimeSlotData = { therapistId, timeSlot, dayDate }

  const { setNodeRef, isOver } = useDroppable({
    id: `slot-${therapistId}-${dayDate}-${timeSlot}`,
    data: slotData,
  })

  return (
    <div
      ref={setNodeRef}
      className={cn(
        'border-b border-gray-100',
        isHourStart && 'border-b-gray-200',
        isOver && !hasConflict && 'bg-green-100',
        isOver && hasConflict && 'bg-red-100',
        isActiveTarget && !hasConflict && !isOver && 'bg-green-50',
        isActiveTarget && hasConflict && !isOver && 'bg-red-50',
      )}
      style={{ height: `${pixelsPerSlot}px` }}
    />
  )
}
