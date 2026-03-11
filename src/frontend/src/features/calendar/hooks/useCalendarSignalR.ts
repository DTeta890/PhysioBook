import { useEffect } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { getCalendarConnection, startConnection, stopConnection } from '@/lib/signalr'

export function useCalendarSignalR() {
  const queryClient = useQueryClient()

  useEffect(() => {
    const connection = getCalendarConnection()

    connection.on('AppointmentCreated', () => {
      void queryClient.invalidateQueries({ queryKey: ['appointments'] })
    })

    connection.on('AppointmentUpdated', () => {
      void queryClient.invalidateQueries({ queryKey: ['appointments'] })
    })

    connection.on('AppointmentDeleted', () => {
      void queryClient.invalidateQueries({ queryKey: ['appointments'] })
    })

    connection.on('AppointmentsGenerated', () => {
      void queryClient.invalidateQueries({ queryKey: ['appointments'] })
    })

    void startConnection().catch(console.error)

    return () => {
      connection.off('AppointmentCreated')
      connection.off('AppointmentUpdated')
      connection.off('AppointmentDeleted')
      connection.off('AppointmentsGenerated')
      void stopConnection()
    }
  }, [queryClient])
}
