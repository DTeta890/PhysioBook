import { Calendar } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function CalendarPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.calendar')} />
      <EmptyState
        icon={<Calendar className="h-12 w-12" />}
        title={t('calendar.empty')}
        description={t('calendar.emptyDescription')}
      />
    </div>
  )
}
