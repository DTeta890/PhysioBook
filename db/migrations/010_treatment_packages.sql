-- Migration: 010_treatment_packages
-- Description: Create treatment_packages and patient_packages tables with RLS

BEGIN;

-- =============================================================================
-- treatment_packages: package template/definition
-- =============================================================================
CREATE TABLE treatment_packages (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id       UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    name            VARCHAR(200) NOT NULL,
    treatment_type_id UUID NOT NULL REFERENCES treatment_types(id) ON DELETE RESTRICT,
    total_sessions  INT NOT NULL CHECK (total_sessions > 0),
    price           NUMERIC(10,2) NOT NULL DEFAULT 0 CHECK (price >= 0),
    validity_days   INT CHECK (validity_days IS NULL OR validity_days > 0),
    description     VARCHAR(1000),
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_treatment_packages_tenant
    ON treatment_packages(tenant_id);

CREATE INDEX idx_treatment_packages_tenant_treatment_type
    ON treatment_packages(tenant_id, treatment_type_id);

CREATE INDEX idx_treatment_packages_tenant_active
    ON treatment_packages(tenant_id, is_active)
    WHERE is_active = true;

-- RLS
ALTER TABLE treatment_packages ENABLE ROW LEVEL SECURITY;

CREATE POLICY treatment_packages_tenant_isolation ON treatment_packages
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

-- Updated_at trigger
CREATE TRIGGER set_treatment_packages_updated_at
    BEFORE UPDATE ON treatment_packages
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- =============================================================================
-- patient_packages: purchased package instance for a specific patient
-- =============================================================================
CREATE TABLE patient_packages (
    id                    UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id             UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    patient_id            UUID NOT NULL,
    treatment_package_id  UUID NOT NULL REFERENCES treatment_packages(id) ON DELETE RESTRICT,
    sessions_used         INT NOT NULL DEFAULT 0 CHECK (sessions_used >= 0),
    purchased_at          TIMESTAMPTZ NOT NULL,
    expires_at            TIMESTAMPTZ,
    status                VARCHAR(20) NOT NULL DEFAULT 'active'
                          CHECK (status IN ('active', 'completed', 'expired', 'cancelled')),
    notes                 VARCHAR(1000),
    created_at            TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at            TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_patient_packages_tenant
    ON patient_packages(tenant_id);

CREATE INDEX idx_patient_packages_tenant_patient
    ON patient_packages(tenant_id, patient_id);

CREATE INDEX idx_patient_packages_tenant_status
    ON patient_packages(tenant_id, status)
    WHERE status = 'active';

-- RLS
ALTER TABLE patient_packages ENABLE ROW LEVEL SECURITY;

CREATE POLICY patient_packages_tenant_isolation ON patient_packages
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

-- Updated_at trigger
CREATE TRIGGER set_patient_packages_updated_at
    BEFORE UPDATE ON patient_packages
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

COMMIT;
