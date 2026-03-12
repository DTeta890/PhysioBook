import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod/v4'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { X } from 'lucide-react'
import { Button } from '@/shared/components/Button'
import { Input } from '@/shared/components/Input'
import { useCreatePatient, useUpdatePatient } from '../hooks/usePatients'
import type { Patient } from '../types'

const createSchema = z.object({
  firstName: z.string().min(1, 'First name is required').max(100),
  lastName: z.string().min(1, 'Last name is required').max(100),
  email: z.email('Invalid email address').optional().or(z.literal('')),
  phone: z.string().max(20).optional().or(z.literal('')),
  dateOfBirth: z.string().optional().or(z.literal('')),
  gender: z.string().optional().or(z.literal('')),
  address: z.string().max(200).optional().or(z.literal('')),
  city: z.string().max(100).optional().or(z.literal('')),
  emergencyContactName: z.string().max(200).optional().or(z.literal('')),
  emergencyContactPhone: z.string().max(20).optional().or(z.literal('')),
  medicalHistory: z.string().optional().or(z.literal('')),
  allergies: z.string().optional().or(z.literal('')),
  notes: z.string().optional().or(z.literal('')),
})

const updateSchema = z.object({
  firstName: z.string().min(1, 'First name is required').max(100),
  lastName: z.string().min(1, 'Last name is required').max(100),
  email: z.email('Invalid email address').optional().or(z.literal('')),
  phone: z.string().max(20).optional().or(z.literal('')),
  dateOfBirth: z.string().optional().or(z.literal('')),
  gender: z.string().optional().or(z.literal('')),
  address: z.string().max(200).optional().or(z.literal('')),
  city: z.string().max(100).optional().or(z.literal('')),
  emergencyContactName: z.string().max(200).optional().or(z.literal('')),
  emergencyContactPhone: z.string().max(20).optional().or(z.literal('')),
  medicalHistory: z.string().optional().or(z.literal('')),
  allergies: z.string().optional().or(z.literal('')),
  notes: z.string().optional().or(z.literal('')),
  isActive: z.boolean(),
})

type CreateFormData = z.infer<typeof createSchema>
type UpdateFormData = z.infer<typeof updateSchema>

interface PatientFormModalProps {
  patient: Patient | null
  onClose: () => void
}

function cleanOptional(val: string | undefined): string | undefined {
  return val || undefined
}

const selectClassName = 'block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'
const textareaClassName = 'block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm transition-colors placeholder:text-gray-400 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'

