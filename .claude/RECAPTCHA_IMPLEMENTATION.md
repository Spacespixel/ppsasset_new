# Google reCAPTCHA v3 Implementation Guide

## Overview
This document describes the complete implementation of Google reCAPTCHA v3 for the registration form on the PPS Asset website. The reCAPTCHA feature:

- **Prevents bot submissions** using Google's invisible v3 verification
- **Disables the submit button** until captcha verification is successful
- **Shows verification status** with visual feedback
- **Validates on both client and server** for maximum security
- **Automatically refreshes tokens** every 2 minutes to keep them fresh

## Features Implemented

### 1. **Client-Side Protection**
- ✅ Submit button disabled until reCAPTCHA verification succeeds
- ✅ Visual feedback showing "ตรวจสอบ..." (Checking...) while validating
- ✅ Shows "ยืนยันแล้ว ✓" (Verified ✓) when successful
- ✅ Automatic token refresh every 2 minutes
- ✅ Loading spinner animation while processing submission
- ✅ Prevents double-submission with button disabled state

### 2. **Server-Side Validation**
- ✅ Backend verification of reCAPTCHA token against Google API
- ✅ Score-based validation (minimum 0.5 score required)
- ✅ Automatic logging of failed validations
- ✅ Graceful fallback if secret key is not configured (development mode)

### 3. **User Experience**
- ✅ Smooth animation transitions
- ✅ Clear visual indicators for button state
- ✅ Thai language feedback messages
- ✅ Disabled button appearance (gray with reduced opacity)
- ✅ Hover effects for enabled button

## Configuration

