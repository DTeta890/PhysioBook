# PhysioBook Development Roadmap

## Status Legend
- ⬜ Not Started
- 🟡 In Progress
- ✅ Completed
- 🔴 Blocked

---

## Phase 0: Foundation (Weeks 1–4)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-00 | Project scaffolding & repo setup | ✅ | — | main |
| T-01 | Docker Compose local dev environment | ✅ | T-00 | feature/T-01-docker |
| T-02 | PostgreSQL schema + RLS foundation | ✅ | T-01 | feature/T-02-database |
| T-03 | .NET 8 Clean Architecture solution setup | ✅ | T-00 | feature/T-03-backend-scaffold |
| T-04 | Multi-tenant middleware + RLS integration | ✅ | T-02, T-03 | feature/T-04-multitenancy |
| T-05 | JWT auth system (register, login, refresh) | ✅ | T-04 | feature/T-05-auth-backend |
| T-06 | Vite React TS project + TailwindCSS + Router | ✅ | T-00 | feature/T-06-frontend-scaffold |
| T-07 | Frontend auth (login page, token management) | ✅ | T-05, T-06 | feature/T-07-auth-frontend |
| T-08 | CI/CD pipeline (GitHub Actions + Docker build) | ✅ | T-01, T-03, T-06 | feature/T-08-cicd |
| T-09 | Tenant provisioning system | ✅ | T-04 | feature/T-09-tenant-provision |

## Phase 1: Core Calendar (Weeks 5–10)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-10 | Therapist CRUD (backend + frontend) | ✅ | T-07 | feature/T-10-therapists |
| T-11 | Treatment types CRUD (backend + frontend) | ✅ | T-07 | feature/T-11-treatment-types |
| T-12 | Appointment domain model + API | ✅ | T-10, T-11 | feature/T-12-appointments-api |
| T-13 | Calendar UI — weekly view with therapist columns | ✅ | T-06 | feature/T-13-calendar-ui |
| T-14 | Drag-and-drop scheduling with @dnd-kit | ✅ | T-12, T-13 | feature/T-14-dnd-calendar |
| T-15 | Appointment quick-create (click empty slot) | ✅ | T-14 | feature/T-15-quick-create |
| T-16 | Conflict detection (visual + API validation) | ✅ | T-14 | feature/T-16-conflicts |
| T-17 | Recurring appointments (rules + generation) | ✅ | T-12 | feature/T-17-recurring |
| T-18 | SignalR real-time calendar sync | ✅ | T-14 | feature/T-18-realtime |
| T-19 | Calendar daily + monthly views | ✅ | T-14 | feature/T-19-calendar-views |

## Phase 2: Patient Management & Walk-Ins (Weeks 11–14)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-20 | Patient CRUD (backend API) | ✅ | T-04 | feature/T-20-patients-api |
| T-21 | Patient management UI (list, search, profile) | ✅ | T-20, T-07 | feature/T-21-patients-ui |
| T-22 | Patient search with fuzzy matching (pg_trgm) | ✅ | T-20 | feature/T-22-patient-search |
| T-23 | Treatment notes (SOAP format) backend + UI | ✅ | T-12, T-20 | feature/T-23-treatment-notes |
| T-24 | Treatment packages (sessions tracking) | ✅ | T-20 | feature/T-24-packages |
| T-25 | Patient document uploads (MinIO) | ✅ | T-20 | feature/T-25-documents |
| T-26 | Walk-in queue backend API | ✅ | T-20 | feature/T-26-walkin-api |
| T-27 | Walk-in queue UI (check-in, wait time, convert) | ✅ | T-26, T-14 | feature/T-27-walkin-ui |
| T-28 | Patient history timeline view | ✅ | T-23, T-24 | feature/T-28-patient-history |

## Phase 3: Notifications (Weeks 15–16)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-29 | RabbitMQ setup + MassTransit integration | ⬜ | T-03 | feature/T-29-rabbitmq |
| T-30 | SMS reminder worker (24h + 1h before) | ⬜ | T-29, T-12 | feature/T-30-sms-worker |
| T-31 | Email notification worker | ⬜ | T-29 | feature/T-31-email-worker |
| T-32 | In-app notifications (bell icon + dropdown) | ⬜ | T-29, T-07 | feature/T-32-in-app-notif |

## Phase 4: Online Booking (Weeks 17–20)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-33 | Public booking API (slots, reserve, confirm) | ⬜ | T-12 | feature/T-33-booking-api |
| T-34 | Public booking page UI (standalone React) | ⬜ | T-33 | feature/T-34-booking-ui |
| T-35 | Albanian bank payment gateway integration | ⬜ | T-33 | feature/T-35-payments |
| T-36 | Payment webhook processing worker | ⬜ | T-35, T-29 | feature/T-36-payment-worker |
| T-37 | Booking confirmation (SMS + email) | ⬜ | T-30, T-31, T-33 | feature/T-37-booking-confirm |
| T-38 | Cancellation / reschedule flow | ⬜ | T-33 | feature/T-38-cancel-reschedule |

## Phase 5: Analytics & Polish (Weeks 21–24)

| ID | Task | Status | Dependencies | Worktree |
|----|------|--------|--------------|----------|
| T-39 | Dashboard analytics API (revenue, patients, occupancy) | ⬜ | T-12, T-20 | feature/T-39-analytics-api |
| T-40 | Dashboard analytics UI (charts + KPIs) | ⬜ | T-39 | feature/T-40-analytics-ui |
| T-41 | Revenue reports + PDF export | ⬜ | T-39, T-29 | feature/T-41-reports |
| T-42 | Patient retention metrics | ⬜ | T-39 | feature/T-42-retention |
| T-43 | i18n — full Albanian translation | ⬜ | T-07 | feature/T-43-i18n |
| T-44 | Helm charts for K3s production deployment | ⬜ | T-08 | feature/T-44-helm |
| T-45 | Monitoring (Prometheus + Grafana) setup | ⬜ | T-44 | feature/T-45-monitoring |
| T-46 | Database backup strategy (pg_dump + WAL) | ⬜ | T-02 | feature/T-46-backups |
| T-47 | Security hardening & penetration testing | ⬜ | all | feature/T-47-security |
| T-48 | End-to-end testing (Playwright) | ⬜ | all | feature/T-48-e2e |

---

## Parallel Execution Map

These tasks CAN be worked on simultaneously in separate worktrees:

**Phase 0 Parallel Groups:**
- Group A: T-02 (database) + T-03 (backend scaffold) + T-06 (frontend scaffold) — independent foundations
- Group B: T-05 (auth backend) + T-06 (frontend scaffold) — if T-03 is done

**Phase 1 Parallel Groups:**
- Group A: T-10 (therapists) + T-11 (treatment types) — independent CRUDs
- Group B: T-13 (calendar UI) can start while T-12 (appointments API) is in progress — use mock data
- Group C: T-17 (recurring) + T-18 (realtime) + T-19 (views) — after T-14 is done

**Phase 2 Parallel Groups:**
- Group A: T-20 (patients API) + T-26 (walk-in API) — can share session if different files
- Group B: T-23 (notes) + T-24 (packages) + T-25 (documents) — independent features on patient

**Phase 3–5:** Mostly sequential within phase, but phases can overlap.
