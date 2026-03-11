-- Seed: Demo tenant and admin user
-- Password: Admin123! (bcrypt cost 12)
-- Generated hash for seeding only — change in production

-- Demo tenant
INSERT INTO tenants (id, name, slug, email, phone, address, city, timezone, subscription_plan, subscription_status, trial_ends_at)
VALUES (
  'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
  'Klinika Fizioterapise Demo',
  'demo',
  'info@demo.physiobook.al',
  '+355 68 000 0000',
  'Rruga Qemal Stafa, Nr. 1',
  'Elbasan',
  'Europe/Tirane',
  'professional',
  'active',
  NOW() + INTERVAL '30 days'
)
ON CONFLICT (slug) DO NOTHING;

-- Admin/owner user
-- Password: Admin123! hashed with bcrypt cost 12
SET app.current_tenant = 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11';

INSERT INTO users (id, tenant_id, email, password_hash, first_name, last_name, phone, role, is_active)
VALUES (
  'b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a22',
  'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
  'admin@physiobook.al',
  '$2a$12$LJ3m4ys3Lg2kFON.2kCHnOYAkTNBGqLMvWMPB5OnVb3cOsVMOXVLq',
  'Admin',
  'PhysioBook',
  '+355 68 111 1111',
  'owner',
  true
)
ON CONFLICT (tenant_id, email) DO NOTHING;

RESET app.current_tenant;
