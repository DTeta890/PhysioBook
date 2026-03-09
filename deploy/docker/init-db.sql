-- Enable required PostgreSQL extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Create application user with limited privileges
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'physiobook_app') THEN
    CREATE ROLE physiobook_app WITH LOGIN PASSWORD 'physiobook_app_dev';
  END IF;
END
$$;

GRANT CONNECT ON DATABASE physiobook TO physiobook_app;
GRANT USAGE ON SCHEMA public TO physiobook_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO physiobook_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO physiobook_app;
