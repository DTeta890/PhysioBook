import {
  createRouter,
  createRoute,
  createRootRoute,
  redirect,
} from '@tanstack/react-router'
import { AppLayout } from './layout/AppLayout'
import { LoginPage } from '@/features/auth/components/LoginPage'
import { CalendarPage } from '@/features/calendar/components/CalendarPage'
import { PatientsPage } from '@/features/patients/components/PatientsPage'
import { PatientDetailPage } from '@/features/patients/components/PatientDetailPage'
import { WalkInsPage } from '@/features/walkins/components/WalkInsPage'
import { BookingPage } from '@/features/booking/components/BookingPage'
import { AnalyticsPage } from '@/features/analytics/components/AnalyticsPage'
import { SettingsPage } from '@/features/auth/components/SettingsPage'
import { useAuth } from '@/features/auth/hooks/useAuth'

function isAuthenticated() {
  return useAuth.getState().isAuthenticated
}

// Root route
const rootRoute = createRootRoute()

// Public routes
const loginRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/login',
  component: LoginPage,
  beforeLoad: () => {
    if (isAuthenticated()) {
      throw redirect({ to: '/calendar' })
    }
  },
})

const bookingRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/booking',
  component: BookingPage,
})

// Protected layout route
const protectedLayoutRoute = createRoute({
  getParentRoute: () => rootRoute,
  id: 'protected',
  component: AppLayout,
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' })
    }
  },
})

// Index redirect
const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  beforeLoad: () => {
    throw redirect({ to: '/calendar' })
  },
})

// Protected routes
const calendarRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/calendar',
  component: CalendarPage,
})

const patientsRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/patients',
  component: PatientsPage,
})

const patientDetailRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/patients/$patientId',
  component: PatientDetailPage,
})

const walkInsRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/walk-ins',
  component: WalkInsPage,
})

const analyticsRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/analytics',
  component: AnalyticsPage,
})

const settingsRoute = createRoute({
  getParentRoute: () => protectedLayoutRoute,
  path: '/settings',
  component: SettingsPage,
})

// Route tree
const routeTree = rootRoute.addChildren([
  indexRoute,
  loginRoute,
  bookingRoute,
  protectedLayoutRoute.addChildren([
    calendarRoute,
    patientsRoute,
    patientDetailRoute,
    walkInsRoute,
    analyticsRoute,
    settingsRoute,
  ]),
])

export const router = createRouter({ routeTree })

// Type safety
declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}
