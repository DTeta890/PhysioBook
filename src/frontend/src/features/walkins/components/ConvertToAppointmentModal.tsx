import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod/v4'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { X } from 'lucide-react'
import { Button } from '@/shared/components/Button'
import { Input } from '@/shared/components/Input'
import { useConvertToAppointment } from '../hooks/useWalkIns'
import { useTherapists } from '@/features/therapists/hooks/useTherapists'
import { useTreatmentTypes } from '@/features/treatment-types/hooks/useTreatmentTypes'
import { useToastStore } from '@/shared/stores/useToastStore'
import type { WalkInEntry } from '../types'

const convertSchema = z.object({
  therapistId: z.string().min(1, 'Therapist is required'),
  treatmentTypeId: z.string().min(1, 'Treatment type is required'),
  startTime: z.string().min(1, 'Start time is required'),
  endTime: z.string().min(1, 'End time is required'),
  notes: z.string().max(1000).optional().or(z.literal('')),
})

type ConvertFormData = z.infer<typeof convertSchema>

interface ConvertToAppointmentModalProps {
  walkIn: WalkInEntry
  onClose: () => void
}

export function ConvertToAppointmentModal({ walkIn, onClose }: ConvertToAppointmentModalProps) {
  const { t } = useTranslation()
  const convertMutation = useConvertToAppointment()
  const { data: therapists } = useTherapists()
  const { data: treatmentTypes } = useTreatmentTypes()
  const toast = useToastStore()

  const form = useForm<ConvertFormData>({
    resolver: zodResolver(convertSchema),
    defaultValues: {
      therapistId: walkIn.assignedTherapistId ?? '',
      treatmentTypeId: walkIn.treatmentTypeId ?? '',
      startTime: '',
      endTime: '',
      notes: walkIn.notes ?? '',
    },
  })

  const selectedTreatmentTypeId = form.watch('treatmentTypeId')
  const startTime = form.watch('startTime')

  useEffect(() => {
    if (!startTime || !selectedTreatmentTypeId || !treatmentTypes) return
    const selectedType = treatmentTypes.find((tt) => tt.id === selectedTreatmentTypeId)
    if (!selectedType) return

    const start = new Date(startTime)
    if (isNaN(start.getTime())) return

    const end = new Date(start.getTime() + selectedType.durationMinutes * 60 * 1000)
    const pad = (n: number) => String(n).padStart(2, '0')
    const endStr = `${end.getFullYear()}-${pad(end.getMonth() + 1)}-${pad(end.getDate())}T${pad(end.getHours())}:${pad(end.getMinutes())}`
    form.setValue('endTime', endStr)
  }, [startTime, selectedTreatmentTypeId, treatmentTypes, form])

  const handleSubmit = (data: ConvertFormData) => {
    convertMutation.mutate(
      {
        id: walkIn.id,
        data: {
          therapistId: data.therapistId,
          treatmentTypeId: data.treatmentTypeId,
          startTime: new Date(data.startTime).toISOString(),
          endTime: new Date(data.endTime).toISOString(),
          notes: data.notes || undefined,
        },
      },
      {
        onSuccess: () => {
          toast.show(t('walkins.converted'), 'success')
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
            {t('walkins.convertToAppointment')}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
          <div className="w-full">
            <label
              htmlFor="therapistId"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('walkins.selectTherapist')}
            </label>
            <select
              id="therapistId"
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('therapistId')}
            >
              <option value="">--</option>
              {therapists
                ?.filter((th) => th.isActive)
                .map((th) => (
                  <option key={th.id} value={th.id}>
                    {th.firstName} {th.lastName}
                  </option>
                ))}
            </select>
            {form.formState.errors.therapistId?.message && (
              <p className="mt-1 text-sm text-red-600">
                {form.formState.errors.therapistId.message}
              </p>
            )}
          </div>

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
              <option value="">--</option>
              {treatmentTypes
                ?.filter((tt) => tt.isActive)
                .map((tt) => (
                  <option key={tt.id} value={tt.id}>
                    {tt.name} ({tt.durationMinutes} {t('treatmentTypes.minutes')})
                  </option>
                ))}
            </select>
            {form.formState.errors.treatmentTypeId?.message && (
              <p className="mt-1 text-sm text-red-600">
                {form.formState.errors.treatmentTypeId.message}
              </p>
            )}
          </div>

          <Input
            label={t('walkins.startTime')}
            type="datetime-local"
            error={form.formState.errors.startTime?.message}
            {...form.register('startTime')}
          />

          <Input
            label={t('walkins.endTime')}
            type="datetime-local"
            error={form.formState.errors.endTime?.message}
            {...form.register('endTime')}
          />

          <div className="w-full">
            <label
              htmlFor="convertNotes"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('walkins.notes')}
            </label>
            <textarea
              id="convertNotes"
              rows={2}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('notes')}
            />
          </div>

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="outline" onClick={onClose}>
              {t('common.cancel')}
            </Button>
            <Button type="submit" disabled={convertMutation.isPending}>
              {t('walkins.convertToAppointment')}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
