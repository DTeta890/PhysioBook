CREATE TABLE recurring_rules (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  therapist_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  patient_id UUID REFERENCES users(id) ON DELETE SET NULL,
  treatment_type_id UUID NOT NULL REFERENCES treatment_types(id) ON DELETE RESTRICT,
  patient_name VARCHAR(255),
  patient_phone VARCHAR(50),
  frequency VARCHAR(20) NOT NULL DEFAULT 'weekly' CHECK (frequency IN ('weekly', 'biweekly', 'monthly')),
  day_of_week INTEGER NOT NULL CHECK (day_of_week BETWEEN 0 AND 6),
  start_time_of_day TIME NOT NULL,
  end_time_of_day TIME NOT NULL,
  starts_from TIMESTAMPTZ NOT NULL,
  ends_at TIMESTAMPTZ,
  max_occurrences INTEGER,
  notes TEXT,
  color VARCHAR(7),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TRIGGER trg_recurring_rules_updated_at
  BEFORE UPDATE ON recurring_rules
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

CREATE INDEX idx_recurring_rules_tenant ON recurring_rules(tenant_id);
CREATE INDEX idx_recurring_rules_tenant_therapist ON recurring_rules(tenant_id, therapist_id);
CREATE INDEX idx_recurring_rules_active ON recurring_rules(tenant_id, is_active) WHERE is_active = true;

ALTER TABLE recurring_rules ENABLE ROW LEVEL SECURITY;
ALTER TABLE recurring_rules FORCE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_recurring_rules ON recurring_rules
  USING (tenant_id = current_setting('app.current_tenant')::uuid);

-- Add recurring_rule_id FK to appointments
ALTER TABLE appointments ADD CONSTRAINT fk_appointments_recurring_rule
  FOREIGN KEY (recurring_rule_id) REFERENCES recurring_rules(id) ON DELETE SET NULL;
CREATE INDEX idx_appointments_recurring_rule ON appointments(recurring_rule_id) WHERE recurring_rule_id IS NOT NULL;
