# T-05: JWT Auth System (Register, Login, Refresh)

## Status: ⬜ Not Started
## Phase: 0 — Foundation
## Dependencies: T-04
## Agents: backend-developer, test-engineer

---

## Objective
Implement complete JWT authentication with registration, login, token refresh, and role-based authorization.

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | /api/v1/auth/register | Owner only | Register new staff user for the tenant |
| POST | /api/v1/auth/login | Public | Login with email + password |
| POST | /api/v1/auth/refresh | Public | Exchange refresh token for new access token |
| POST | /api/v1/auth/logout | Authenticated | Revoke refresh token |
| GET | /api/v1/auth/me | Authenticated | Get current user profile |
| PUT | /api/v1/auth/me | Authenticated | Update own profile |
| PUT | /api/v1/auth/change-password | Authenticated | Change own password |

## Implementation Details

### JWT Token Structure
```json
{
  "sub": "user-uuid",
  "tenant_id": "tenant-uuid",
  "email": "user@clinic.al",
  "role": "therapist",
  "is_therapist": true,
  "iat": 1234567890,
  "exp": 1234568790
}
```

### Login Flow
1. Receive `{ email, password }` → validate with FluentValidation
2. Find user by email within tenant (use tenant from request origin or explicit tenant_id)
3. Verify password with bcrypt
4. Generate JWT access token (15 min expiry)
5. Generate random refresh token, hash it, store in `refresh_tokens`
6. Return `{ accessToken, refreshToken, expiresAt, user }`

### Refresh Flow
1. Receive `{ refreshToken }` → hash and find in DB
2. Verify not expired and not revoked
3. Revoke old refresh token (prevent reuse)
4. Generate new access + refresh token pair
5. Return new tokens

### Registration Flow (Owner/Admin only)
1. Validate: email unique within tenant, role is valid
2. Hash password with bcrypt (cost 12)
3. Create user record with tenant_id from JWT
4. Return created user (without password_hash)

## Steps

- [ ] 1. Create Domain: `User` entity, `Role` enum, `RefreshToken` entity
- [ ] 2. Create Application layer:
  - Commands: `LoginCommand`, `RegisterCommand`, `RefreshTokenCommand`, `LogoutCommand`, `ChangePasswordCommand`
  - Queries: `GetCurrentUserQuery`
  - DTOs: `AuthResponse`, `UserDto`, `LoginRequest`, `RegisterRequest`
  - Validators: `LoginValidator`, `RegisterValidator`, `ChangePasswordValidator`
- [ ] 3. Create Infrastructure:
  - `JwtService` — token generation and validation
  - `PasswordService` — bcrypt hash and verify
  - `AuthRepository` — refresh token storage
- [ ] 4. Create Api:
  - `AuthController` with all endpoints
  - `JwtMiddleware` — extract and validate token, set HttpContext.User
  - `TenantMiddleware` — extract tenant_id from JWT, set on DbContext
- [ ] 5. Configure JWT in `Program.cs` with validation parameters
- [ ] 6. Write unit tests:
  - Login with correct password → returns tokens
  - Login with wrong password → 401
  - Login with inactive user → 401
  - Refresh with valid token → new tokens
  - Refresh with expired token → 401
  - Refresh with revoked token → 401
  - Register duplicate email → 409
  - Register with insufficient role → 403
- [ ] 7. Write integration tests:
  - Full login → access protected endpoint → refresh → access again flow
  - Multi-tenant isolation: User from Tenant A cannot login via Tenant B

## Verification
- `dotnet build` — no errors
- `dotnet test` — all auth tests pass
- Manual test: login via curl/Postman, use token to access /auth/me
- Verify JWT contains correct claims including tenant_id

## Summary of Changes
<!-- Filled in after completion -->
