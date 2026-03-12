-- Migration: 007_patients
-- Description: Create patients table with RLS and update appointments FK

CREATE TABLE patients (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  email VARCHAR(255),
  phone VARCHAR(50),
  date_of_birth DATE,
  gender VARCHAR(10) CHECK (gender IN ('male', 'female', 'other')),
  address TEXT,
  city VARCHAR(100),
  emergency_contact_name VARCHAR(200),
  emergency_contact_phone VARCHAR(50),
  medical_history TEXT,
  allergies TEXT,
  notes TEXT,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TRIGGER trg_patients_updated_at
  BEFORE UPDATE ON patients
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

-- Indexes
CREATE INDEX idx_patients_tenant_name ON patients(tenant_id, last_name, first_name);
CREATE INDEX idx_patients_tenant_phone ON patients(tenant_id, phone) WHERE phone IS NOT NULL;
CREATE INDEX idx_patients_tenant_email ON patients(tenant_id, email) WHERE email IS NOT NULL;
CREATE INDEX idx_patients_tenant_active ON patients(tenant_id, is_active) WHERE is_active = true;

-- pg_trgm indexes for fuzzy search
CREATE INDEX idx_patients_first_name_trgm ON patients USING gin (first_name gin_trgm_ops);
CREATE INDEX idx_patients_last_name_trgm ON patients USING gin (last_name gin_trgm_ops);
CREATE INDEX idx_patients_phone_trgm ON patients USING gin (phone gin_trgm_ops) WHERE phone IS NOT NULL;

-- RLS
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;
ALTER TABLE patients FORCE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_patients ON patients
  USING (tenant_id = current_setting('app.current_tenant')::uuid);

-- Update appointments.patient_id to reference patients table instead of users
-- First drop the existing FK if it exists
ALTER TABLE appointments DROP CONSTRAINT IF EXISTS appointments_patient_id_fkey;
ALTER TABLE appointments
  ADD CONSTRAINT appointments_patient_id_fkey
  FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE SET NULL;
