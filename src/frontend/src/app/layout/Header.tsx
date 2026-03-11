import { useTranslation } from 'react-i18next'
import { Menu, Bell, LogOut } from 'lucide-react'
import { useAuth } from '@/features/auth/hooks/useAuth'
import { useLogout } from '@/features/auth/hooks/useLogout'

interface HeaderProps {
  onMenuClick: () => void
}

export function Header({ onMenuClick }: HeaderProps) {
  const { t, i18n } = useTranslation()
  const user = useAuth((s) => s.user)
  const handleLogout = useLogout()

  const toggleLanguage = () => {
    const next = i18n.language === 'sq' ? 'en' : 'sq'
    void i18n.changeLanguage(next)
  }

  return (
    <header className="flex h-16 items-center justify-between border-b border-gray-200 bg-white px-4 lg:px-6">
      <button
        onClick={onMenuClick}
        className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 lg:hidden"
      >
        <Menu className="h-5 w-5" />
      </button>

      <div className="hidden lg:block">
        <span className="text-sm text-gray-500">{t('app.welcome')}</span>
      </div>

      <div className="flex items-center gap-3">
        <button
          onClick={toggleLanguage}
          className="rounded-lg px-2.5 py-1.5 text-xs font-medium text-gray-600 hover:bg-gray-100 uppercase"
        >
          {i18n.language === 'sq' ? 'EN' : 'SQ'}
        </button>

        <button className="relative rounded-lg p-2 text-gray-500 hover:bg-gray-100">
          <Bell className="h-5 w-5" />
        </button>

        {user && (
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary-100 text-sm font-medium text-primary-700">
              {user.firstName.charAt(0).toUpperCase()}
            </div>
            <span className="hidden text-sm font-medium text-gray-700 md:block">
              {user.firstName} {user.lastName}
            </span>
          </div>
        )}

        <button
          onClick={() => void handleLogout()}
          className="rounded-lg p-2 text-gray-500 hover:bg-gray-100"
          title={t('auth.logout')}
        >
          <LogOut className="h-5 w-5" />
        </button>
      </div>
    </header>
  )
}
