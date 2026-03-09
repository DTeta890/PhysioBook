import { CalendarCheck } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function BookingPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.booking')} />
      <EmptyState
        icon={<CalendarCheck className="h-12 w-12" />}
        title={t('booking.empty')}
        description={t('booking.emptyDescription')}
      />
    </div>
  )
}
