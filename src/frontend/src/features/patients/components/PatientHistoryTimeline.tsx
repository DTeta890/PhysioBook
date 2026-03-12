import { useState, useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import { Calendar, Clock, User, XCircle, AlertTriangle } from 'lucide-react'
import { format } from 'date-fns'
import { cn } from '@/shared/utils/cn'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { usePatientTimeline } from '../hooks/usePatientHistory'
import { TreatmentNoteCard } from './TreatmentNoteCard'
import { PatientPackageCard } from './PatientPackageCard'
import type { TimelineEvent, AppointmentSummary, TreatmentNote, PatientPackage } from '../types'

type FilterType = 'all' | 'appointment' | 'treatment_note' | 'package_purchase'

interface PatientHistoryTimelineProps {
  patientId: string
}

const appointmentStatusVariant: Record<string, 'success' | 'info' | 'warning' | 'error' | 'neutral'> = {
  completed: 'success',
  Completed: 'success',
  scheduled: 'info',
  Scheduled: 'info',
  confirmed: 'info',
  Confirmed: 'info',
  'in-progress': 'info',
  'In-Progress': 'info',
  cancelled: 'error',
  Cancelled: 'error',
  'no-show': 'warning',
  'No-Show': 'warning',
  NoShow: 'warning',
}

export function PatientHistoryTimeline({ patientId }: PatientHistoryTimelineProps) {
  const { t } = useTranslation()
  const [filter, setFilter] = useState<FilterType>('all')
  const { data: events, isLoading, isError } = usePatientTimeline(patientId)

  const filteredEvents = useMemo(() => {
    if (filter === 'all') return events
    return events.filter((e) => e.type === filter)
  }, [events, filter])

  const groupedByMonth = useMemo(() => {
    const groups: Record<string, TimelineEvent[]> = {}
    for (const event of filteredEvents) {
      const key = format(new Date(event.date), 'MMMM yyyy')
      if (!groups[key]) groups[key] = []
      groups[key].push(event)
    }
    return groups
  }, [filteredEvents])

  const filterButtons: { key: FilterType; label: string }[] = [
    { key: 'all', label: t('common.search') === 'Search' ? 'All' : 'Te gjitha' },
    { key: 'appointment', label: t('patients.appointments') },
    { key: 'treatment_note', label: t('patients.treatmentNotes') },
    { key: 'package_purchase', label: t('patients.treatmentPackages') },
  ]

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

  return (
    <div>
      {/* Filter buttons */}
      <div className="mb-6 flex flex-wrap gap-2">
        {filterButtons.map((btn) => (
          <button
            key={btn.key}
            type="button"
            onClick={() => setFilter(btn.key)}
            className={cn(
              'rounded-full px-4 py-1.5 text-sm font-medium transition-colors',
              filter === btn.key
                ? 'bg-green-600 text-white'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200',
            )}
          >
            {btn.label}
          </button>
        ))}
      </div>

      {/* Timeline */}
      {filteredEvents.length === 0 ? (
        <EmptyState
          icon={<Calendar className="h-12 w-12" />}
          title={t('patients.noHistory')}
          description={t('patients.noHistoryDescription')}
        />
      ) : (
        <div className="space-y-8">
          {Object.entries(groupedByMonth).map(([monthLabel, monthEvents]) => (
            <div key={monthLabel}>
              <h3 className="mb-4 text-sm font-semibold uppercase tracking-wider text-gray-500">
                {monthLabel}
              </h3>
              <div className="relative space-y-4 pl-6 before:absolute before:left-[9px] before:top-2 before:h-[calc(100%-16px)] before:w-0.5 before:bg-gray-200">
                {monthEvents.map((event) => (
                  <TimelineItem key={event.id} event={event} />
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

function TimelineItem({ event }: { event: TimelineEvent }) {
  return (
    <div className="relative">
      {/* Timeline dot */}
      <div
        className={cn(
          'absolute -left-6 top-4 h-[18px] w-[18px] rounded-full border-2 border-white',
          event.type === 'appointment' && 'bg-blue-500',
          event.type === 'treatment_note' && 'bg-purple-500',
          event.type === 'package_purchase' && 'bg-green-500',
        )}
      />
      {event.type === 'appointment' && (
        <AppointmentTimelineCard appointment={event.data as AppointmentSummary} />
      )}
      {event.type === 'treatment_note' && (
        <TreatmentNoteCard note={event.data as TreatmentNote} />
      )}
      {event.type === 'package_purchase' && (
        <PatientPackageCard pkg={event.data as PatientPackage} />
      )}
    </div>
  )
}

function AppointmentTimelineCard({ appointment }: { appointment: AppointmentSummary }) {
  const { t } = useTranslation()

  const startDate = new Date(appointment.startTime)
  const endDate = new Date(appointment.endTime)
  const formattedDate = format(startDate, 'dd MMM yyyy')
  const formattedTime = `${format(startDate, 'HH:mm')} - ${format(endDate, 'HH:mm')}`
  const statusVariant = appointmentStatusVariant[appointment.status] ?? 'neutral'

  const statusIcon =
    appointment.status.toLowerCase() === 'cancelled' ? (
      <XCircle className="h-3 w-3" />
    ) : appointment.status.toLowerCase() === 'no-show' || appointment.status.toLowerCase() === 'noshow' ? (
      <AlertTriangle className="h-3 w-3" />
    ) : null

  return (
    <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
      <div className="flex items-start justify-between">
        <div className="flex items-center gap-3">
          <div
            className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full"
            style={{
              backgroundColor: appointment.color ? `${appointment.color}20` : '#dbeafe',
              color: appointment.color ?? '#2563eb',
            }}
          >
            <Calendar className="h-4 w-4" />
          </div>
          <div>
            <h4 className="text-sm font-medium text-gray-900">
              {appointment.treatmentTypeName}
            </h4>
            <div className="mt-0.5 flex items-center gap-2 text-xs text-gray-500">
              <span className="flex items-center gap-1">
                <Clock className="h-3 w-3" />
                {formattedDate} {formattedTime}
              </span>
            </div>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {appointment.isWalkIn && (
            <Badge variant="neutral">{t('patients.walkIn')}</Badge>
          )}
          <Badge variant={statusVariant}>
            <span className="flex items-center gap-1">
              {statusIcon}
              {appointment.status}
            </span>
          </Badge>
        </div>
      </div>

      <div className="mt-2 flex items-center gap-1 text-xs text-gray-500">
        <User className="h-3 w-3" />
        {appointment.therapistName}
      </div>

      {appointment.notes && (
        <p className="mt-2 text-sm text-gray-600">{appointment.notes}</p>
      )}

      {appointment.cancellationReason && (
        <p className="mt-1 text-xs text-red-500">
          {appointment.cancellationReason}
        </p>
      )}
    </div>
  )
}
