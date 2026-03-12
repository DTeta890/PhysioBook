-- Migration: 010_walk_in_entries
-- Description: Create walk_in_entries table for walk-in queue management

BEGIN;

CREATE TABLE walk_in_entries (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    patient_id UUID REFERENCES users(id) ON DELETE SET NULL,
    patient_name VARCHAR(200) NOT NULL,
    patient_phone VARCHAR(50),
    treatment_type_id UUID REFERENCES treatment_types(id) ON DELETE SET NULL,
    reason_for_visit VARCHAR(500),
    priority INTEGER NOT NULL DEFAULT 1,
    status VARCHAR(20) NOT NULL DEFAULT 'waiting',
    checked_in_at TIMESTAMPTZ NOT NULL,
    called_at TIMESTAMPTZ,
    completed_at TIMESTAMPTZ,
    assigned_therapist_id UUID REFERENCES users(id) ON DELETE SET NULL,
    converted_appointment_id UUID REFERENCES appointments(id) ON DELETE SET NULL,
    queue_position INTEGER NOT NULL,
    notes VARCHAR(1000),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT chk_walk_in_status CHECK (status IN ('waiting', 'in_progress', 'served', 'no_show', 'cancelled')),
    CONSTRAINT chk_walk_in_priority CHECK (priority IN (1, 2))
);

-- Indexes
CREATE INDEX idx_walk_in_entries_tenant_status
    ON walk_in_entries (tenant_id, status)
    WHERE status = 'waiting';

CREATE INDEX idx_walk_in_entries_tenant_checked_in
    ON walk_in_entries (tenant_id, checked_in_at);

CREATE INDEX idx_walk_in_entries_tenant_patient
    ON walk_in_entries (tenant_id, patient_id)
    WHERE patient_id IS NOT NULL;

-- Row-Level Security
ALTER TABLE walk_in_entries ENABLE ROW LEVEL SECURITY;

CREATE POLICY walk_in_entries_tenant_isolation ON walk_in_entries
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

CREATE POLICY walk_in_entries_tenant_insert ON walk_in_entries
    FOR INSERT
    WITH CHECK (tenant_id = current_setting('app.current_tenant')::UUID);

COMMIT;
