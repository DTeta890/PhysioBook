import { User } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function PatientDetailPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('patients.detail')} />
      <EmptyState
        icon={<User className="h-12 w-12" />}
        title={t('patients.detailEmpty')}
      />
    </div>
  )
}
