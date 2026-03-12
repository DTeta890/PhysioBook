import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { ClipboardList, Clock, Users, CheckCircle, Phone, UserPlus } from 'lucide-react'
import { format } from 'date-fns'
import { PageHeader } from '@/shared/components/PageHeader'
import { Button } from '@/shared/components/Button'
import { Card } from '@/shared/components/Card'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { useWalkInQueue, useWalkInStats, useCallWalkIn, useCompleteWalkIn, useCancelWalkIn } from '../hooks/useWalkIns'
import { useTherapists } from '@/features/therapists/hooks/useTherapists'
import { useToastStore } from '@/shared/stores/useToastStore'
import { CheckInModal } from './CheckInModal'
import { ConvertToAppointmentModal } from './ConvertToAppointmentModal'
import type { WalkInEntry } from '../types'

type StatusFilter = 'all' | 'waiting' | 'in_progress' | 'served' | 'cancelled'

const STATUS_BADGE_VARIANT: Record<string, 'success' | 'warning' | 'error' | 'info' | 'neutral'> = {
  waiting: 'warning',
  in_progress: 'info',
  served: 'success',
  no_show: 'error',
  cancelled: 'error',
}

const STATUS_LABEL_KEY: Record<string, string> = {
  waiting: 'walkins.statusWaiting',
  in_progress: 'walkins.statusInProgress',
  served: 'walkins.statusServed',
  no_show: 'walkins.statusNoShow',
  cancelled: 'walkins.statusCancelled',
}

