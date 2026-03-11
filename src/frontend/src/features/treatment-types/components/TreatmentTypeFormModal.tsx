import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { Input } from '@/shared/components/Input'
import { Button } from '@/shared/components/Button'
import type { TreatmentType, CreateTreatmentTypeRequest } from '../types'

interface TreatmentTypeFormData {
  name: string
  description: string
  durationMinutes: number
  price: number
  color: string
  isActive: boolean
}

interface TreatmentTypeFormModalProps {
  isOpen: boolean
  onClose: () => void
  onSubmit: (data: CreateTreatmentTypeRequest & { isActive?: boolean }) => void
  treatmentType?: TreatmentType | null
  isSubmitting?: boolean
}

export function TreatmentTypeFormModal({
  isOpen,
  onClose,
  onSubmit,
  treatmentType,
  isSubmitting = false,
}: TreatmentTypeFormModalProps) {
  const { t } = useTranslation()
  const isEdit = !!treatmentType

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<TreatmentTypeFormData>({
    defaultValues: {
      name: '',
      description: '',
      durationMinutes: 30,
      price: 0,
      color: '#3B82F6',
      isActive: true,
    },
  })

  useEffect(() => {
    if (treatmentType) {
      reset({
        name: treatmentType.name,
        description: treatmentType.description ?? '',
        durationMinutes: treatmentType.durationMinutes,
        price: treatmentType.price,
        color: treatmentType.color ?? '#3B82F6',
        isActive: treatmentType.isActive,
      })
    } else {
      reset({
        name: '',
        description: '',
        durationMinutes: 30,
        price: 0,
        color: '#3B82F6',
        isActive: true,
      })
    }
  }, [treatmentType, reset])

  const handleFormSubmit = (data: TreatmentTypeFormData) => {
    onSubmit({
      name: data.name,
      description: data.description || undefined,
      durationMinutes: Number(data.durationMinutes),
      price: Number(data.price),
      color: data.color || undefined,
      ...(isEdit ? { isActive: data.isActive } : {}),
    })
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div
        className="fixed inset-0 bg-black/50"
        onClick={onClose}
        onKeyDown={(e) => {
          if (e.key === 'Escape') onClose()
        }}
        role="button"
        tabIndex={0}
        aria-label="Close modal"
      />
      <div className="relative z-10 w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
        <h2 className="mb-4 text-lg font-semibold text-gray-900">
          {isEdit ? t('treatmentTypes.edit') : t('treatmentTypes.add')}
        </h2>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
          <Input
            label={t('treatmentTypes.name')}
            error={errors.name?.message}
            {...register('name', {
              required: 'Name is required',
              maxLength: { value: 255, message: 'Name must not exceed 255 characters' },
            })}
          />

          <div className="w-full">
            <label
              htmlFor="description"
              className="mb-1 block text-sm font-medium text-gray-700"
            >
              {t('treatmentTypes.description')}
            </label>
            <textarea
              id="description"
              rows={3}
              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors placeholder:text-gray-400 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              {...register('description')}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <Input
              label={t('treatmentTypes.duration')}
              type="number"
              error={errors.durationMinutes?.message}
              {...register('durationMinutes', {
                required: 'Duration is required',
                valueAsNumber: true,
                min: { value: 1, message: 'Duration must be at least 1 minute' },
                max: { value: 480, message: 'Duration must not exceed 480 minutes' },
              })}
            />

            <Input
              label={t('treatmentTypes.price')}
              type="number"
              step="0.01"
              error={errors.price?.message}
              {...register('price', {
                required: 'Price is required',
                valueAsNumber: true,
                min: { value: 0, message: 'Price must be zero or positive' },
              })}
            />
          </div>

          <Input
            label={t('treatmentTypes.color')}
            type="color"
            className="h-10 cursor-pointer"
            error={errors.color?.message}
            {...register('color', {
              pattern: {
                value: /^#[0-9A-Fa-f]{6}$/,
                message: 'Must be a valid hex color',
              },
            })}
          />

          {isEdit && (
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isActive"
                className="h-4 w-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                {...register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                {t('treatmentTypes.active')}
              </label>
            </div>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="outline" onClick={onClose}>
              {t('common.cancel')}
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? t('common.loading') : t('common.save')}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
