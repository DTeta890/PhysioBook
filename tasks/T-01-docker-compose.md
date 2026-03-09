# T-01: Docker Compose Local Dev Environment

## Status: ⬜ Not Started
## Phase: 0 — Foundation
## Dependencies: T-00
## Agents: devops-engineer

---

## Objective
Create a Docker Compose setup that spins up the entire local development stack with a single `docker-compose up`.

## Services Required

| Service | Image | Ports | Notes |
|---------|-------|-------|-------|
| postgres | postgres:16-alpine | 5432:5432 | With uuid-ossp and pg_trgm extensions |
| redis | redis:7-alpine | 6379:6379 | Session cache, rate limiting |
| rabbitmq | rabbitmq:3-management-alpine | 5672:5672, 15672:15672 | Management UI at :15672 |
| minio | minio/minio | 9000:9000, 9001:9001 | S3-compatible file storage |
| api | build from Dockerfile.api | 5000:5000 | .NET 8 API with hot reload |
| frontend | build from Dockerfile.frontend | 3000:3000 | Vite dev server with HMR |

## Steps

- [ ] 1. Create `deploy/docker/docker-compose.yml` with all services
- [ ] 2. Create `deploy/docker/Dockerfile.api` (multi-stage: sdk → aspnet runtime)
  - Dev stage uses `dotnet watch run` for hot reload
- [ ] 3. Create `deploy/docker/Dockerfile.frontend` (node:20-alpine with Vite dev server)
- [ ] 4. Create `deploy/docker/.env.example` with all required environment variables:
  - `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`
  - `RABBITMQ_DEFAULT_USER`, `RABBITMQ_DEFAULT_PASS`
  - `MINIO_ROOT_USER`, `MINIO_ROOT_PASSWORD`
  - `JWT_SECRET`, `JWT_ISSUER`, `JWT_AUDIENCE`
  - `REDIS_URL`
- [ ] 5. Create PostgreSQL init script `deploy/docker/init-db.sql`:
  - Enable extensions: uuid-ossp, pg_trgm
  - Create application database user with limited privileges
- [ ] 6. Add health checks for all services
- [ ] 7. Create `Makefile` or `scripts/dev.sh` with convenience commands:
  - `make up` — start all services
  - `make down` — stop all services
  - `make logs` — tail all logs
  - `make reset-db` — drop and recreate database
  - `make seed` — run seed data

## Verification
- `docker-compose up -d` starts all services without errors
- PostgreSQL accepts connections on 5432 with extensions enabled
- RabbitMQ management UI accessible at localhost:15672
- MinIO console accessible at localhost:9001
- API responds to health check at localhost:5000/health
- Frontend loads at localhost:3000

## Summary of Changes
<!-- Filled in after completion -->
