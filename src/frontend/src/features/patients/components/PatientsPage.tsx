import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from '@tanstack/react-router'
import { Users, Pencil, Eye, Search } from 'lucide-react'
import { PageHeader } from '@/shared/components/PageHeader'
import { Button } from '@/shared/components/Button'
import { Card } from '@/shared/components/Card'
import { Badge } from '@/shared/components/Badge'
import { EmptyState } from '@/shared/components/EmptyState'
import { LoadingSpinner } from '@/shared/components/LoadingSpinner'
import { cn } from '@/shared/utils/cn'
import { usePatients } from '../hooks/usePatients'
import { PatientFormModal } from './PatientFormModal'
import type { Patient } from '../types'

type FilterStatus = 'all' | 'active' | 'inactive'

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

export function PatientsPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [modalOpen, setModalOpen] = useState(false)
  const [editingPatient, setEditingPatient] = useState<Patient | null>(null)
  const [searchInput, setSearchInput] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [filterStatus, setFilterStatus] = useState<FilterStatus>('all')
  const [page, setPage] = useState(1)
  const pageSize = 10

  // Debounce search input
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchInput)
      setPage(1)
    }, 300)
    return () => clearTimeout(timer)
  }, [searchInput])

  const isActiveFilter = filterStatus === 'all' ? undefined : filterStatus === 'active'
  const { data: pagedResult, isLoading } = usePatients(
    debouncedSearch || undefined,
    isActiveFilter,
    page,
    pageSize,
  )

  const patients = pagedResult?.items ?? []
  const totalPages = pagedResult?.totalPages ?? 1

  const handleAdd = () => {
    setEditingPatient(null)
    setModalOpen(true)
  }

  const handleEdit = (patient: Patient) => {
    setEditingPatient(patient)
    setModalOpen(true)
  }

  const handleView = (patient: Patient) => {
    void navigate({ to: '/patients/$patientId', params: { patientId: patient.id } })
  }

  const handleCloseModal = () => {
    setModalOpen(false)
    setEditingPatient(null)
  }

  const handleFilterChange = (status: FilterStatus) => {
    setFilterStatus(status)
    setPage(1)
  }

  if (isLoading) {
    return (
      <div>
        <PageHeader title={t('nav.patients')} />
        <div className="flex items-center justify-center py-12">
          <LoadingSpinner size="lg" />
        </div>
      </div>
    )
  }

  return (
    <div>
      <PageHeader
        title={t('nav.patients')}
        actions={
          <Button onClick={handleAdd}>
            {t('patients.add')}
          </Button>
        }
      />

      {/* Search and filters */}
      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="relative w-full sm:max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            placeholder={t('patients.searchPlaceholder')}
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            className="block w-full rounded-lg border border-gray-300 py-2 pl-10 pr-3 text-sm shadow-sm transition-colors placeholder:text-gray-400 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
          />
        </div>
        <div className="flex items-center gap-1 rounded-lg border border-gray-200 bg-gray-50 p-1">
          {(['all', 'active', 'inactive'] as const).map((status) => (
            <button
              key={status}
              onClick={() => handleFilterChange(status)}
              className={cn(
                'rounded-md px-3 py-1.5 text-sm font-medium transition-colors',
                filterStatus === status
                  ? 'bg-white text-gray-900 shadow-sm'
                  : 'text-gray-500 hover:text-gray-700',
              )}
            >
              {t(`patients.${status}`)}
            </button>
          ))}
        </div>
      </div>

      {patients.length === 0 ? (
        <EmptyState
          icon={<Users className="h-12 w-12" />}
          title={t('patients.empty')}
          description={t('patients.emptyDescription')}
          action={
            <Button onClick={handleAdd}>
              {t('patients.add')}
            </Button>
          }
        />
      ) : (
        <>
          {/* Desktop table */}
          <Card padding={false} className="hidden md:block">
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="border-b border-gray-200 bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 font-medium text-gray-500">
                      {t('patients.firstName')} / {t('patients.lastName')}
                    </th>
                    <th className="px-6 py-3 font-medium text-gray-500">
                      {t('patients.phone')}
                    </th>
                    <th className="px-6 py-3 font-medium text-gray-500">
                      {t('patients.email')}
                    </th>
                    <th className="px-6 py-3 font-medium text-gray-500">
                      {t('patients.dateOfBirth')}
                    </th>
                    <th className="px-6 py-3 font-medium text-gray-500">
                      Status
                    </th>
                    <th className="px-6 py-3 font-medium text-gray-500" />
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {patients.map((patient) => {
                    const age = calculateAge(patient.dateOfBirth)
                    return (
                      <tr key={patient.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4">
                          <button
                            onClick={() => handleView(patient)}
                            className="font-medium text-gray-900 hover:text-primary-600"
                          >
                            {patient.firstName} {patient.lastName}
                          </button>
                        </td>
                        <td className="px-6 py-4 text-gray-600">
                          {patient.phone ?? t('patients.noPhone')}
                        </td>
                        <td className="px-6 py-4 text-gray-600">
                          {patient.email ?? t('patients.noEmail')}
                        </td>
                        <td className="px-6 py-4 text-gray-600">
                          {age !== null ? `${age} ${t('patients.age')}` : '-'}
                        </td>
                        <td className="px-6 py-4">
                          <Badge variant={patient.isActive ? 'success' : 'neutral'}>
                            {patient.isActive ? t('patients.active') : t('patients.inactive')}
                          </Badge>
                        </td>
                        <td className="px-6 py-4">
                          <div className="flex items-center gap-2">
                            <button
                              onClick={() => handleView(patient)}
                              className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                              title={t('patients.detail')}
                            >
                              <Eye className="h-4 w-4" />
                            </button>
                            <button
                              onClick={() => handleEdit(patient)}
                              className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                              title={t('common.edit')}
                            >
                              <Pencil className="h-4 w-4" />
                            </button>
                          </div>
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          </Card>

          {/* Mobile cards */}
          <div className="flex flex-col gap-3 md:hidden">
            {patients.map((patient) => {
              const age = calculateAge(patient.dateOfBirth)
              return (
                <Card key={patient.id}>
                  <div className="flex items-start justify-between">
                    <div>
                      <button
                        onClick={() => handleView(patient)}
                        className="text-base font-medium text-gray-900 hover:text-primary-600"
                      >
                        {patient.firstName} {patient.lastName}
                      </button>
                      <div className="mt-1 space-y-0.5 text-sm text-gray-500">
                        <p>{patient.phone ?? t('patients.noPhone')}</p>
                        <p>{patient.email ?? t('patients.noEmail')}</p>
                        {age !== null && (
                          <p>{age} {t('patients.age')}</p>
                        )}
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <Badge variant={patient.isActive ? 'success' : 'neutral'}>
                        {patient.isActive ? t('patients.active') : t('patients.inactive')}
                      </Badge>
                    </div>
                  </div>
                  <div className="mt-3 flex items-center gap-2 border-t border-gray-100 pt-3">
                    <Button size="sm" variant="outline" onClick={() => handleView(patient)}>
                      <Eye className="mr-1 h-3.5 w-3.5" />
                      {t('patients.detail')}
                    </Button>
                    <Button size="sm" variant="ghost" onClick={() => handleEdit(patient)}>
                      <Pencil className="mr-1 h-3.5 w-3.5" />
                      {t('common.edit')}
                    </Button>
                  </div>
                </Card>
              )
            })}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-4 flex items-center justify-center gap-2">
              <Button
                size="sm"
                variant="outline"
                disabled={page <= 1}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
              >
                {t('common.back')}
              </Button>
              <span className="px-3 text-sm text-gray-600">
                {page} / {totalPages}
              </span>
              <Button
                size="sm"
                variant="outline"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => p + 1)}
              >
                {'>'}
              </Button>
            </div>
          )}
        </>
      )}

      {modalOpen && (
        <PatientFormModal
          patient={editingPatient}
          onClose={handleCloseModal}
        />
      )}
    </div>
  )
}
