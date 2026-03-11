import { create } from 'zustand'
import { addWeeks, subWeeks, startOfToday } from 'date-fns'
import type { CalendarView } from '../types'

interface CalendarState {
  currentDate: Date
  view: CalendarView
  selectedDayIndex: number // 0=Mon, 1=Tue, ... 5=Sat
  setCurrentDate: (date: Date) => void
  setView: (view: CalendarView) => void
  setSelectedDayIndex: (index: number) => void
  goToNext: () => void
  goToPrev: () => void
  goToToday: () => void
}

export const useCalendarStore = create<CalendarState>((set) => ({
  currentDate: startOfToday(),
  view: 'week',
  selectedDayIndex: Math.min(new Date().getDay() === 0 ? 5 : new Date().getDay() - 1, 5),
  setCurrentDate: (date: Date) => set({ currentDate: date }),
  setView: (view: CalendarView) => set({ view }),
  setSelectedDayIndex: (index: number) => set({ selectedDayIndex: index }),
  goToNext: () =>
    set((state) => ({ currentDate: addWeeks(state.currentDate, 1) })),
  goToPrev: () =>
    set((state) => ({ currentDate: subWeeks(state.currentDate, 1) })),
  goToToday: () =>
    set({
      currentDate: startOfToday(),
      selectedDayIndex: Math.min(new Date().getDay() === 0 ? 5 : new Date().getDay() - 1, 5),
    }),
}))
