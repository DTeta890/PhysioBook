-- Migration: 002_rls_refresh_tokens
-- Description: Add RLS policy to refresh_tokens via user's tenant_id
-- Date: 2026-03-11

-- Enable RLS on refresh_tokens (isolated via user's tenant_id)
ALTER TABLE refresh_tokens ENABLE ROW LEVEL SECURITY;
ALTER TABLE refresh_tokens FORCE ROW LEVEL SECURITY;

CREATE POLICY tenant_isolation_refresh_tokens ON refresh_tokens
  USING (
    user_id IN (
      SELECT id FROM users WHERE tenant_id = current_setting('app.current_tenant')::uuid
    )
  );
