import { BarChart3 } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function AnalyticsPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.analytics')} />
      <EmptyState
        icon={<BarChart3 className="h-12 w-12" />}
        title={t('analytics.empty')}
        description={t('analytics.emptyDescription')}
      />
    </div>
  )
}
