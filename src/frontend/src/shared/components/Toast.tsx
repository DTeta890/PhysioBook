import { useEffect } from 'react'
import { cn } from '@/shared/utils/cn'
import { CheckCircle, XCircle, X } from 'lucide-react'
import { useToastStore } from '@/shared/stores/useToastStore'

export function Toast() {
  const { message, type, hide } = useToastStore()

  useEffect(() => {
    if (!message) return
    const timer = setTimeout(hide, 3000)
    return () => clearTimeout(timer)
  }, [message, hide])

  if (!message) return null

  return (
    <div className="fixed bottom-4 left-1/2 z-50 -translate-x-1/2">
      <div
        className={cn(
          'flex items-center gap-2 rounded-lg px-4 py-3 shadow-lg',
          'animate-[slideUp_0.2s_ease-out]',
          type === 'success'
            ? 'bg-green-600 text-white'
            : 'bg-red-600 text-white',
        )}
      >
        {type === 'success' ? (
          <CheckCircle className="h-5 w-5 shrink-0" />
        ) : (
          <XCircle className="h-5 w-5 shrink-0" />
        )}
        <span className="text-sm font-medium">{message}</span>
        <button
          onClick={hide}
          className="ml-2 shrink-0 rounded p-0.5 hover:bg-white/20"
        >
          <X className="h-4 w-4" />
        </button>
      </div>
    </div>
  )
}
