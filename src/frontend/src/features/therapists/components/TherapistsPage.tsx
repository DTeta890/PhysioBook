import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { UserCheck, Pencil, Trash2 } from 'lucide-react'
import { PageHeader } from '@/shared/components/PageHeader'
import { Button } from '@/shared/components/Button'
import { Card } from '@/shared/components/Card'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { useTherapists, useDeleteTherapist } from '../hooks/useTherapists'
import { TherapistFormModal } from './TherapistFormModal'
import type { Therapist } from '../types'

export function TherapistsPage() {
  const { t } = useTranslation()
  const { data: therapists, isLoading } = useTherapists()
  const deleteMutation = useDeleteTherapist()
  const [modalOpen, setModalOpen] = useState(false)
  const [editingTherapist, setEditingTherapist] = useState<Therapist | null>(null)

  const handleAdd = () => {
    setEditingTherapist(null)
    setModalOpen(true)
  }

  const handleEdit = (therapist: Therapist) => {
    setEditingTherapist(therapist)
    setModalOpen(true)
  }

  const handleDelete = (therapist: Therapist) => {
    if (window.confirm(t('therapists.deleteConfirm'))) {
      deleteMutation.mutate(therapist.id)
    }
  }

  const handleCloseModal = () => {
    setModalOpen(false)
    setEditingTherapist(null)
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <LoadingSpinner size="lg" />
      </div>
    )
  }

  return (
    <div>
      <PageHeader
        title={t('therapists.title')}
        actions={
          <Button onClick={handleAdd}>
            {t('therapists.add')}
          </Button>
        }
      />

      {!therapists || therapists.length === 0 ? (
        <EmptyState
          icon={<UserCheck className="h-12 w-12" />}
          title={t('therapists.empty')}
          description={t('therapists.emptyDescription')}
          action={
            <Button onClick={handleAdd}>
              {t('therapists.add')}
            </Button>
          }
        />
      ) : (
        <Card padding={false}>
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="border-b border-gray-200 bg-gray-50">
                <tr>
                  <th className="px-6 py-3 font-medium text-gray-500">
                    {t('therapists.firstName')} / {t('therapists.lastName')}
                  </th>
                  <th className="px-6 py-3 font-medium text-gray-500">
                    {t('therapists.email')}
                  </th>
                  <th className="px-6 py-3 font-medium text-gray-500">
                    {t('therapists.specialization')}
                  </th>
                  <th className="px-6 py-3 font-medium text-gray-500">
                    {t('therapists.color')}
                  </th>
                  <th className="px-6 py-3 font-medium text-gray-500">
                    Status
                  </th>
                  <th className="px-6 py-3 font-medium text-gray-500" />
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {therapists.map((therapist) => (
                  <tr key={therapist.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 font-medium text-gray-900">
                      {therapist.firstName} {therapist.lastName}
                    </td>
                    <td className="px-6 py-4 text-gray-600">
                      {therapist.email}
                    </td>
                    <td className="px-6 py-4 text-gray-600">
                      {therapist.specialization ?? '-'}
                    </td>
                    <td className="px-6 py-4">
                      {therapist.color ? (
                        <div className="flex items-center gap-2">
                          <span
                            className="inline-block h-5 w-5 rounded-full border border-gray-200"
                            style={{ backgroundColor: therapist.color }}
                          />
                          <span className="text-gray-500 text-xs">{therapist.color}</span>
                        </div>
                      ) : (
                        <span className="text-gray-400">-</span>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      <Badge variant={therapist.isActive ? 'success' : 'neutral'}>
                        {therapist.isActive ? t('therapists.active') : t('therapists.inactive')}
                      </Badge>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2">
                        <button
                          onClick={() => handleEdit(therapist)}
                          className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                          title={t('common.edit')}
                        >
                          <Pencil className="h-4 w-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(therapist)}
                          className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-600"
                          title={t('common.delete')}
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      )}

      {modalOpen && (
        <TherapistFormModal
          therapist={editingTherapist}
          onClose={handleCloseModal}
        />
      )}
    </div>
  )
}
