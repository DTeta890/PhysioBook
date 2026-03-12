-- Migration: 010_patient_documents
-- Description: Create patient_documents table for storing document metadata

BEGIN;

-- Create patient_documents table
CREATE TABLE IF NOT EXISTS patient_documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    patient_id UUID NOT NULL,
    file_name VARCHAR(500) NOT NULL,
    storage_key VARCHAR(1000) NOT NULL,
    content_type VARCHAR(255) NOT NULL,
    file_size_bytes BIGINT NOT NULL CHECK (file_size_bytes > 0),
    category VARCHAR(50) NOT NULL DEFAULT 'other',
    description VARCHAR(1000),
    uploaded_by VARCHAR(255) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_patient_documents_tenant_patient ON patient_documents (tenant_id, patient_id);
CREATE INDEX idx_patient_documents_tenant_category ON patient_documents (tenant_id, category);

-- Row-Level Security
ALTER TABLE patient_documents ENABLE ROW LEVEL SECURITY;

CREATE POLICY patient_documents_tenant_isolation ON patient_documents
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

CREATE POLICY patient_documents_tenant_insert ON patient_documents
    FOR INSERT
    WITH CHECK (tenant_id = current_setting('app.current_tenant')::UUID);

CREATE POLICY patient_documents_tenant_update ON patient_documents
    FOR UPDATE
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

CREATE POLICY patient_documents_tenant_delete ON patient_documents
    FOR DELETE
    USING (tenant_id = current_setting('app.current_tenant')::UUID);

COMMIT;
