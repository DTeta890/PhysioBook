import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod/v4'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { X } from 'lucide-react'
import { Button } from '@/shared/components/Button'
import { Input } from '@/shared/components/Input'
import { useCreateTherapist, useUpdateTherapist } from '../hooks/useTherapists'
import type { Therapist } from '../types'

const createSchema = z.object({
  email: z.email('Invalid email address'),
  firstName: z.string().min(1, 'First name is required').max(100),
  lastName: z.string().min(1, 'Last name is required').max(100),
  password: z
    .string()
    .min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Must contain uppercase letter')
    .regex(/[a-z]/, 'Must contain lowercase letter')
    .regex(/[0-9]/, 'Must contain a number'),
  phone: z.string().max(20).optional().or(z.literal('')),
  specialization: z.string().max(200).optional().or(z.literal('')),
  color: z
    .string()
    .regex(/^#[0-9A-Fa-f]{6}$/, 'Must be a valid hex color')
    .optional()
    .or(z.literal('')),
})

const updateSchema = z.object({
  firstName: z.string().min(1, 'First name is required').max(100),
  lastName: z.string().min(1, 'Last name is required').max(100),
  phone: z.string().max(20).optional().or(z.literal('')),
  specialization: z.string().max(200).optional().or(z.literal('')),
  color: z
    .string()
    .regex(/^#[0-9A-Fa-f]{6}$/, 'Must be a valid hex color')
    .optional()
    .or(z.literal('')),
  isActive: z.boolean(),
})

type CreateFormData = z.infer<typeof createSchema>
type UpdateFormData = z.infer<typeof updateSchema>

interface TherapistFormModalProps {
  therapist: Therapist | null
  onClose: () => void
}

export function TherapistFormModal({ therapist, onClose }: TherapistFormModalProps) {
  const { t } = useTranslation()
  const isEditing = !!therapist
  const createMutation = useCreateTherapist()
  const updateMutation = useUpdateTherapist()

  const createForm = useForm<CreateFormData>({
    resolver: zodResolver(createSchema),
    defaultValues: {
      email: '',
      firstName: '',
      lastName: '',
      password: '',
      phone: '',
      specialization: '',
      color: '',
    },
  })

  const updateForm = useForm<UpdateFormData>({
    resolver: zodResolver(updateSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      phone: '',
      specialization: '',
      color: '',
      isActive: true,
    },
  })

  useEffect(() => {
    if (therapist) {
      updateForm.reset({
        firstName: therapist.firstName,
        lastName: therapist.lastName,
        phone: therapist.phone ?? '',
        specialization: therapist.specialization ?? '',
        color: therapist.color ?? '',
        isActive: therapist.isActive,
      })
    }
  }, [therapist, updateForm])

  const handleCreate = (data: CreateFormData) => {
    createMutation.mutate(
      {
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName,
        password: data.password,
        phone: data.phone || undefined,
        specialization: data.specialization || undefined,
        color: data.color || undefined,
      },
      { onSuccess: () => onClose() },
    )
  }

  const handleUpdate = (data: UpdateFormData) => {
    if (!therapist) return
    updateMutation.mutate(
      {
        id: therapist.id,
        data: {
          firstName: data.firstName,
          lastName: data.lastName,
          phone: data.phone || undefined,
          specialization: data.specialization || undefined,
          color: data.color || undefined,
          isActive: data.isActive,
        },
      },
      { onSuccess: () => onClose() },
    )
  }

  const isPending = createMutation.isPending || updateMutation.isPending

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="fixed inset-0 bg-black/50" onClick={onClose} />
      <div className="relative z-10 w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">
            {isEditing ? t('therapists.edit') : t('therapists.add')}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        {isEditing ? (
          <form onSubmit={updateForm.handleSubmit(handleUpdate)} className="space-y-4">
            <Input
              label={t('therapists.firstName')}
              error={updateForm.formState.errors.firstName?.message}
              {...updateForm.register('firstName')}
            />
            <Input
              label={t('therapists.lastName')}
              error={updateForm.formState.errors.lastName?.message}
              {...updateForm.register('lastName')}
            />
            <Input
              label={t('therapists.phone')}
              error={updateForm.formState.errors.phone?.message}
              {...updateForm.register('phone')}
            />
            <Input
              label={t('therapists.specialization')}
              error={updateForm.formState.errors.specialization?.message}
              {...updateForm.register('specialization')}
            />
            <Input
              label={t('therapists.color')}
              placeholder="#FF5733"
              error={updateForm.formState.errors.color?.message}
              {...updateForm.register('color')}
            />
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isActive"
                className="h-4 w-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                {...updateForm.register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                {t('therapists.active')}
              </label>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <Button type="button" variant="outline" onClick={onClose}>
                {t('common.cancel')}
              </Button>
              <Button type="submit" disabled={isPending}>
                {t('common.save')}
              </Button>
            </div>
          </form>
        ) : (
          <form onSubmit={createForm.handleSubmit(handleCreate)} className="space-y-4">
            <Input
              label={t('therapists.email')}
              type="email"
              error={createForm.formState.errors.email?.message}
              {...createForm.register('email')}
            />
            <Input
              label={t('therapists.password')}
              type="password"
              error={createForm.formState.errors.password?.message}
              {...createForm.register('password')}
            />
            <Input
              label={t('therapists.firstName')}
              error={createForm.formState.errors.firstName?.message}
              {...createForm.register('firstName')}
            />
            <Input
              label={t('therapists.lastName')}
              error={createForm.formState.errors.lastName?.message}
              {...createForm.register('lastName')}
            />
            <Input
              label={t('therapists.phone')}
              error={createForm.formState.errors.phone?.message}
              {...createForm.register('phone')}
            />
            <Input
              label={t('therapists.specialization')}
              error={createForm.formState.errors.specialization?.message}
              {...createForm.register('specialization')}
            />
            <Input
              label={t('therapists.color')}
              placeholder="#FF5733"
              error={createForm.formState.errors.color?.message}
              {...createForm.register('color')}
            />
            <div className="flex justify-end gap-3 pt-2">
              <Button type="button" variant="outline" onClick={onClose}>
                {t('common.cancel')}
              </Button>
              <Button type="submit" disabled={isPending}>
                {t('common.save')}
              </Button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}
