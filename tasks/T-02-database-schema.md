# T-02: PostgreSQL Schema + RLS Foundation

## Status: ⬜ Not Started
## Phase: 0 — Foundation
## Dependencies: T-01
## Agents: database-engineer

---

## Objective
Create the core database schema with the tenants table, users table, and establish the Row-Level Security pattern that all future tables will follow.

## Tables to Create

### tenants
```sql
CREATE TABLE tenants (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(100) UNIQUE NOT NULL,
  email VARCHAR(255) NOT NULL,
  phone VARCHAR(50),
  address TEXT,
  city VARCHAR(100) DEFAULT 'Elbasan',
  timezone VARCHAR(50) DEFAULT 'Europe/Tirane',
  logo_url VARCHAR(500),
  subscription_plan VARCHAR(50) DEFAULT 'starter',
  subscription_status VARCHAR(20) DEFAULT 'trial',
  trial_ends_at TIMESTAMPTZ,
  settings JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);
```

### users
```sql
CREATE TABLE users (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
  email VARCHAR(255) NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  phone VARCHAR(50),
  role VARCHAR(30) NOT NULL CHECK (role IN ('owner', 'admin', 'therapist', 'receptionist')),
  is_therapist BOOLEAN DEFAULT false,
  specialization VARCHAR(255),
  color VARCHAR(7),
  avatar_url VARCHAR(500),
  is_active BOOLEAN DEFAULT true,
  last_login_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW(),
  UNIQUE (tenant_id, email)
);
```

### refresh_tokens
```sql
CREATE TABLE refresh_tokens (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  token_hash VARCHAR(255) NOT NULL UNIQUE,
  expires_at TIMESTAMPTZ NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  revoked_at TIMESTAMPTZ
);
```

## Steps

- [ ] 1. Create initial migration with extensions: `uuid-ossp`, `pg_trgm`
- [ ] 2. Create `tenants` table (this is the ONLY table without `tenant_id`)
- [ ] 3. Create `users` table with `tenant_id` FK
- [ ] 4. Create `refresh_tokens` table
- [ ] 5. Enable RLS on `users` table:
  ```sql
  ALTER TABLE users ENABLE ROW LEVEL SECURITY;
  ALTER TABLE users FORCE ROW LEVEL SECURITY;
  CREATE POLICY tenant_isolation_users ON users
    USING (tenant_id = current_setting('app.current_tenant')::uuid);
  ```
- [ ] 6. Create `updated_at` auto-update trigger function:
  ```sql
  CREATE FUNCTION update_updated_at()
  RETURNS TRIGGER AS $$
  BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
  END;
  $$ LANGUAGE plpgsql;
  ```
- [ ] 7. Apply trigger to `tenants` and `users` tables
- [ ] 8. Create indexes:
  - `idx_users_tenant_email ON users(tenant_id, email)`
  - `idx_users_tenant_role ON users(tenant_id, role)`
  - `idx_users_tenant_therapist ON users(tenant_id, is_therapist) WHERE is_therapist = true`
- [ ] 9. Create seed data:
  - One demo tenant: "Klinika Fizioterapise Demo" with slug "demo"
  - One owner user: admin@physiobook.al / password (bcrypt hashed)
- [ ] 10. Write EF Core entity configurations matching the schema

## Verification
- Migration runs without errors
- RLS test: set `app.current_tenant` to a random UUID, query users → empty result
- RLS test: set `app.current_tenant` to demo tenant UUID, query users → returns admin user
- Seed data loads correctly
- EF Core `dotnet ef database update` succeeds

## Summary of Changes
<!-- Filled in after completion -->
