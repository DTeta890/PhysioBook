import { useTranslation } from 'react-i18next'
import { Package, Calendar } from 'lucide-react'
import { format } from 'date-fns'
import { Badge } from '@/shared/components/Badge'
import type { PatientPackage } from '../types'

interface PatientPackageCardProps {
  pkg: PatientPackage
}

const statusVariantMap: Record<string, 'success' | 'info' | 'warning' | 'error' | 'neutral'> = {
  active: 'success',
  Active: 'success',
  completed: 'info',
  Completed: 'info',
  expired: 'warning',
  Expired: 'warning',
  cancelled: 'error',
  Cancelled: 'error',
}

const statusI18nMap: Record<string, string> = {
  active: 'patients.packageActive',
  Active: 'patients.packageActive',
  completed: 'patients.packageCompleted',
  Completed: 'patients.packageCompleted',
  expired: 'patients.packageExpired',
  Expired: 'patients.packageExpired',
  cancelled: 'patients.packageCancelled',
  Cancelled: 'patients.packageCancelled',
}

export function PatientPackageCard({ pkg }: PatientPackageCardProps) {
  const { t } = useTranslation()

  const progressPercent =
    pkg.totalSessions > 0
      ? Math.round((pkg.sessionsUsed / pkg.totalSessions) * 100)
      : 0

  const statusVariant = statusVariantMap[pkg.status] ?? 'neutral'
  const statusLabel = statusI18nMap[pkg.status]
    ? t(statusI18nMap[pkg.status])
    : pkg.status

  return (
    <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
      <div className="flex items-start justify-between">
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-green-100 text-green-600">
            <Package className="h-4 w-4" />
          </div>
          <div>
            <h4 className="text-sm font-medium text-gray-900">
              {pkg.treatmentPackageName}
            </h4>
            <p className="text-xs text-gray-500">{pkg.treatmentTypeName}</p>
          </div>
        </div>
        <Badge variant={statusVariant}>{statusLabel}</Badge>
      </div>

      <div className="mt-4">
        <div className="flex items-center justify-between text-sm">
          <span className="text-gray-600">
            {t('patients.sessionsUsed', {
              used: pkg.sessionsUsed,
              total: pkg.totalSessions,
            })}
          </span>
          <span className="text-xs text-gray-500">
            {t('patients.sessionsRemaining', { count: pkg.sessionsRemaining })}
          </span>
        </div>
        <div className="mt-1.5 h-2 w-full overflow-hidden rounded-full bg-gray-100">
          <div
            className="h-full rounded-full bg-green-500 transition-all"
            style={{ width: `${progressPercent}%` }}
          />
        </div>
      </div>

      <div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-gray-500">
        <span className="flex items-center gap-1">
          <Calendar className="h-3 w-3" />
          {t('patients.purchasedOn', {
            date: format(new Date(pkg.purchasedAt), 'dd MMM yyyy'),
          })}
        </span>
        {pkg.expiresAt && (
          <span>
            {t('patients.expiresOn', {
              date: format(new Date(pkg.expiresAt), 'dd MMM yyyy'),
            })}
          </span>
        )}
        <span className="font-medium text-gray-700">
          {pkg.price.toLocaleString()} ALL
        </span>
      </div>

      {pkg.notes && (
        <p className="mt-2 text-xs text-gray-500">{pkg.notes}</p>
      )}
    </div>
  )
}
