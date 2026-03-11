import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod/v4'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { X } from 'lucide-react'
import { Button } from '@/shared/components/Button'
import { Input } from '@/shared/components/Input'
import { useTreatmentTypes } from '@/features/treatment-types/hooks/useTreatmentTypes'
import { useCreateAppointment } from '../hooks/useCreateAppointment'
import { useToastStore } from '@/shared/stores/useToastStore'
import { buildDateTimeFromSlot, calculateEndTime } from '../utils/calendar-utils'
import type { TimeSlotData } from '../types'

const appointmentSchema = z.object({
  patientName: z.string().min(1, 'Patient name is required'),
  patientPhone: z.string().max(20).optional().or(z.literal('')),
  treatmentTypeId: z.string().min(1, 'Treatment type is required'),
  durationMinutes: z.number().min(15).max(480),
  notes: z.string().max(1000).optional().or(z.literal('')),
  color: z
    .string()
    .regex(/^#[0-9A-Fa-f]{6}$/, 'Must be a valid hex color')
    .optional()
    .or(z.literal('')),
})

type AppointmentFormData = z.infer<typeof appointmentSchema>

interface AppointmentFormModalProps {
  slotData: TimeSlotData
  onClose: () => void
  onSuccess: () => void
}

export function AppointmentFormModal({ slotData, onClose, onSuccess }: AppointmentFormModalProps) {
  const { t } = useTranslation()
  const { data: treatmentTypes } = useTreatmentTypes()
  const createMutation = useCreateAppointment()
  const showToast = useToastStore((s) => s.show)

  const form = useForm<AppointmentFormData>({
    resolver: zodResolver(appointmentSchema),
    defaultValues: {
      patientName: '',
      patientPhone: '',
      treatmentTypeId: '',
      durationMinutes: 30,
      notes: '',
      color: '',
    },
  })

  const selectedTreatmentTypeId = form.watch('treatmentTypeId')

  // Auto-fill duration and color when treatment type changes
  useEffect(() => {
    if (!selectedTreatmentTypeId || !treatmentTypes) return
    const selected = treatmentTypes.find((tt) => tt.id === selectedTreatmentTypeId)
    if (selected) {
      form.setValue('durationMinutes', selected.durationMinutes)
      if (selected.color) {
        form.setValue('color', selected.color)
      }
    }
  }, [selectedTreatmentTypeId, treatmentTypes, form])

  const handleSubmit = (data: AppointmentFormData) => {
    const startTime = buildDateTimeFromSlot(slotData.dayDate, slotData.timeSlot)
    const endTime = calculateEndTime(startTime, data.durationMinutes)

    createMutation.mutate(
      {
        therapistId: slotData.therapistId,
        patientName: data.patientName,
        patientPhone: data.patientPhone || undefined,
        treatmentTypeId: data.treatmentTypeId,
        startTime,
        endTime,
        notes: data.notes || undefined,
        color: data.color || undefined,
      },
      {
        onSuccess: () => {
          showToast(t('calendar.appointmentCreated'), 'success')
          onSuccess()
        },
      },
    )
  }

  const activeTreatmentTypes = treatmentTypes?.filter((tt) => tt.isActive) ?? []

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="fixed inset-0 bg-black/50" onClick={onClose} />
      <div className="relative z-10 w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">
            {t('calendar.quickCreate')}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="mb-4 rounded-lg bg-gray-50 px-3 py-2 text-sm text-gray-600">
          {slotData.dayDate} &middot; {slotData.timeSlot}
        </div>

        <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
          <Input
            label={t('calendar.patientName')}
            error={form.formState.errors.patientName?.message}
            {...form.register('patientName')}
          />

          <Input
            label={t('calendar.patientPhone')}
            error={form.formState.errors.patientPhone?.message}
            {...form.register('patientPhone')}
          />

          {/* Treatment Type select */}
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
              <option value="">{t('calendar.treatmentType')}...</option>
              {activeTreatmentTypes.map((tt) => (
                <option key={tt.id} value={tt.id}>
                  {tt.name} ({tt.durationMinutes} min)
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
            label={t('calendar.duration')}
            type="number"
            min={15}
            max={480}
            error={form.formState.errors.durationMinutes?.message}
            {...form.register('durationMinutes', { valueAsNumber: true })}
          />

          {/* Notes textarea */}
          <div className="w-full">
            <label
              htmlFor="notes"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('calendar.notes')}
            </label>
            <textarea
              id="notes"
              rows={3}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors placeholder:text-gray-400 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...form.register('notes')}
            />
            {form.formState.errors.notes?.message && (
              <p className="mt-1 text-sm text-red-600">
                {form.formState.errors.notes.message}
              </p>
            )}
          </div>

          <Input
            label={t('calendar.color')}
            placeholder="#22c55e"
            error={form.formState.errors.color?.message}
            {...form.register('color')}
          />

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="outline" onClick={onClose}>
              {t('common.cancel')}
            </Button>
            <Button type="submit" disabled={createMutation.isPending}>
              {createMutation.isPending ? t('common.loading') : t('common.save')}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