export function PatientFormModal({ patient, onClose }: PatientFormModalProps) {
  const { t } = useTranslation()
  const isEditing = !!patient
  const createMutation = useCreatePatient()
  const updateMutation = useUpdatePatient()

  const createForm = useForm<CreateFormData>({
    resolver: zodResolver(createSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      gender: '',
      address: '',
      city: '',
      emergencyContactName: '',
      emergencyContactPhone: '',
      medicalHistory: '',
      allergies: '',
      notes: '',
    },
  })

  const updateForm = useForm<UpdateFormData>({
    resolver: zodResolver(updateSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      gender: '',
      address: '',
      city: '',
      emergencyContactName: '',
      emergencyContactPhone: '',
      medicalHistory: '',
      allergies: '',
      notes: '',
      isActive: true,
    },
  })

  useEffect(() => {
    if (patient) {
      updateForm.reset({
        firstName: patient.firstName,
        lastName: patient.lastName,
        email: patient.email ?? '',
        phone: patient.phone ?? '',
        dateOfBirth: patient.dateOfBirth ?? '',
        gender: patient.gender ?? '',
        address: patient.address ?? '',
        city: patient.city ?? '',
        emergencyContactName: patient.emergencyContactName ?? '',
        emergencyContactPhone: patient.emergencyContactPhone ?? '',
        medicalHistory: patient.medicalHistory ?? '',
        allergies: patient.allergies ?? '',
        notes: patient.notes ?? '',
        isActive: patient.isActive,
      })
    }
  }, [patient, updateForm])

  const handleCreate = (data: CreateFormData) => {
    createMutation.mutate(
      {
        firstName: data.firstName,
        lastName: data.lastName,
        email: cleanOptional(data.email),
        phone: cleanOptional(data.phone),
        dateOfBirth: cleanOptional(data.dateOfBirth),
        gender: cleanOptional(data.gender),
        address: cleanOptional(data.address),
        city: cleanOptional(data.city),
        emergencyContactName: cleanOptional(data.emergencyContactName),
        emergencyContactPhone: cleanOptional(data.emergencyContactPhone),
        medicalHistory: cleanOptional(data.medicalHistory),
        allergies: cleanOptional(data.allergies),
        notes: cleanOptional(data.notes),
      },
      { onSuccess: () => onClose() },
    )
  }

  const handleUpdate = (data: UpdateFormData) => {
    if (!patient) return
    updateMutation.mutate(
      {
        id: patient.id,
        data: {
          firstName: data.firstName,
          lastName: data.lastName,
          email: cleanOptional(data.email),
          phone: cleanOptional(data.phone),
          dateOfBirth: cleanOptional(data.dateOfBirth),
          gender: cleanOptional(data.gender),
          address: cleanOptional(data.address),
          city: cleanOptional(data.city),
          emergencyContactName: cleanOptional(data.emergencyContactName),
          emergencyContactPhone: cleanOptional(data.emergencyContactPhone),
          medicalHistory: cleanOptional(data.medicalHistory),
          allergies: cleanOptional(data.allergies),
          notes: cleanOptional(data.notes),
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
      <div className="relative z-10 max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-xl bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900">
            {isEditing ? t('patients.edit') : t('patients.add')}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X className="h-5 w-5" />
          </button>
        </div>

        {isEditing ? (
          <form onSubmit={updateForm.handleSubmit(handleUpdate)} className="space-y-6">
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.personalInfo')}</h3>
              <div className="space-y-4">
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.firstName')} error={updateForm.formState.errors.firstName?.message} {...updateForm.register('firstName')} />
                  <Input label={t('patients.lastName')} error={updateForm.formState.errors.lastName?.message} {...updateForm.register('lastName')} />
                </div>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.dateOfBirth')} type="date" error={updateForm.formState.errors.dateOfBirth?.message} {...updateForm.register('dateOfBirth')} />
                  <div className="w-full">
                    <label htmlFor="gender-edit" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.gender')}</label>
                    <select id="gender-edit" className={selectClassName} {...updateForm.register('gender')}>
                      <option value="">{'-'}</option>
                      <option value="male">{t('patients.genderMale')}</option>
                      <option value="female">{t('patients.genderFemale')}</option>
                      <option value="other">{t('patients.genderOther')}</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.contactInfo')}</h3>
              <div className="space-y-4">
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.email')} type="email" error={updateForm.formState.errors.email?.message} {...updateForm.register('email')} />
                  <Input label={t('patients.phone')} error={updateForm.formState.errors.phone?.message} {...updateForm.register('phone')} />
                </div>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.address')} error={updateForm.formState.errors.address?.message} {...updateForm.register('address')} />
                  <Input label={t('patients.city')} error={updateForm.formState.errors.city?.message} {...updateForm.register('city')} />
                </div>
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.emergencyContact')}</h3>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Input label={t('patients.emergencyContact')} error={updateForm.formState.errors.emergencyContactName?.message} {...updateForm.register('emergencyContactName')} />
                <Input label={t('patients.emergencyPhone')} error={updateForm.formState.errors.emergencyContactPhone?.message} {...updateForm.register('emergencyContactPhone')} />
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.medicalHistory')}</h3>
              <div className="space-y-4">
                <div className="w-full">
                  <label htmlFor="medicalHistory-edit" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.medicalHistory')}</label>
                  <textarea id="medicalHistory-edit" rows={3} className={textareaClassName} {...updateForm.register('medicalHistory')} />
                </div>
                <div className="w-full">
                  <label htmlFor="allergies-edit" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.allergies')}</label>
                  <textarea id="allergies-edit" rows={2} className={textareaClassName} {...updateForm.register('allergies')} />
                </div>
                <div className="w-full">
                  <label htmlFor="notes-edit" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.notes')}</label>
                  <textarea id="notes-edit" rows={2} className={textareaClassName} {...updateForm.register('notes')} />
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isActive"
                className="h-4 w-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                {...updateForm.register('isActive')}
              />
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                {t('patients.active')}
              </label>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <Button type="button" variant="outline" onClick={onClose}>{t('common.cancel')}</Button>
              <Button type="submit" disabled={isPending}>{t('common.save')}</Button>
            </div>
          </form>
        ) : (
          <form onSubmit={createForm.handleSubmit(handleCreate)} className="space-y-6">
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.personalInfo')}</h3>
              <div className="space-y-4">
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.firstName')} error={createForm.formState.errors.firstName?.message} {...createForm.register('firstName')} />
                  <Input label={t('patients.lastName')} error={createForm.formState.errors.lastName?.message} {...createForm.register('lastName')} />
                </div>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.dateOfBirth')} type="date" error={createForm.formState.errors.dateOfBirth?.message} {...createForm.register('dateOfBirth')} />
                  <div className="w-full">
                    <label htmlFor="gender-create" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.gender')}</label>
                    <select id="gender-create" className={selectClassName} {...createForm.register('gender')}>
                      <option value="">{'-'}</option>
                      <option value="male">{t('patients.genderMale')}</option>
                      <option value="female">{t('patients.genderFemale')}</option>
                      <option value="other">{t('patients.genderOther')}</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.contactInfo')}</h3>
              <div className="space-y-4">
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.email')} type="email" error={createForm.formState.errors.email?.message} {...createForm.register('email')} />
                  <Input label={t('patients.phone')} error={createForm.formState.errors.phone?.message} {...createForm.register('phone')} />
                </div>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input label={t('patients.address')} error={createForm.formState.errors.address?.message} {...createForm.register('address')} />
                  <Input label={t('patients.city')} error={createForm.formState.errors.city?.message} {...createForm.register('city')} />
                </div>
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.emergencyContact')}</h3>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Input label={t('patients.emergencyContact')} error={createForm.formState.errors.emergencyContactName?.message} {...createForm.register('emergencyContactName')} />
                <Input label={t('patients.emergencyPhone')} error={createForm.formState.errors.emergencyContactPhone?.message} {...createForm.register('emergencyContactPhone')} />
              </div>
            </div>
            <div>
              <h3 className="mb-3 text-sm font-semibold text-gray-700">{t('patients.medicalHistory')}</h3>
              <div className="space-y-4">
                <div className="w-full">
                  <label htmlFor="medicalHistory-create" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.medicalHistory')}</label>
                  <textarea id="medicalHistory-create" rows={3} className={textareaClassName} {...createForm.register('medicalHistory')} />
                </div>
                <div className="w-full">
                  <label htmlFor="allergies-create" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.allergies')}</label>
                  <textarea id="allergies-create" rows={2} className={textareaClassName} {...createForm.register('allergies')} />
                </div>
                <div className="w-full">
                  <label htmlFor="notes-create" className="mb-1 block text-sm font-medium text-gray-700">{t('patients.notes')}</label>
                  <textarea id="notes-create" rows={2} className={textareaClassName} {...createForm.register('notes')} />
                </div>
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <Button type="button" variant="outline" onClick={onClose}>{t('common.cancel')}</Button>
              <Button type="submit" disabled={isPending}>{t('common.save')}</Button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}
