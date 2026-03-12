import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { ChevronDown, ChevronUp, FileText, CheckCircle, AlertCircle } from 'lucide-react'
import { format } from 'date-fns'
import { Badge } from '@/shared/components/Badge'
import { cn } from '@/shared/utils/cn'
import type { TreatmentNote } from '../types'

interface TreatmentNoteCardProps {
  note: TreatmentNote
}

export function TreatmentNoteCard({ note }: TreatmentNoteCardProps) {
  const { t } = useTranslation()
  const [expanded, setExpanded] = useState(false)

  const formattedDate = format(new Date(note.appointmentDate), 'dd MMM yyyy')

  return (
    <div
      className="rounded-lg border border-gray-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    >
      <button
        type="button"
        onClick={() => setExpanded(!expanded)}
        className="flex w-full items-center justify-between p-4 text-left"
      >
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-purple-100 text-purple-600">
            <FileText className="h-4 w-4" />
          </div>
          <div className="min-w-0">
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium text-gray-900">
                {formattedDate}
              </span>
              <Badge variant={note.isSigned ? 'success' : 'warning'}>
                {note.isSigned ? (
                  <span className="flex items-center gap-1">
                    <CheckCircle className="h-3 w-3" />
                    {t('patients.signed')}
                  </span>
                ) : (
                  <span className="flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {t('patients.unsigned')}
                  </span>
                )}
              </Badge>
            </div>
            <p className="text-sm text-gray-500">{note.therapistName}</p>
            {(note.diagnosis ?? note.assessment) && (
              <p className="mt-1 truncate text-sm text-gray-600">
                {note.diagnosis ?? note.assessment}
              </p>
            )}
          </div>
        </div>
        {expanded ? (
          <ChevronUp className="h-4 w-4 shrink-0 text-gray-400" />
        ) : (
          <ChevronDown className="h-4 w-4 shrink-0 text-gray-400" />
        )}
      </button>

      <div
        className={cn(
          'overflow-hidden transition-all duration-200',
          expanded ? 'max-h-[800px] opacity-100' : 'max-h-0 opacity-0',
        )}
      >
        <div className="space-y-4 border-t border-gray-100 px-4 pb-4 pt-3">
          {note.subjective && (
            <SoapField label={t('patients.subjective')} value={note.subjective} />
          )}
          {note.objective && (
            <SoapField label={t('patients.objective')} value={note.objective} />
          )}
          {note.assessment && (
            <SoapField label={t('patients.assessment')} value={note.assessment} />
          )}
          {note.plan && (
            <SoapField label={t('patients.plan')} value={note.plan} />
          )}
          {note.diagnosis && (
            <SoapField label={t('patients.diagnosis')} value={note.diagnosis} />
          )}
          {note.treatmentProvided && (
            <SoapField label={t('patients.treatmentProvided')} value={note.treatmentProvided} />
          )}

          {(note.painLevelBefore !== null || note.painLevelAfter !== null) && (
            <div>
              <span className="text-xs font-semibold uppercase tracking-wider text-gray-500">
                {t('patients.painLevel')}
              </span>
              <div className="mt-1 flex gap-4">
                {note.painLevelBefore !== null && (
                  <PainIndicator
                    label={t('patients.painBefore', { level: note.painLevelBefore })}
                    value={note.painLevelBefore}
                  />
                )}
                {note.painLevelAfter !== null && (
                  <PainIndicator
                    label={t('patients.painAfter', { level: note.painLevelAfter })}
                    value={note.painLevelAfter}
                  />
                )}
              </div>
            </div>
          )}

          {note.exercisesPrescribed && (
            <SoapField label={t('patients.exercises')} value={note.exercisesPrescribed} />
          )}
          {note.followUpInstructions && (
            <SoapField label={t('patients.followUp')} value={note.followUpInstructions} />
          )}

          {note.isSigned && note.signedAt && (
            <p className="text-xs text-gray-400">
              {t('patients.signedAt', {
                date: format(new Date(note.signedAt), 'dd MMM yyyy HH:mm'),
              })}
            </p>
          )}
        </div>
      </div>
    </div>
  )
}

function SoapField({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="text-xs font-semibold uppercase tracking-wider text-gray-500">
        {label}
      </span>
      <p className="mt-0.5 text-sm text-gray-700">{value}</p>
    </div>
  )
}

function PainIndicator({ label, value }: { label: string; value: number }) {
  const color =
    value <= 3
      ? 'text-green-600'
      : value <= 6
        ? 'text-yellow-600'
        : 'text-red-600'

  return (
    <span className={cn('text-sm font-medium', color)}>
      {label}
    </span>
  )
}
