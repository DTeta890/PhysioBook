import { ClipboardList } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function WalkInsPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.walkins')} />
      <EmptyState
        icon={<ClipboardList className="h-12 w-12" />}
        title={t('walkins.empty')}
        description={t('walkins.emptyDescription')}
      />
    </div>
  )
}
