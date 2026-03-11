CREATE TABLE appointments (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  therapist_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  patient_id UUID REFERENCES users(id) ON DELETE SET NULL,
  treatment_type_id UUID NOT NULL REFERENCES treatment_types(id) ON DELETE RESTRICT,
  patient_name VARCHAR(255),
  patient_phone VARCHAR(50),
  start_time TIMESTAMPTZ NOT NULL,
  end_time TIMESTAMPTZ NOT NULL,
  status VARCHAR(20) NOT NULL DEFAULT 'scheduled' CHECK (status IN ('scheduled','confirmed','in_progress','completed','cancelled','no_show')),
  notes TEXT,
  cancellation_reason TEXT,
  is_walk_in BOOLEAN DEFAULT false,
  color VARCHAR(7),
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TRIGGER trg_appointments_updated_at
  BEFORE UPDATE ON appointments
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

-- Indexes
CREATE INDEX idx_appointments_tenant_therapist_date ON appointments(tenant_id, therapist_id, start_time);
CREATE INDEX idx_appointments_tenant_date ON appointments(tenant_id, start_time);
CREATE INDEX idx_appointments_tenant_patient ON appointments(tenant_id, patient_id) WHERE patient_id IS NOT NULL;
CREATE INDEX idx_appointments_tenant_status ON appointments(tenant_id, status) WHERE status NOT IN ('completed', 'cancelled');

-- RLS
ALTER TABLE appointments ENABLE ROW LEVEL SECURITY;
ALTER TABLE appointments FORCE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_appointments ON appointments
  USING (tenant_id = current_setting('app.current_tenant')::uuid);
