import { useCallback, useRef } from 'react'
import { GripHorizontal } from 'lucide-react'

interface ResizeHandleProps {
  appointmentId: string
  top: number
  onResizeStart: (appointmentId: string) => void
  onResizeMove: (deltaY: number) => void
  onResizeEnd: () => void
}

export function ResizeHandle({
  appointmentId,
  top,
  onResizeStart,
  onResizeMove,
  onResizeEnd,
}: ResizeHandleProps) {
  const startYRef = useRef(0)

  const handlePointerDown = useCallback(
    (e: React.PointerEvent) => {
      e.stopPropagation()
      e.preventDefault()
      startYRef.current = e.clientY
      onResizeStart(appointmentId)

      const target = e.currentTarget as HTMLElement
      target.setPointerCapture(e.pointerId)
    },
    [appointmentId, onResizeStart],
  )

  const handlePointerMove = useCallback(
    (e: React.PointerEvent) => {
      e.stopPropagation()
      const deltaY = e.clientY - startYRef.current
      onResizeMove(deltaY)
    },
    [onResizeMove],
  )

  const handlePointerUp = useCallback(
    (e: React.PointerEvent) => {
      e.stopPropagation()
      onResizeEnd()
    },
    [onResizeEnd],
  )

  return (
    <div
      className="absolute left-1 right-1 flex cursor-ns-resize items-center justify-center rounded-b"
      style={{ top: `${top}px`, height: '6px' }}
      onPointerDown={handlePointerDown}
      onPointerMove={handlePointerMove}
      onPointerUp={handlePointerUp}
    >
      <GripHorizontal className="h-3 w-3 text-gray-400" />
    </div>
  )
}
