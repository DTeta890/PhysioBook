import { create } from 'zustand'
import { addDays, subDays } from 'date-fns'
import type { CalendarView } from '../types'

interface CalendarState {
  currentDate: Date
  view: CalendarView
  selectedDayIndex: number
  setCurrentDate: (date: Date) => void
  setView: (view: CalendarView) => void
  setSelectedDayIndex: (index: number) => void
  goToToday: () => void
  goForward: () => void
  goBack: () => void
}

export const useCalendarStore = create<CalendarState>((set, get) => ({
  currentDate: new Date(),
  view: 'week',
  selectedDayIndex: new Date().getDay() === 0 ? 6 : new Date().getDay() - 1, // Monday=0

  setCurrentDate: (date) => set({ currentDate: date }),
  setView: (view) => set({ view }),
  setSelectedDayIndex: (index) => set({ selectedDayIndex: index }),

  goToToday: () =>
    set({
      currentDate: new Date(),
      selectedDayIndex: new Date().getDay() === 0 ? 6 : new Date().getDay() - 1,
    }),

  goForward: () => {
    const { currentDate, view } = get()
    const days = view === 'week' ? 7 : 1
    set({ currentDate: addDays(currentDate, days) })
  },

  goBack: () => {
    const { currentDate, view } = get()
    const days = view === 'week' ? 7 : 1
    set({ currentDate: subDays(currentDate, days) })
  },
}))
