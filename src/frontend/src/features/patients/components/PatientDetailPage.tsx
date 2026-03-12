import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate, useParams } from '@tanstack/react-router'
import { User, ArrowLeft, Pencil } from 'lucide-react'
import { PageHeader } from '@/shared/components/PageHeader'
import { Button } from '@/shared/components/Button'
import { Card } from '@/shared/components/Card'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { usePatient } from '../hooks/usePatients'
import { PatientFormModal } from './PatientFormModal'

function calculateAge(dateOfBirth: string | null): number | null {
  if (!dateOfBirth) return null
  const dob = new Date(dateOfBirth)
  const today = new Date()
  let age = today.getFullYear() - dob.getFullYear()
  const monthDiff = today.getMonth() - dob.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
    age--
  }
  return age
}

function formatGender(gender: string | null, t: (key: string) => string): string {
  if (!gender) return '-'
  if (gender === 'male') return t('patients.genderMale')
  if (gender === 'female') return t('patients.genderFemale')
  if (gender === 'other') return t('patients.genderOther')
  return gender
}

export function PatientDetailPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { patientId } = useParams({ strict: false }) as { patientId: string }
  const { data: patient, isLoading } = usePatient(patientId)
  const [modalOpen, setModalOpen] = useState(false)

  const handleBack = () => {
    void navigate({ to: '/patients' })
  }

  const handleEdit = () => {
    setModalOpen(true)
  }

  const handleCloseModal = () => {
    setModalOpen(false)
  }

  if (isLoading) {
    return (
      <div>
        <PageHeader title={t('patients.detail')} />
        <div className="flex items-center justify-center py-12">
          <LoadingSpinner size="lg" />
        </div>
      </div>
    )
  }

  if (!patient) {
    return (
      <div>
        <PageHeader title={t('patients.detail')} />
        <EmptyState
          icon={<User className="h-12 w-12" />}
          title={t('patients.detailEmpty')}
          action={
            <Button variant="outline" onClick={handleBack}>
              <ArrowLeft className="mr-1 h-4 w-4" />
              {t('common.back')}
            </Button>
          }
        />
      </div>
    )
  }

  const age = calculateAge(patient.dateOfBirth)

  return (
    <div>
      <PageHeader
        title={`${patient.firstName} ${patient.lastName}`}
        actions={
          <div className="flex items-center gap-2">
            <Button variant="outline" onClick={handleBack}>
              <ArrowLeft className="mr-1 h-4 w-4" />
              {t('common.back')}
            </Button>
            <Button onClick={handleEdit}>
              <Pencil className="mr-1 h-4 w-4" />
              {t('patients.edit')}
            </Button>
          </div>
        }
      />

      <div className="mb-4">
        <Badge variant={patient.isActive ? 'success' : 'neutral'}>
          {patient.isActive ? t('patients.active') : t('patients.inactive')}
        </Badge>
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        {/* Personal Information */}
        <Card title={t('patients.personalInfo')}>
          <dl className="space-y-3">
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.firstName')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">{patient.firstName}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.lastName')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">{patient.lastName}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.dateOfBirth')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">
                {patient.dateOfBirth ?? '-'}
                {age !== null && (
                  <span className="ml-2 text-gray-500">({age} {t('patients.age')})</span>
                )}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.gender')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">{formatGender(patient.gender, t)}</dd>
            </div>
          </dl>
        </Card>

        {/* Contact Information */}
        <Card title={t('patients.contactInfo')}>
          <dl className="space-y-3">
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.email')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">
                {patient.email ?? t('patients.noEmail')}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.phone')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">
                {patient.phone ?? t('patients.noPhone')}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.address')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">{patient.address ?? '-'}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.city')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">{patient.city ?? '-'}</dd>
            </div>
          </dl>
        </Card>

        {/* Emergency Contact */}
        <Card title={t('patients.emergencyContact')}>
          <dl className="space-y-3">
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.emergencyContact')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">
                {patient.emergencyContactName ?? '-'}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.emergencyPhone')}</dt>
              <dd className="mt-0.5 text-sm text-gray-900">
                {patient.emergencyContactPhone ?? '-'}
              </dd>
            </div>
          </dl>
        </Card>

        {/* Medical Information */}
        <Card title={t('patients.medicalHistory')}>
          <dl className="space-y-3">
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.medicalHistory')}</dt>
              <dd className="mt-0.5 whitespace-pre-wrap text-sm text-gray-900">
                {patient.medicalHistory ?? '-'}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.allergies')}</dt>
              <dd className="mt-0.5 whitespace-pre-wrap text-sm text-gray-900">
                {patient.allergies ?? '-'}
              </dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">{t('patients.notes')}</dt>
              <dd className="mt-0.5 whitespace-pre-wrap text-sm text-gray-900">
                {patient.notes ?? '-'}
              </dd>
            </div>
          </dl>
        </Card>
      </div>

      {modalOpen && (
        <PatientFormModal
          patient={patient}
          onClose={handleCloseModal}
        />
      )}
    </div>
  )
}
