import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Pencil, Trash2 } from 'lucide-react'
import { PageHeader } from '@/shared/components/PageHeader'
import { Button } from '@/shared/components/Button'
import { Card } from '@/shared/components/Card'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import {
  useTreatmentTypes,
  useCreateTreatmentType,
  useUpdateTreatmentType,
  useDeleteTreatmentType,
} from '../hooks/useTreatmentTypes'
import { TreatmentTypeFormModal } from './TreatmentTypeFormModal'
import type { TreatmentType, CreateTreatmentTypeRequest } from '../types'

export function TreatmentTypesPage() {
  const { t } = useTranslation()
  const { data: treatmentTypes, isLoading } = useTreatmentTypes()
  const createMutation = useCreateTreatmentType()
  const updateMutation = useUpdateTreatmentType()
  const deleteMutation = useDeleteTreatmentType()

  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingType, setEditingType] = useState<TreatmentType | null>(null)

  const handleCreate = () => {
    setEditingType(null)
    setIsModalOpen(true)
  }

  const handleEdit = (treatmentType: TreatmentType) => {
    setEditingType(treatmentType)
    setIsModalOpen(true)
  }

  const handleDelete = (treatmentType: TreatmentType) => {
    if (window.confirm(t('treatmentTypes.deleteConfirm'))) {
      deleteMutation.mutate(treatmentType.id)
    }
  }

  const handleSubmit = (data: CreateTreatmentTypeRequest & { isActive?: boolean }) => {
    if (editingType) {
      updateMutation.mutate(
        {
          id: editingType.id,
          data: {
            name: data.name,
            description: data.description,
            durationMinutes: data.durationMinutes,
            price: data.price,
            color: data.color,
            isActive: data.isActive ?? true,
          },
        },
        {
          onSuccess: () => setIsModalOpen(false),
        },
      )
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setIsModalOpen(false),
      })
    }
  }

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('sq-AL', {
      style: 'currency',
      currency: 'ALL',
    }).format(price)
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
        title={t('treatmentTypes.title')}
        actions={
          <Button onClick={handleCreate}>{t('treatmentTypes.add')}</Button>
        }
      />

      {!treatmentTypes || treatmentTypes.length === 0 ? (
        <Card>
          <EmptyState
            title={t('treatmentTypes.empty')}
            description={t('treatmentTypes.emptyDescription')}
            action={
              <Button onClick={handleCreate}>{t('treatmentTypes.add')}</Button>
            }
          />
        </Card>
      ) : (
        <Card padding={false}>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                    {t('treatmentTypes.name')}
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                    {t('treatmentTypes.duration')}
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                    {t('treatmentTypes.price')}
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                    {t('treatmentTypes.color')}
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                    Status
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
                    {t('common.edit')}
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200 bg-white">
                {treatmentTypes.map((tt) => (
                  <tr key={tt.id} className="hover:bg-gray-50">
                    <td className="whitespace-nowrap px-6 py-4">
                      <div className="text-sm font-medium text-gray-900">
                        {tt.name}
                      </div>
                      {tt.description && (
                        <div className="text-sm text-gray-500">{tt.description}</div>
                      )}
                    </td>
                    <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">
                      {tt.durationMinutes} {t('treatmentTypes.minutes')}
                    </td>
                    <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-700">
                      {formatPrice(tt.price)}
                    </td>
                    <td className="whitespace-nowrap px-6 py-4">
                      {tt.color ? (
                        <div className="flex items-center gap-2">
                          <span
                            className="inline-block h-5 w-5 rounded-full border border-gray-200"
                            style={{ backgroundColor: tt.color }}
                          />
                          <span className="text-xs text-gray-500">{tt.color}</span>
                        </div>
                      ) : (
                        <span className="text-sm text-gray-400">--</span>
                      )}
                    </td>
                    <td className="whitespace-nowrap px-6 py-4">
                      <Badge variant={tt.isActive ? 'success' : 'neutral'}>
                        {tt.isActive
                          ? t('treatmentTypes.active')
                          : t('treatmentTypes.inactive')}
                      </Badge>
                    </td>
                    <td className="whitespace-nowrap px-6 py-4 text-right">
                      <div className="flex items-center justify-end gap-2">
                        <button
                          type="button"
                          onClick={() => handleEdit(tt)}
                          className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                          aria-label={t('common.edit')}
                        >
                          <Pencil className="h-4 w-4" />
                        </button>
                        <button
                          type="button"
                          onClick={() => handleDelete(tt)}
                          className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-600"
                          aria-label={t('common.delete')}
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

      <TreatmentTypeFormModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleSubmit}
        treatmentType={editingType}
        isSubmitting={createMutation.isPending || updateMutation.isPending}
      />
    </div>
  )
}
