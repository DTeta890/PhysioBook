import { useForm } from 'react-hook-form'
import { z } from 'zod/v4'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { X } from 'lucide-react'
import { Button } from '@/shared/components/Button'
import { Input } from '@/shared/components/Input'
import { useCheckIn } from '../hooks/useWalkIns'
import { useTreatmentTypes } from '@/features/treatment-types/hooks/useTreatmentTypes'
import { useToastStore } from '@/shared/stores/useToastStore'

const checkInSchema = z.object({
  patientName: z.string().min(1, 'Patient name is required').max(200),
  patientPhone: z.string().max(20).optional().or(z.literal('')),
  treatmentTypeId: z.string().optional().or(z.literal('')),
  reasonForVisit: z.string().max(500).optional().or(z.literal('')),
  priority: z.number().min(1).max(2),
  notes: z.string().max(1000).optional().or(z.literal('')),
})

type CheckInFormData = z.infer<typeof checkInSchema>

interface CheckInModalProps {
  onClose: () => void
}

export function CheckInModal({ onClose }: CheckInModalProps) {
  const { t } = useTranslation()
  const checkInMutation = useCheckIn()
  const { data: treatmentTypes } = useTreatmentTypes()
  const toast = useToastStore()

  const form = useForm<CheckInFormData>({
    resolver: zodResolver(checkInSchema),
    defaultValues: {
      patientName: '',
      patientPhone: '',
      treatmentTypeId: '',
      reasonForVisit: '',
      priority: 1,
      notes: '',
    },
  })

  const handleSubmit = (data: CheckInFormData) => {
    checkInMutation.mutate(
      {
        patientName: data.patientName,
        patientPhone: data.patientPhone || undefined,
        treatmentTypeId: data.treatmentTypeId || undefined,
        reasonForVisit: data.reasonForVisit || undefined,
        priority: data.priority,
        notes: data.notes || undefined,
      },
      {
        onSuccess: () => {
          toast.show(t('walkins.checkedIn'), 'success')
          onClose()
        },
        onError: () => {
          toast.show(t('common.error'), 'error')
        },
      },
    )
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="fixed inset-0 bg-black/50" onClick={onClose} />
      <div className="relative z-10 w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">
            {t('walkins.checkIn')}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
          <Input
            label={t('walkins.patientName')}
            error={form.formState.errors.patientName?.message}
            {...form.register('patientName')}
          />

          <Input
            label={t('walkins.patientPhone')}
            error={form.formState.errors.patientPhone?.message}
            {...form.register('patientPhone')}
          />

          <div className="w-full">
            <label
              htmlFor="treatmentTypeId"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('calendar.treatmentType')}
            </label>
            <select
              id="treatmentTypeId"
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('treatmentTypeId')}
            >
              <option value="">{t('common.noResults')}</option>
              {treatmentTypes?.map((tt) => (
                <option key={tt.id} value={tt.id}>
                  {tt.name}
                </option>
              ))}
            </select>
          </div>

          <div className="w-full">
            <label
              htmlFor="reasonForVisit"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('walkins.reasonForVisit')}
            </label>
            <textarea
              id="reasonForVisit"
              rows={2}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('reasonForVisit')}
            />
          </div>

          <div className="w-full">
            <label className="mb-1 block text-sm font-medium text-gray-700">
              {t('walkins.priority')}
            </label>
            <div className="flex gap-3">
              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  value={1}
                  className="h-4 w-4 border-gray-300 text-primary-600 focus:ring-primary-500"
                  {...form.register('priority', { valueAsNumber: true })}
                />
                <span className="text-sm text-gray-700">{t('walkins.priorityNormal')}</span>
              </label>
              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  value={2}
                  className="h-4 w-4 border-gray-300 text-red-600 focus:ring-red-500"
                  {...form.register('priority', { valueAsNumber: true })}
                />
                <span className="text-sm text-gray-700">{t('walkins.priorityUrgent')}</span>
              </label>
            </div>
          </div>

          <div className="w-full">
            <label
              htmlFor="notes"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('walkins.notes')}
            </label>
            <textarea
              id="notes"
              rows={2}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('notes')}
            />
          </div>

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="outline" onClick={onClose}>
              {t('common.cancel')}
            </Button>
            <Button type="submit" disabled={checkInMutation.isPending}>
              {t('walkins.checkIn')}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