### Step 1: Get reCAPTCHA Keys
1. Visit [Google reCAPTCHA Admin Console](https://www.google.com/recaptcha/admin)
2. Create a new reCAPTCHA v3 project
3. Choose **reCAPTCHA v3** (not v2)
4. Add your domain(s):
   - `ppsasset.com`
   - `www.ppsasset.com`
   - `localhost:5000` (for development)
5. Copy the **Site Key** and **Secret Key**

### Step 2: Update appsettings.json
Add your reCAPTCHA keys to `appsettings.json`:

```json
{
  "RecaptchaSettings": {
    "SiteKey": "YOUR_RECAPTCHA_V3_SITE_KEY",
    "SecretKey": "YOUR_RECAPTCHA_V3_SECRET_KEY"
  }
}
```

⚠️ **Important**: Never commit the secret key to version control. Use:
- Environment variables in production
- User secrets in development
- Azure Key Vault or similar for sensitive data

### Step 3: Development Environment (Optional)
For local development without real reCAPTCHA keys, the system gracefully allows submissions:

```csharp
// In RecaptchaService.cs
if (string.IsNullOrEmpty(secretKey))
{
    // If secret key is not configured, allow submission for development
    return true;
}
```

## File Changes Summary

### New Files Created
1. **`Services/RecaptchaService.cs`** - Backend verification service
   - `IRecaptchaService` interface
   - `RecaptchaService` implementation
   - Communicates with Google reCAPTCHA API
   - Score-based validation (threshold: 0.5)

### Modified Files

#### 1. **`appsettings.json`**
- Added `RecaptchaSettings` section with Site Key and Secret Key placeholders

#### 2. **`Models/RegistrationInputModel.cs`**
- Added `RecaptchaToken` property to receive the token from frontend

#### 3. **`Program.cs`**
- Registered `IRecaptchaService` with HttpClient for API communication
- Line: `builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();`

#### 4. **`Controllers/HomeController.cs`**
- Injected `IRecaptchaService` into constructor
- Added reCAPTCHA token verification in `RegisterProject` action
- Pass `RecaptchaSiteKey` from ViewBag to view for client-side script

#### 5. **`Views/Home/Project.cshtml`**
- Added reCAPTCHA v3 script tag
- Added hidden input field for token: `<input type="hidden" name="RecaptchaToken" id="recaptchaToken" />`
- Added JavaScript to execute reCAPTCHA and manage token lifecycle
- Updated submit button to be disabled initially (`disabled` attribute)
- Added CSS animations and styling for button states
- Modified form submission handler to check reCAPTCHA verification

## JavaScript Implementation Details

### Token Generation Flow
```
Page Load
  ↓
executeRecaptcha() called
  ↓
grecaptcha.execute() generates token
  ↓
Token stored in hidden input
  ↓
Submit button enabled
  ↓
Status message shows "ยืนยันแล้ว ✓"
```

### Token Refresh Flow
```
Initial token generation
  ↓
setInterval(executeRecaptcha, 120000) // Every 2 minutes
  ↓
Token automatically refreshed
  ↓
Button remains enabled (no visual change)
```

### Form Submission Flow
```
User clicks submit
  ↓
Check reCAPTCHA verified && token exists
  ↓
If verified: Show spinner, disable button, submit AJAX
  ↓
Server validates token with Google API
  ↓
If valid: Save registration, show success modal
  ↓
If invalid: Show error message, enable button
```

## CSS Styling

### Button States
```css
/* Disabled State (waiting for reCAPTCHA) */
#registrationSubmitBtn:disabled {
    opacity: 0.6;
    background-color: #999 !important;
    cursor: not-allowed;
}

/* Enabled State (reCAPTCHA verified) */
#registrationSubmitBtn:not(:disabled) {
    opacity: 1;
    transition: all 0.3s ease;
}

/* Hover State (reCAPTCHA verified) */
#registrationSubmitBtn:hover:not(:disabled) {
    opacity: 0.9;
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
```

### Spinner Animation
```css
@@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}

.spinner-icon {
    width: 16px;
    height: 16px;
    border: 2px solid rgba(255, 255, 255, 0.3);
    border-top: 2px solid white;
    border-radius: 50%;
    animation: spin 0.8s linear infinite;
}
```

## Server-Side Validation

### reCAPTCHA Response Validation
```csharp
// In RecaptchaService.VerifyTokenAsync()
// Calls: https://www.google.com/recaptcha/api/siteverify
// Returns: { success: bool, score: 0.0-1.0, action: string, ... }
//
// Validation checks:
// 1. success === true
// 2. score >= 0.5 (configurable threshold)
// 3. action === "register"
```

### Score Interpretation
- **Score 0.9+**: Very likely legitimate user
- **Score 0.5-0.9**: Probably legitimate, may have some bot-like behavior
- **Score <0.5**: Likely bot or suspicious behavior
- **Threshold set to 0.5**: Balance between security and user experience

### Error Handling
```csharp
// If reCAPTCHA validation fails:
if (!recaptchaValid)
{
    // Log the failure
    _logger.LogWarning("reCAPTCHA validation failed for project {ProjectId}");

    // Return appropriate error response
    if (isAjaxRequest)
    {
        return Json(new {
            success = false,
            message = "reCAPTCHA ยืนยันไม่สำเร็จ กรุณาลองใหม่อีกครั้ง"
        });
    }
}
```

## Testing Guide

### Local Testing (Development)
1. **Without reCAPTCHA keys**: Set `RecaptchaSettings:SecretKey` to empty string
   - Button will be disabled initially
   - Will be auto-enabled after ~1 second (simulated verification)
   - Form submission will succeed without actual Google API call

2. **With reCAPTCHA keys**: Add valid keys to `appsettings.json`
   - Real verification with Google API
   - Actual score calculation
   - Full security validation

### Testing Steps
1. Navigate to a project page with registration form
2. Observe:
   - Button is initially disabled (gray appearance)
   - "Google reCAPTCHA - ตรวจสอบ..." message appears
   - After ~1-2 seconds: Status changes to "ยืนยันแล้ว ✓"
   - Button becomes enabled (normal color)
   - Hover effects work on enabled button

3. Fill in form fields
4. Click submit button
5. Observe:
   - Button shows spinner
   - Button is disabled during submission
   - After response: Button returns to normal state

### Testing with Real Keys
```bash
# 1. Restart application
dotnet run --environment Production

# 2. Test submission with valid data
# Expected: Submission succeeds with reCAPTCHA validation

# 3. Test with bot-like behavior (rapid submissions)
# Expected: Some submissions may be rejected with low reCAPTCHA score
```

## Security Considerations

### ✅ Security Features
- **Invisible verification**: Users don't need to click anything
- **Token validation**: Server-side verification against Google API
- **Score-based assessment**: Advanced bot detection
- **Automatic token refresh**: Prevents token reuse attacks
- **AJAX headers check**: Prevents CSRF attacks
- **Server-side validation**: Not dependent on client-side checks

### ⚠️ Security Best Practices
1. **Never expose secret key** in client-side code
2. **Always validate on server** - don't trust client-side validation alone
3. **Store keys securely** - use environment variables or key vaults
4. **Monitor reCAPTCHA score** - track in analytics for bot detection patterns
5. **Refresh tokens regularly** - implemented with 2-minute interval
6. **Rate limiting** - consider implementing additional rate limiting if needed

## Monitoring & Analytics

### What to Track
1. **reCAPTCHA scores**: Monitor distribution of scores
2. **Rejection rate**: Track percentage of failed verifications
3. **Token refresh**: Ensure tokens are being refreshed successfully
4. **Submission success rate**: Monitor overall registration success

### Google reCAPTCHA Dashboard
Visit [Google reCAPTCHA Admin Console](https://www.google.com/recaptcha/admin) to:
- View analytics and traffic patterns
- Check score distribution
- Monitor bot detection effectiveness
- See blocked requests

## Troubleshooting

### Button Remains Disabled
**Issue**: Submit button stays gray and disabled
**Causes**:
- reCAPTCHA script failed to load
- Site key not configured correctly
- JavaScript error in console
**Solutions**:
1. Check browser console for errors (F12)
2. Verify Site Key is correct in `appsettings.json`
3. Check that reCAPTCHA domain is configured in Google admin console
4. Wait 5-10 minutes after domain addition (propagation delay)

### Submit Button Gets Disabled After Verification
**Issue**: Button enabled briefly then disabled again
**Causes**: Server-side validation failed
**Solutions**:
1. Check server logs for reCAPTCHA validation errors
2. Verify Secret Key is correct
3. Check Score threshold (default 0.5)
4. Check internet connectivity for API call

### reCAPTCHA Script Error in Console
**Issue**: `grecaptcha is not defined` or similar
**Causes**: Script not loaded or wrong site key
**Solutions**:
1. Verify Site Key is passed correctly from controller
2. Check network tab to see if script loads
3. Ensure Site Key is for v3, not v2

### Form Submission Fails with "reCAPTCHA ยืนยันไม่สำเร็จ"
**Issue**: Registration fails even though button is enabled
**Causes**:
- Token expired (>2 min old but not refreshed)
- Server-side validation failed
- Secret key incorrect
**Solutions**:
1. Wait 2 seconds and retry (ensure fresh token)
2. Check server logs for detailed error
3. Verify reCAPTCHA keys are correct

## Future Enhancements

### Possible Improvements
1. **Admin dashboard**: Display reCAPTCHA score analytics
2. **Dynamic threshold**: Adjust score threshold based on traffic patterns
3. **Rate limiting**: Add account-based or IP-based rate limits
4. **Multi-language**: Support more languages for status messages
5. **Fallback CAPTCHA**: Add human-readable CAPTCHA for very low scores
6. **Advanced monitoring**: Log and track bot patterns
7. **Custom scoring**: Implement additional validation factors

## References
- [Google reCAPTCHA Documentation](https://developers.google.com/recaptcha/docs/v3)
- [reCAPTCHA Admin Console](https://www.google.com/recaptcha/admin)
- [reCAPTCHA Implementation Guide](https://developers.google.com/recaptcha/docs/v3/integrate)

## Summary

The reCAPTCHA v3 implementation provides:
- ✅ Invisible bot protection
- ✅ User-friendly experience (no manual CAPTCHA clicking)
- ✅ Strong server-side validation
- ✅ Clear visual feedback
- ✅ Automatic token management
- ✅ Production-ready security

The registration form is now protected against automated bot submissions while maintaining a smooth user experience for legitimate users.
