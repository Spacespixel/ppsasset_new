# PPS Asset Production Deployment Instructions

## Pre-Deployment Checklist ✅
- [x] Project built successfully (Release configuration)
- [x] Published to `/publish/` directory  
- [x] PowerShell deployment script verified
- [x] Production configurations prepared
- [x] Database setup script generated

## Windows VPS Connection Details
- **VPS IP**: 103.13.231.222
- **RDP Port**: 33899
- **Target Directory**: `C:\Production\web\ppsasset.com`

## Step 1: Connect to Windows VPS
```bash
# From your local machine, connect via RDP:
# Windows: mstsc /v:103.13.231.222:33899
# Mac: Use Microsoft Remote Desktop app with:
#   PC name: 103.13.231.222:33899
```

## Step 2: Copy Published Files to VPS
You'll need to copy the entire `/publish/` directory to the VPS. Options:

### Option A: Direct Copy via RDP
1. Connect to VPS via RDP
2. Copy the entire `publish/` folder from your local machine
3. Place it in `C:\Production\web\ppsasset.com\staging\`

### Option B: Using SCP/SFTP (if available)
```bash
# If SSH/SCP is available on the Windows VPS:
scp -P 33899 -r publish/ Administrator@103.13.231.222:C:/Production/web/ppsasset.com/staging/
```

## Step 3: Execute Deployment Script on VPS
Once connected to the Windows VPS via RDP, open PowerShell as Administrator and run:

```powershell
# Navigate to the deployment directory
cd "C:\Production\web\ppsasset.com"

# Execute the deployment script
.\deploy-to-vps.ps1 -DatabasePassword "YOUR_SECURE_DATABASE_PASSWORD"
```

### Alternative Deployment with Custom Settings
```powershell
# For custom configuration:
.\deploy-to-vps.ps1 `
  -DatabasePassword "YOUR_SECURE_DATABASE_PASSWORD" `
  -DomainName "ppsasset.com" `
  -HttpPort "80" `
  -HttpsPort "443" `
  -DatabaseName "ppsasset_production_db" `
  -DatabaseUser "ppsasset_user"
```

## Step 4: Database Setup
After the deployment script completes, you'll need to setup the MySQL database:

```powershell
# The script will generate: setup_database.sql
# Run this in MySQL command line:
mysql -u root -p < "C:\Production\web\ppsasset.com\setup_database.sql"
```

## Step 5: Copy Application Files
```powershell
# Copy your published files to the wwwroot directory:
Copy-Item "C:\Production\web\ppsasset.com\staging\publish\*" "C:\Production\web\ppsasset.com\wwwroot\" -Recurse -Force

# Ensure proper permissions:
icacls "C:\Production\web\ppsasset.com\wwwroot" /grant "IIS_IUSRS:(OI)(CI)F" /T
```

## Step 6: Verify Deployment
```powershell
# Check IIS services status:
Get-Website -Name "ppsasset.com"
Get-IISAppPool -Name "ppsasset.com_AppPool"

# Test the website:
Invoke-WebRequest -Uri "http://localhost" -UseBasicParsing
```

## Expected Results After Deployment

### ✅ Safety Features Active
- Complete resource isolation from other sites
- Memory limit: 1GB, CPU limit: 50%
- Separate directory structure and permissions
- Dedicated database with minimal permissions
- Independent logging system

### ✅ Multi-Tenant Benefits
- Zero impact on existing websites
- Automatic backup and rollback capability
- Standard ports 80/443 for production access
- Professional domain configuration

### ✅ Production Configuration
- **Website**: `http://ppsasset.com` (after DNS setup)
- **HTTPS**: `https://ppsasset.com` (after SSL certificate)
- **Direct IP**: `http://103.13.231.222`
- **Database**: Isolated `ppsasset_production_db`
- **Logs**: `C:\Production\logs\ppsasset\`

## Post-Deployment Tasks

### 1. DNS Configuration
- Point `ppsasset.com` A record to `103.13.231.222`
- Website will respond on standard ports 80/443

### 2. SSL Certificate Setup
- Recommended: Let's Encrypt via win-acme
- Configure for domain `ppsasset.com`

### 3. Performance Monitoring
- Monitor application pool resource usage
- Check logs: `C:\Production\logs\ppsasset\`
- Verify existing sites remain operational

## Emergency Rollback
If issues occur, run these commands in PowerShell:

```powershell
Remove-Website -Name "ppsasset.com"
Remove-WebAppPool -Name "ppsasset.com_AppPool"
Remove-Item "C:\Production\web\ppsasset.com" -Recurse -Force

# Verify existing sites are still operational:
Get-Website
```

## Support & Troubleshooting

### Common Issues:
1. **Port Conflicts**: The script automatically detects and prevents conflicts
2. **Database Access**: Verify MySQL is running and credentials are correct
3. **File Permissions**: Run the icacls commands to fix permission issues
4. **Application Pool**: Check Event Viewer for any startup errors

### Health Check Endpoints:
- `/ServiceStatus` - Application health check
- `/MigrateDatabase` - Database migration (development only)

The deployment is designed to be completely safe for multi-tenant hosting - your existing websites will remain completely unaffected during and after the deployment process.