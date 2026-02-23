# reCAPTCHA v3 Quick Start Setup

## ⚡ 5-Minute Setup

### Step 1: Get reCAPTCHA Keys (2 minutes)
1. Go to https://www.google.com/recaptcha/admin
2. Click **+ Create** button
3. Enter:
   - **Label**: `PPS Asset Registration`
   - **reCAPTCHA type**: Select **reCAPTCHA v3**
   - **Domains**:
     - `ppsasset.com`
     - `www.ppsasset.com`
     - `localhost:5000` (for local testing)
4. Click **Create**
5. Copy your **Site Key** and **Secret Key**

### Step 2: Add Keys to Configuration (1 minute)

#### Option A: Local Development (Fastest)
Edit `appsettings.json`:
```json
{
  "RecaptchaSettings": {
    "SiteKey": "YOUR_SITE_KEY_HERE",
    "SecretKey": "YOUR_SECRET_KEY_HERE"
  }
}
```

#### Option B: Environment Variables (Secure for Production)
```bash
# Linux/Mac
export RecaptchaSettings__SiteKey="YOUR_SITE_KEY"
export RecaptchaSettings__SecretKey="YOUR_SECRET_KEY"

# Windows
set RecaptchaSettings__SiteKey=YOUR_SITE_KEY
set RecaptchaSettings__SecretKey=YOUR_SECRET_KEY
```

#### Option C: User Secrets (Best for Development)
```bash
dotnet user-secrets set "RecaptchaSettings:SiteKey" "YOUR_SITE_KEY"
dotnet user-secrets set "RecaptchaSettings:SecretKey" "YOUR_SECRET_KEY"
```

### Step 3: Test It! (2 minutes)
```bash
dotnet run
```

Navigate to http://localhost:5000/Project/ricco-residence-hathairat

You should see:
1. **Submit button is gray and disabled** with text "ลงทะเบียน"
2. **Below form**: "Google reCAPTCHA - ตรวจสอบ..."
3. **After 1-2 seconds**: Message changes to "ยืนยันแล้ว ✓"
4. **Submit button becomes enabled** (normal color)
5. **Hover on button**: Smooth lift effect

## How It Works

```
User visits page
         ↓
reCAPTCHA script loads
         ↓
Token generated in background
         ↓
Submit button becomes ENABLED
         ↓
User fills form + clicks submit
         ↓
AJAX sends form + token
         ↓
Server validates token with Google
         ↓
Server validates score (must be ≥0.5)
         ↓
If valid: Save registration + show success
If invalid: Show error message
```

## What's Protected

✅ **Bot Protection**: Invisible verification prevents automated submissions
✅ **Spam Prevention**: Only legitimate users can submit
✅ **Score-Based**: Google AI assesses user legitimacy (0.0-1.0 scale)
✅ **No User Friction**: Unlike v2, no CAPTCHA puzzle to solve
✅ **Server Validation**: Double-checked on backend

## File Changes Made

| File | Change |
|------|--------|
| `appsettings.json` | Added `RecaptchaSettings` config |
| `Program.cs` | Registered `IRecaptchaService` |
| `HomeController.cs` | Added reCAPTCHA validation + service injection |
| `Models/RegistrationInputModel.cs` | Added `RecaptchaToken` property |
| `Views/Home/Project.cshtml` | Added script, token input, styling |
| `Services/RecaptchaService.cs` | **NEW** - Backend validation service |

## Testing Without Real Keys

If you don't have reCAPTCHA keys yet:
1. Don't add anything to `appsettings.json`
2. Run the app
3. Button will auto-enable after ~1 second (fallback mode)
4. Form will submit successfully

This is perfect for development before you have production keys!

## Security Notes

⚠️ **DO NOT** commit secret key to Git
- Use environment variables in production
- Use user secrets in development
- Use Azure Key Vault for enterprise

✅ **Good**: Server validates all tokens
✅ **Good**: Automatic token refresh (2 minutes)
✅ **Good**: Score-based bot detection
✅ **Good**: Clear error messages

## Verify It's Working

### In Browser
1. Open DevTools (F12)
2. Go to **Network** tab
3. Submit the form
4. Look for request to `RegisterProject`
5. Check **Response** - should see `"success": true`

### In Server Logs
```
info: HomeController: Registered enquiry for project ricco-residence-hathairat
```

### In Google reCAPTCHA Admin Console
1. Go to https://www.google.com/recaptcha/admin
2. Select your project
3. See:
   - Traffic chart
   - Score distribution
   - Blocked requests

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Button stays disabled | Check `RecaptchaSettings:SiteKey` in config |
| Submit fails with reCAPTCHA error | Check `RecaptchaSettings:SecretKey` is correct |
| Nothing appears in reCAPTCHA console | May take 5-10 min to propagate |
| `grecaptcha is not defined` error | Verify Site Key is passed from controller |

## Next Steps

After basic setup:
1. ✅ Test with real keys
2. ✅ Monitor reCAPTCHA dashboard
3. ✅ Adjust score threshold if needed (default 0.5)
4. ✅ Add analytics tracking
5. ✅ Deploy to production

## Support

For issues:
1. Check `.claude/RECAPTCHA_IMPLEMENTATION.md` for detailed docs
2. Review browser console (F12) for errors
3. Check server logs for validation failures
4. Visit Google reCAPTCHA docs: https://developers.google.com/recaptcha/docs/v3

---

**That's it!** Your registration form is now protected by Google reCAPTCHA v3. 🎉
