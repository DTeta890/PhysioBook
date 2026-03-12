export interface Patient {
  id: string
  firstName: string
  lastName: string
  fullName: string
  email: string | null
  phone: string | null
  dateOfBirth: string | null
  gender: string | null
  address: string | null
  city: string | null
  emergencyContactName: string | null
  emergencyContactPhone: string | null
  medicalHistory: string | null
  allergies: string | null
  notes: string | null
  isActive: boolean
  createdAt: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface CreatePatientRequest {
  firstName: string
  lastName: string
  email?: string
  phone?: string
  dateOfBirth?: string
  gender?: string
  address?: string
  city?: string
  emergencyContactName?: string
  emergencyContactPhone?: string
  medicalHistory?: string
  allergies?: string
  notes?: string
}

export interface UpdatePatientRequest extends CreatePatientRequest {
  isActive: boolean
}

export interface PatientListParams {
  search?: string
  isActive?: boolean
  page?: number
  pageSize?: number
}

export interface AppointmentSummary {
  id: string
  therapistId: string
  therapistName: string
  patientId: string | null
  patientName: string | null
  patientPhone: string | null
  treatmentTypeId: string
  treatmentTypeName: string
  startTime: string
  endTime: string
  status: string
  notes: string | null
  cancellationReason: string | null
  isWalkIn: boolean
  color: string | null
  recurringRuleId: string | null
  createdAt: string
}

export interface TreatmentNote {
  id: string
  appointmentId: string
  patientId: string | null
  patientName: string | null
  therapistId: string
  therapistName: string
  appointmentDate: string
  subjective: string | null
  objective: string | null
  assessment: string | null
  plan: string | null
  diagnosis: string | null
  treatmentProvided: string | null
  painLevelBefore: number | null
  painLevelAfter: number | null
  rangeOfMotionNotes: string | null
  exercisesPrescribed: string | null
  followUpInstructions: string | null
  isSigned: boolean
  signedAt: string | null
  createdAt: string
}

export interface PatientPackage {
  id: string
  patientId: string
  treatmentPackageId: string
  treatmentPackageName: string
  treatmentTypeName: string
  totalSessions: number
  sessionsUsed: number
  sessionsRemaining: number
  price: number
  purchasedAt: string
  expiresAt: string | null
  status: string
  notes: string | null
  createdAt: string
}

export interface TimelineEvent {
  id: string
  type: 'appointment' | 'treatment_note' | 'package_purchase'
  date: string
  data: AppointmentSummary | TreatmentNote | PatientPackage
}
