import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import '@/lib/i18n'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
  },
})

function AppShell() {
  const { t } = useTranslation()

  return (
    <div className="flex min-h-screen items-center justify-center bg-primary-50">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-primary-700">
          {t('app.name')}
        </h1>
        <p className="mt-2 text-lg text-primary-600">
          {t('app.welcome')}
        </p>
      </div>
    </div>
  )
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppShell />
    </QueryClientProvider>
  )
}
