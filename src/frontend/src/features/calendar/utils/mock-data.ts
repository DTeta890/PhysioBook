import { format, addDays, startOfWeek } from 'date-fns'
import type { Appointment, Therapist } from '../types'

function getThisWeekDate(dayOffset: number): string {
  const weekStart = startOfWeek(new Date(), { weekStartsOn: 1 })
  return format(addDays(weekStart, dayOffset), 'yyyy-MM-dd')
}

export const mockTherapists: Therapist[] = [
  {
    id: 'therapist-1',
    firstName: 'Ana',
    lastName: 'Hoxha',
    color: '#22c55e',
    specialization: 'Sports Rehabilitation',
  },
  {
    id: 'therapist-2',
    firstName: 'Dritan',
    lastName: 'Kola',
    color: '#3b82f6',
    specialization: 'Manual Therapy',
  },
  {
    id: 'therapist-3',
    firstName: 'Elira',
    lastName: 'Basha',
    color: '#f59e0b',
    specialization: 'Pediatric Therapy',
  },
]

export const mockAppointments: Appointment[] = [
  // Monday
  {
    id: 'apt-1',
    patientName: 'Arben Dervishi',
    therapistId: 'therapist-1',
    treatmentType: 'Sports Massage',
    startTime: `${getThisWeekDate(0)}T09:00:00`,
    endTime: `${getThisWeekDate(0)}T10:00:00`,
    durationMinutes: 60,
    status: 'confirmed',
    color: '#22c55e',
    notes: 'Knee injury follow-up',
  },
  {
    id: 'apt-2',
    patientName: 'Besa Murati',
    therapistId: 'therapist-1',
    treatmentType: 'Ultrasound Therapy',
    startTime: `${getThisWeekDate(0)}T10:30:00`,
    endTime: `${getThisWeekDate(0)}T11:15:00`,
    durationMinutes: 45,
    status: 'scheduled',
    color: '#3b82f6',
  },
  {
    id: 'apt-3',
    patientName: 'Gentian Leka',
    therapistId: 'therapist-2',
    treatmentType: 'Manual Therapy',
    startTime: `${getThisWeekDate(0)}T09:30:00`,
    endTime: `${getThisWeekDate(0)}T10:30:00`,
    durationMinutes: 60,
    status: 'confirmed',
    color: '#f59e0b',
  },
  {
    id: 'apt-4',
    patientName: 'Dorina Shkurti',
    therapistId: 'therapist-3',
    treatmentType: 'Pediatric Assessment',
    startTime: `${getThisWeekDate(0)}T11:00:00`,
    endTime: `${getThisWeekDate(0)}T12:00:00`,
    durationMinutes: 60,
    status: 'scheduled',
    color: '#ef4444',
  },
  // Tuesday
  {
    id: 'apt-5',
    patientName: 'Edi Hasa',
    therapistId: 'therapist-1',
    treatmentType: 'Sports Rehabilitation',
    startTime: `${getThisWeekDate(1)}T08:00:00`,
    endTime: `${getThisWeekDate(1)}T09:00:00`,
    durationMinutes: 60,
    status: 'confirmed',
    color: '#22c55e',
  },
  {
    id: 'apt-6',
    patientName: 'Flora Beqiri',
    therapistId: 'therapist-2',
    treatmentType: 'Electrotherapy',
    startTime: `${getThisWeekDate(1)}T10:00:00`,
    endTime: `${getThisWeekDate(1)}T10:30:00`,
    durationMinutes: 30,
    status: 'scheduled',
    color: '#8b5cf6',
  },
  // Wednesday
  {
    id: 'apt-7',
    patientName: 'Genti Prifti',
    therapistId: 'therapist-1',
    treatmentType: 'Post-Surgery Rehab',
    startTime: `${getThisWeekDate(2)}T14:00:00`,
    endTime: `${getThisWeekDate(2)}T15:00:00`,
    durationMinutes: 60,
    status: 'scheduled',
    color: '#ec4899',
  },
  {
    id: 'apt-8',
    patientName: 'Hana Topi',
    therapistId: 'therapist-3',
    treatmentType: 'Balance Training',
    startTime: `${getThisWeekDate(2)}T09:00:00`,
    endTime: `${getThisWeekDate(2)}T09:45:00`,
    durationMinutes: 45,
    status: 'confirmed',
    color: '#14b8a6',
  },
  // Thursday
  {
    id: 'apt-9',
    patientName: 'Ilir Shehu',
    therapistId: 'therapist-2',
    treatmentType: 'Spinal Manipulation',
    startTime: `${getThisWeekDate(3)}T11:00:00`,
    endTime: `${getThisWeekDate(3)}T12:00:00`,
    durationMinutes: 60,
    status: 'scheduled',
    color: '#f59e0b',
  },
  // Friday
  {
    id: 'apt-10',
    patientName: 'Jeta Rama',
    therapistId: 'therapist-1',
    treatmentType: 'Hot/Cold Therapy',
    startTime: `${getThisWeekDate(4)}T15:00:00`,
    endTime: `${getThisWeekDate(4)}T15:30:00`,
    durationMinutes: 30,
    status: 'confirmed',
    color: '#06b6d4',
  },
]
