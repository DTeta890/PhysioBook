CREATE TABLE treatment_types (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  duration_minutes INTEGER NOT NULL DEFAULT 30,
  price DECIMAL(10,2) NOT NULL DEFAULT 0,
  color VARCHAR(7),
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TRIGGER trg_treatment_types_updated_at
  BEFORE UPDATE ON treatment_types
  FOR EACH ROW EXECUTE FUNCTION update_updated_at();

CREATE INDEX idx_treatment_types_tenant ON treatment_types(tenant_id);
CREATE INDEX idx_treatment_types_tenant_active ON treatment_types(tenant_id, is_active) WHERE is_active = true;

ALTER TABLE treatment_types ENABLE ROW LEVEL SECURITY;
ALTER TABLE treatment_types FORCE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_treatment_types ON treatment_types
  USING (tenant_id = current_setting('app.current_tenant')::uuid);
