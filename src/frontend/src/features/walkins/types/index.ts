export interface WalkInEntry {
  id: string
  patientId: string | null
  patientName: string
  patientPhone: string | null
  treatmentTypeId: string | null
  treatmentTypeName: string | null
  reasonForVisit: string | null
  priority: number
  status: string
  checkedInAt: string
  calledAt: string | null
  completedAt: string | null
  assignedTherapistId: string | null
  assignedTherapistName: string | null
  convertedAppointmentId: string | null
  queuePosition: number
  waitTimeMinutes: number
  notes: string | null
  createdAt: string
}

export interface WalkInStats {
  totalWalkIns: number
  averageWaitMinutes: number
  servedCount: number
  noShowCount: number
  cancelledCount: number
  peakHour: number | null
}

export interface CheckInRequest {
  patientId?: string
  patientName: string
  patientPhone?: string
  treatmentTypeId?: string
  reasonForVisit?: string
  priority?: number
  notes?: string
}

export interface CallRequest {
  therapistId: string
}

export interface ConvertRequest {
  therapistId: string
  treatmentTypeId: string
  startTime: string
  endTime: string
  notes?: string
}
