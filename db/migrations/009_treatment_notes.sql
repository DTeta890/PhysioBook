-- Migration: 008_treatment_notes
-- Description: Create treatment_notes table with SOAP format and RLS
-- Date: 2026-03-12

CREATE TABLE treatment_notes (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  appointment_id UUID NOT NULL REFERENCES appointments(id) ON DELETE CASCADE,
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  therapist_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  -- SOAP format
  subjective TEXT,
  objective TEXT,
  assessment TEXT,
  plan TEXT,
  -- Additional
  diagnosis TEXT,
  treatment_provided TEXT,
  pain_level_before SMALLINT CHECK (pain_level_before BETWEEN 0 AND 10),
  pain_level_after SMALLINT CHECK (pain_level_after BETWEEN 0 AND 10),
  range_of_motion_notes TEXT,
  exercises_prescribed TEXT,
  follow_up_instructions TEXT,
  is_signed BOOLEAN DEFAULT false,
  signed_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TRIGGER trg_treatment_notes_updated_at
  BEFORE UPDATE ON treatment_notes
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

-- Indexes
CREATE INDEX idx_treatment_notes_tenant_appointment ON treatment_notes(tenant_id, appointment_id);
CREATE INDEX idx_treatment_notes_tenant_patient ON treatment_notes(tenant_id, patient_id) WHERE patient_id IS NOT NULL;
CREATE INDEX idx_treatment_notes_tenant_therapist ON treatment_notes(tenant_id, therapist_id);
CREATE UNIQUE INDEX idx_treatment_notes_appointment_unique ON treatment_notes(appointment_id);

-- RLS
ALTER TABLE treatment_notes ENABLE ROW LEVEL SECURITY;
ALTER TABLE treatment_notes FORCE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_treatment_notes ON treatment_notes
  USING (tenant_id = current_setting('app.current_tenant')::uuid);
