import { useState, useEffect } from 'react'
import { DEFAULT_CONFIG } from '../utils/calendar-utils'
import type { CalendarConfig } from '../types'

interface CurrentTimeIndicatorProps {
  config?: CalendarConfig
}

export function CurrentTimeIndicator({ config = DEFAULT_CONFIG }: CurrentTimeIndicatorProps) {
  const [now, setNow] = useState(new Date())

  useEffect(() => {
    const interval = setInterval(() => setNow(new Date()), 60_000)
    return () => clearInterval(interval)
  }, [])

  const currentMinutes = now.getHours() * 60 + now.getMinutes()
  const startMinutes = config.startHour * 60
  const endMinutes = config.endHour * 60

  // Don't render if current time is outside calendar range
  if (currentMinutes < startMinutes || currentMinutes > endMinutes) return null

  const pixelsPerMinute = config.pixelsPerSlot / config.slotDurationMinutes
  const top = (currentMinutes - startMinutes) * pixelsPerMinute

  return (
    <div
      className="pointer-events-none absolute left-0 right-0 z-20"
      style={{ top: `${top}px` }}
    >
      <div className="flex items-center">
        <div className="h-2.5 w-2.5 shrink-0 rounded-full bg-red-500" />
        <div className="h-0.5 flex-1 bg-red-500" />
      </div>
    </div>
  )
}
