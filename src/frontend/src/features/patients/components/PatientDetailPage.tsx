import { useState } from 'react'
import type { ReactNode } from 'react'
import { User, Clock, Package } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { useParams } from '@tanstack/react-router'
import { PageHeader } from '@/shared/components/PageHeader'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { cn } from '@/shared/utils/cn'
import { PatientHistoryTimeline } from './PatientHistoryTimeline'
import { PatientPackageCard } from './PatientPackageCard'
import { usePatientPackages } from '../hooks/usePatientPackages'

type TabKey = 'overview' | 'history' | 'packages'

export function PatientDetailPage() {
  const { t } = useTranslation()
  const [activeTab, setActiveTab] = useState<TabKey>('overview')
  const { patientId } = useParams({ strict: false }) as { patientId?: string }

  const tabs: { key: TabKey; label: string; icon: ReactNode }[] = [
    { key: 'overview', label: t('patients.overview'), icon: <User className="h-4 w-4" /> },
    { key: 'history', label: t('patients.history'), icon: <Clock className="h-4 w-4" /> },
    { key: 'packages', label: t('patients.packages'), icon: <Package className="h-4 w-4" /> },
  ]

  return (
    <div>
      <PageHeader title={t('patients.detail')} />

      {/* Tab bar */}
      <div className="mb-6 border-b border-gray-200">
        <nav className="-mb-px flex gap-6" aria-label="Tabs">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              type="button"
              onClick={() => setActiveTab(tab.key)}
              className={cn(
                'flex items-center gap-2 border-b-2 px-1 pb-3 text-sm font-medium transition-colors',
                activeTab === tab.key
                  ? 'border-green-600 text-green-600'
                  : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700',
              )}
            >
              {tab.icon}
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab content */}
      {activeTab === 'overview' && (
        <EmptyState
          icon={<User className="h-12 w-12" />}
          title={t('patients.detailEmpty')}
        />
      )}

      {activeTab === 'history' && patientId && (
        <PatientHistoryTimeline patientId={patientId} />
      )}

      {activeTab === 'history' && !patientId && (
        <EmptyState
          icon={<Clock className="h-12 w-12" />}
          title={t('patients.noHistory')}
          description={t('patients.noHistoryDescription')}
        />
      )}

      {activeTab === 'packages' && patientId && (
        <PackagesTab patientId={patientId} />
      )}

      {activeTab === 'packages' && !patientId && (
        <EmptyState
          icon={<Package className="h-12 w-12" />}
          title={t('patients.noPackages')}
          description={t('patients.noPackagesDescription')}
        />
      )}
    </div>
  )
}

function PackagesTab({ patientId }: { patientId: string }) {
  const { t } = useTranslation()
  const { data: packages, isLoading, isError } = usePatientPackages(patientId)

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  if (isError) {
    return (
      <div className="flex items-center justify-center py-12">
        <p className="text-sm text-red-500">{t('common.error')}</p>
      </div>
    )
  }

  if (!packages || packages.length === 0) {
    return (
      <EmptyState
        icon={<Package className="h-12 w-12" />}
        title={t('patients.noPackages')}
        description={t('patients.noPackagesDescription')}
      />
    )
  }

  return (
    <div className="grid gap-4 md:grid-cols-2">
      {packages.map((pkg) => (
        <PatientPackageCard key={pkg.id} pkg={pkg} />
      ))}
    </div>
  )
}
