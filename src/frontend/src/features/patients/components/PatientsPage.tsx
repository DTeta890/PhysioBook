import { Users } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'

export function PatientsPage() {
  const { t } = useTranslation()

  return (
    <div>
      <PageHeader title={t('nav.patients')} />
      <EmptyState
        icon={<Users className="h-12 w-12" />}
        title={t('patients.empty')}
        description={t('patients.emptyDescription')}
      />
    </div>
  )
}
