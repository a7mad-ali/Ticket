# Registration & Login Flow Updates

## Migration notes
Add the following columns to the `Users` table:

- `PasswordHash` (string, nullable)
- `RegistrationStatus` (int, default 0 = NotRegistered; 1 = PendingEmailVerification; 2 = Verified)
- `FailedLoginAttempts` (int, default 0)
- `LoginLockedUntilUtc` (datetime, nullable)
- `EmailVerificationCode` (string, nullable)
- `EmailVerificationCodeExpiresAtUtc` (datetime, nullable)
- `EmailVerificationAttempts` (int, default 0)
- `EmailVerificationLockedUntilUtc` (datetime, nullable)
- `ResendCount` (int, default 0)
- `LastResendAtUtc` (datetime, nullable)

## Example response payloads

### POST /api/users/precheck
**Success (completeRegistration)**
```json
{
  "nextStep": "completeRegistration",
  "userId": 42,
  "message": "If the information matches our records, you can continue."
}
```

**Success (login)**
```json
{
  "nextStep": "login",
  "userId": 42,
  "message": "If the information matches our records, you can continue."
}
```

**Success (notFound)**
```json
{
  "nextStep": "notFound",
  "userId": null,
  "message": "If the information matches our records, you can continue."
}
```

### PUT /api/users/{userId}/registration
**Success**
```json
{
  "userId": 42,
  "status": "PendingEmailVerification",
  "email": "ada@example.com",
  "verificationExpiresAtUtc": "2024-06-01T12:45:00Z"
}
```

**Error (email in use)**
```json
{
  "code": "EMAIL_IN_USE",
  "message": "Email already registered."
}
```

### POST /api/users/{userId}/resend-email-code
**Success**
```json
{
  "userId": 42,
  "status": "PendingEmailVerification",
  "email": "ada@example.com",
  "verificationExpiresAtUtc": "2024-06-01T12:45:00Z"
}
```

**Error (resend limit)**
```json
{
  "code": "RESEND_LIMIT",
  "message": "Resend limit reached. Try again later."
}
```

### POST /api/users/{userId}/verify-email
**Success**
```json
{
  "verified": true,
  "errorCode": null
}
```

**Error (expired or locked)**
```json
{
  "code": "OTP_EXPIRED",
  "message": "Invalid or expired verification code."
}
```

### POST /api/auth/login
**Success**
```json
{
  "accessToken": "<jwt>",
  "expiresAtUtc": "2024-06-01T13:00:00Z",
  "tokenType": "Bearer",
  "refreshToken": null
}
```

**Error (email not verified)**
```json
{
  "code": "EMAIL_NOT_VERIFIED",
  "message": "Email is not verified."
}
```