export function WalkInsPage() {
  const { t } = useTranslation()
  const toast = useToastStore()
  const [activeTab, setActiveTab] = useState<StatusFilter>('all')
  const [showCheckIn, setShowCheckIn] = useState(false)
  const [convertTarget, setConvertTarget] = useState<WalkInEntry | null>(null)
  const [callingId, setCallingId] = useState<string | null>(null)
  const [cancellingId, setCancellingId] = useState<string | null>(null)

  const today = format(new Date(), 'yyyy-MM-dd')
  const statusParam = activeTab === 'all' ? undefined : activeTab
  const { data: queue, isLoading } = useWalkInQueue(statusParam, today)
  const { data: stats } = useWalkInStats(today, today)
  const { data: therapists } = useTherapists()
  const callMutation = useCallWalkIn()
  const completeMutation = useCompleteWalkIn()
  const cancelMutation = useCancelWalkIn()

  const waitingCount = stats?.totalWalkIns ?? 0
  const avgWait = stats?.averageWaitMinutes ?? 0
  const servedCount = stats?.servedCount ?? 0

  const handleCall = (entryId: string, therapistId: string) => {
    callMutation.mutate(
      { id: entryId, therapistId },
      {
        onSuccess: () => {
          toast.show(t('walkins.called'), 'success')
          setCallingId(null)
        },
        onError: () => toast.show(t('common.error'), 'error'),
      },
    )
  }

  const handleComplete = (id: string) => {
    completeMutation.mutate(id, {
      onSuccess: () => toast.show(t('walkins.completed'), 'success'),
      onError: () => toast.show(t('common.error'), 'error'),
    })
  }

  const handleCancel = (id: string) => {
    cancelMutation.mutate(
      { id },
      {
        onSuccess: () => {
          toast.show(t('walkins.cancelled'), 'success')
          setCancellingId(null)
        },
        onError: () => toast.show(t('common.error'), 'error'),
      },
    )
  }

  const tabs: { key: StatusFilter; label: string }[] = [
    { key: 'all', label: t('walkins.all') },
    { key: 'waiting', label: t('walkins.statusWaiting') },
    { key: 'in_progress', label: t('walkins.statusInProgress') },
    { key: 'served', label: t('walkins.statusServed') },
    { key: 'cancelled', label: t('walkins.statusCancelled') },
  ]

  const activeTherapists = therapists?.filter((th) => th.isActive) ?? []

  return (
    <div>
      <PageHeader
        title={t('nav.walkins')}
        actions={
          <Button onClick={() => setShowCheckIn(true)}>
            <UserPlus className="mr-2 h-4 w-4" />
            {t('walkins.checkIn')}
          </Button>
        }
      />

      {/* Stats bar */}
      <div className="mb-6 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Card className="flex items-center gap-3">
          <div className="rounded-lg bg-yellow-100 p-2">
            <Users className="h-5 w-5 text-yellow-600" />
          </div>
          <div>
            <p className="text-sm text-gray-500">{t('walkins.totalWaiting')}</p>
            <p className="text-xl font-bold text-gray-900">{waitingCount}</p>
          </div>
        </Card>
        <Card className="flex items-center gap-3">
          <div className="rounded-lg bg-blue-100 p-2">
            <Clock className="h-5 w-5 text-blue-600" />
          </div>
          <div>
            <p className="text-sm text-gray-500">{t('walkins.avgWaitTime')}</p>
            <p className="text-xl font-bold text-gray-900">
              {t('walkins.waitMinutes', { minutes: Math.round(avgWait) })}
            </p>
          </div>
        </Card>
        <Card className="flex items-center gap-3">
          <div className="rounded-lg bg-green-100 p-2">
            <CheckCircle className="h-5 w-5 text-green-600" />
          </div>
          <div>
            <p className="text-sm text-gray-500">{t('walkins.servedToday')}</p>
            <p className="text-xl font-bold text-gray-900">{servedCount}</p>
          </div>
        </Card>
      </div>

      {/* Filter tabs */}
      <div className="mb-4 flex gap-1 overflow-x-auto rounded-lg border border-gray-200 bg-gray-50 p-1">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`whitespace-nowrap rounded-md px-3 py-1.5 text-sm font-medium transition-colors ${
              activeTab === tab.key
                ? 'bg-white text-gray-900 shadow-sm'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {/* Queue list */}
      {isLoading ? (
        <div className="flex justify-center py-12">
          <LoadingSpinner size="lg" />
        </div>
      ) : !queue || queue.length === 0 ? (
        <EmptyState
          icon={<ClipboardList className="h-12 w-12" />}
          title={t('walkins.empty')}
          description={t('walkins.emptyDescription')}
          action={
            <Button onClick={() => setShowCheckIn(true)}>
              <UserPlus className="mr-2 h-4 w-4" />
              {t('walkins.checkIn')}
            </Button>
          }
        />
      ) : (
        <div className="space-y-3">
          {queue.map((entry) => (
            <Card key={entry.id} className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  {entry.status === 'waiting' && (
                    <span className="text-sm font-bold text-gray-400">
                      {t('walkins.queuePosition', { position: entry.queuePosition })}
                    </span>
                  )}
                  <span className="text-base font-semibold text-gray-900">
                    {entry.patientName}
                  </span>
                  <Badge variant={entry.priority === 2 ? 'error' : 'neutral'}>
                    {entry.priority === 2 ? t('walkins.priorityUrgent') : t('walkins.priorityNormal')}
                  </Badge>
                  <Badge variant={STATUS_BADGE_VARIANT[entry.status] ?? 'neutral'}>
                    {t(STATUS_LABEL_KEY[entry.status] ?? 'walkins.statusWaiting')}
                  </Badge>
                </div>

                <div className="mt-1 flex flex-wrap gap-x-4 gap-y-1 text-sm text-gray-500">
                  {entry.patientPhone && (
                    <span className="flex items-center gap-1">
                      <Phone className="h-3.5 w-3.5" />
                      {entry.patientPhone}
                    </span>
                  )}
                  {entry.reasonForVisit && (
                    <span>{entry.reasonForVisit}</span>
                  )}
                  {entry.treatmentTypeName && (
                    <span>{entry.treatmentTypeName}</span>
                  )}
                  <span className="flex items-center gap-1">
                    <Clock className="h-3.5 w-3.5" />
                    {t('walkins.waitMinutes', { minutes: entry.waitTimeMinutes })}
                  </span>
                  {entry.assignedTherapistName && (
                    <span>{entry.assignedTherapistName}</span>
                  )}
                </div>

                <div className="mt-1 text-xs text-gray-400">
                  {t('walkins.checkedInAt')} {format(new Date(entry.checkedInAt), 'HH:mm')}
                </div>
              </div>

              {/* Action buttons */}
              <div className="flex flex-wrap gap-2">
                {entry.status === 'waiting' && (
                  <>
                    {callingId === entry.id ? (
                      <div className="flex items-center gap-2">
                        <select
                          className="rounded-lg border border-gray-300 px-2 py-1.5 text-sm"
                          defaultValue=""
                          onChange={(e) => {
                            if (e.target.value) handleCall(entry.id, e.target.value)
                          }}
                        >
                          <option value="" disabled>
                            {t('walkins.selectTherapist')}
                          </option>
                          {activeTherapists.map((th) => (
                            <option key={th.id} value={th.id}>
                              {th.firstName} {th.lastName}
                            </option>
                          ))}
                        </select>
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => setCallingId(null)}
                        >
                          {t('common.cancel')}
                        </Button>
                      </div>
                    ) : (
                      <Button
                        size="sm"
                        onClick={() => setCallingId(entry.id)}
                        disabled={callMutation.isPending}
                      >
                        {t('walkins.call')}
                      </Button>
                    )}
                    {cancellingId === entry.id ? (
                      <div className="flex items-center gap-2">
                        <Button
                          size="sm"
                          variant="outline"
                          className="border-red-300 text-red-700 hover:bg-red-50"
                          onClick={() => handleCancel(entry.id)}
                          disabled={cancelMutation.isPending}
                        >
                          {t('common.confirm')}
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => setCancellingId(null)}
                        >
                          {t('common.cancel')}
                        </Button>
                      </div>
                    ) : (
                      <Button
                        size="sm"
                        variant="outline"
                        className="border-red-300 text-red-700 hover:bg-red-50"
                        onClick={() => setCancellingId(entry.id)}
                      >
                        {t('walkins.cancel')}
                      </Button>
                    )}
                  </>
                )}
                {entry.status === 'in_progress' && (
                  <>
                    <Button
                      size="sm"
                      onClick={() => handleComplete(entry.id)}
                      disabled={completeMutation.isPending}
                    >
                      {t('walkins.complete')}
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => setConvertTarget(entry)}
                    >
                      {t('walkins.convertToAppointment')}
                    </Button>
                  </>
                )}
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Modals */}
      {showCheckIn && <CheckInModal onClose={() => setShowCheckIn(false)} />}
      {convertTarget && (
        <ConvertToAppointmentModal
          walkIn={convertTarget}
          onClose={() => setConvertTarget(null)}
        />
      )}
    </div>
  )
}
