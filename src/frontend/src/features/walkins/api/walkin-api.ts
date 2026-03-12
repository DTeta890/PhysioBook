import { apiClient } from '@/lib/api-client'
import type { WalkInEntry, WalkInStats, CheckInRequest, ConvertRequest } from '../types'

export const walkinApi = {
  getQueue: (status?: string, date?: string) => {
    const params = new URLSearchParams()
    if (status) params.set('status', status)
    if (date) params.set('date', date)
    const query = params.toString()
    return apiClient.get<WalkInEntry[]>(`/walkins${query ? `?${query}` : ''}`)
  },

  getById: (id: string) => apiClient.get<WalkInEntry>(`/walkins/${id}`),

  getStats: (from: string, to: string) => {
    const params = new URLSearchParams({ from, to })
    return apiClient.get<WalkInStats>(`/walkins/stats?${params.toString()}`)
  },

  checkIn: (data: CheckInRequest) => apiClient.post<WalkInEntry>('/walkins', data),

  call: (id: string, therapistId: string) =>
    apiClient.patch<WalkInEntry>(`/walkins/${id}/call`, { therapistId }),

  complete: (id: string) => apiClient.patch<WalkInEntry>(`/walkins/${id}/complete`),

  cancel: (id: string, reason?: string) =>
    apiClient.patch<WalkInEntry>(`/walkins/${id}/cancel`, { reason }),

  convertToAppointment: (id: string, data: ConvertRequest) =>
    apiClient.post<WalkInEntry>(`/walkins/${id}/convert`, data),
}
