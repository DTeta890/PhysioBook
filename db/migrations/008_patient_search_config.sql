-- Migration: 008_patient_search_config
-- Description: Configure pg_trgm for patient fuzzy search
-- Date: 2026-03-12

-- Set a lower similarity threshold for better fuzzy matching.
-- This is a session-level setting; for persistence across all sessions,
-- add the following to postgresql.conf:
--   pg_trgm.similarity_threshold = 0.1
--
-- The default threshold is 0.3 which may be too strict for short Albanian names.
-- A threshold of 0.1 provides broader fuzzy matches while still filtering noise.

-- No schema changes needed - pg_trgm indexes were created in 007_patients.sql
