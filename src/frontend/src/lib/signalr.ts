import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from '@microsoft/signalr'
import { getAccessToken } from '@/lib/api-client'

const HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL ?? '/hubs/calendar'

let connection: HubConnection | null = null

export function getCalendarConnection(): HubConnection {
  if (!connection) {
    connection = new HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => getAccessToken() ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()
  }
  return connection
}

export async function startConnection(): Promise<void> {
  const conn = getCalendarConnection()
  if (conn.state === 'Disconnected') {
    await conn.start()
  }
}

export async function stopConnection(): Promise<void> {
  if (connection && connection.state !== 'Disconnected') {
    await connection.stop()
  }
}
