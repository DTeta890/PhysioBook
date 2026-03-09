import { Settings } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function SettingsPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.settings')} />
      <EmptyState
        icon={<Settings className="h-12 w-12" />}
        title={t('settings.empty')}
        description={t('settings.emptyDescription')}
      />
    </div>
  )
}
