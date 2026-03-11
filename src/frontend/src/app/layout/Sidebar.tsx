import { Link, useMatchRoute } from '@tanstack/react-router'
import { useTranslation } from 'react-i18next'
import {
  Calendar,
  Users,
  UserCheck,
  ClipboardList,
  BarChart3,
  Settings,
  X,
} from 'lucide-react'
import { cn } from '@/shared/utils/cn'

interface SidebarProps {
  open: boolean
  onClose: () => void
}

const navItems = [
  { to: '/calendar' as const, icon: Calendar, labelKey: 'nav.calendar' },
  { to: '/therapists' as const, icon: UserCheck, labelKey: 'nav.therapists' },
  { to: '/patients' as const, icon: Users, labelKey: 'nav.patients' },
  { to: '/walk-ins' as const, icon: ClipboardList, labelKey: 'nav.walkins' },
  { to: '/analytics' as const, icon: BarChart3, labelKey: 'nav.analytics' },
  { to: '/settings' as const, icon: Settings, labelKey: 'nav.settings' },
] as const

export function Sidebar({ open, onClose }: SidebarProps) {
  const { t } = useTranslation()
  const matchRoute = useMatchRoute()

  return (
    <>
      {/* Mobile overlay */}
      {open && (
        <div
          className="fixed inset-0 z-40 bg-black/50 lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={cn(
          'fixed inset-y-0 left-0 z-50 flex w-64 flex-col bg-white border-r border-gray-200 transition-transform lg:static lg:translate-x-0',
          open ? 'translate-x-0' : '-translate-x-full',
        )}
      >
        <div className="flex h-16 items-center justify-between px-6 border-b border-gray-100">
          <h1 className="text-xl font-bold text-primary-700">PhysioBook</h1>
          <button onClick={onClose} className="lg:hidden text-gray-500 hover:text-gray-700">
            <X className="h-5 w-5" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 px-3 py-4">
          {navItems.map((item) => {
            const isActive = !!matchRoute({ to: item.to, fuzzy: true })
            return (
              <Link
                key={item.to}
                to={item.to}
                onClick={onClose}
                className={cn(
                  'flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-primary-50 text-primary-700'
                    : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900',
                )}
              >
                <item.icon className="h-5 w-5" />
                {t(item.labelKey)}
              </Link>
            )
          })}
        </nav>
      </aside>
    </>
  )
}
