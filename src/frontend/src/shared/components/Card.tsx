import type { ReactNode } from 'react'
import { cn } from '@/shared/utils/cn'

interface CardProps {
  title?: string
  children: ReactNode
  className?: string
  padding?: boolean
}

export function Card({ title, children, className, padding = true }: CardProps) {
  return (
    <div
      className={cn(
        'rounded-xl border border-gray-200 bg-white shadow-sm',
        padding && 'p-6',
        className,
      )}
    >
      {title && (
        <h3 className="mb-4 text-lg font-semibold text-gray-900">{title}</h3>
      )}
      {children}
    </div>
  )
}
