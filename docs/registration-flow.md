# Registration Flow Updates

## Migration notes
Add the following columns to the `Users` table:

- `RegistrationStatus` (int, default 0 = NotRegistered)
- `EmailVerificationCode` (string, nullable)
- `EmailVerificationCodeExpiresAtUtc` (datetime, nullable)
- `EmailVerificationAttempts` (int, default 0)
- `EmailVerificationLockedUntilUtc` (datetime, nullable)
- `ResendCount` (int, default 0)
- `LastResendAtUtc` (datetime, nullable)

## Example response payloads

### POST /api/users/precheck
**Success (needsRegistration)**
```json
{
  "status": "needsRegistration",
  "userId": 42,
  "fullName": "Ada Lovelace",
  "email": "ada@example.com",
  "phone": "+1-555-0100",
  "departmentName": "Engineering"
}
```

**Success (valid)**
```json
{
  "status": "valid",
  "userId": 42,
  "fullName": "Ada Lovelace",
  "email": "ada@example.com",
  "phone": "+1-555-0100",
  "departmentName": "Engineering"
}
```

### PUT /api/users/{userId}/registration
**Success**
```json
{
  "userId": 42,
  "status": "PendingVerification",
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
