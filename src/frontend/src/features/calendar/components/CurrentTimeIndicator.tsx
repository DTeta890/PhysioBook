import { useState, useEffect } from 'react'
import type { CalendarConfig } from '../types'

interface CurrentTimeIndicatorProps {
  config: CalendarConfig
}

export function CurrentTimeIndicator({ config }: CurrentTimeIndicatorProps) {
  const [now, setNow] = useState(new Date())

  useEffect(() => {
    const interval = setInterval(() => setNow(new Date()), 60_000)
    return () => clearInterval(interval)
  }, [])

  const currentMinutes = now.getHours() * 60 + now.getMinutes()
  const configStartMinutes = config.startHour * 60
  const configEndMinutes = config.endHour * 60

  if (currentMinutes < configStartMinutes || currentMinutes > configEndMinutes) {
    return null
  }

  const pixelsPerSlot = 20
  const pixelsPerMinute = pixelsPerSlot / config.slotDurationMinutes
  const top = (currentMinutes - configStartMinutes) * pixelsPerMinute

  return (
    <div
      className="pointer-events-none absolute left-0 right-0 z-20"
      style={{ top: `${top}px` }}
    >
      <div className="flex items-center">
        <div className="h-2.5 w-2.5 -translate-x-1/2 rounded-full bg-red-500" />
        <div className="h-0.5 w-full bg-red-500" />
      </div>
    </div>
  )
}
