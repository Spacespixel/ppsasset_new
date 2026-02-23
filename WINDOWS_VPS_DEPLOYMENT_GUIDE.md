# Windows VPS Deployment Guide for PPS Asset
**ASP.NET Core 8.0 Real Estate Website Deployment**

## VPS Server Details
**SAVE THESE DETAILS SECURELY**
- **Hostname**: vps1520.vpshispeed.net
- **Main IP**: 103.13.231.222
- **Remote Port**: 33899
- **Username**: administrator
- **Platform**: Windows Server (VPS)

---

## 1. Remote Desktop Connection (RDP)

### Windows RDP Connection
1. **Open Remote Desktop Connection**:
   - Press `Win + R`, type `mstsc`, press Enter
   
2. **Connection Settings**:
   - **Computer**: `103.13.231.222:33899`
   - **User name**: `administrator`
   - Click "Show Options" for advanced settings
   - **Save credentials** for future use

3. **Advanced RDP Settings**:
   - Go to "Advanced" tab
   - Enable "Connect from anywhere"
   - Set connection quality based on your internet

### macOS/Linux RDP Connection
```bash
# Install Microsoft Remote Desktop from App Store (macOS)
# Or use rdesktop/freerdp on Linux

# Example with freerdp (Linux):
xfreerdp /v:103.13.231.222:33899 /u:administrator /cert-ignore
```

### File Transfer Options
1. **RDP Drive Mapping** (Recommended):
   - In RDP connection settings → Local Resources → More
   - Check "Drives" to map local drives to remote session

2. **Alternative Methods**:
   - FileZilla with SFTP (if enabled)
   - WinSCP for Windows users
   - Browser upload to cloud storage → download on VPS

---

## 2. IIS Installation and Configuration

### Install IIS with ASP.NET Core Support
Connect to your VPS via RDP, then run PowerShell as Administrator:

```powershell
# Enable IIS with all required features
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole, IIS-WebServer, IIS-CommonHttpFeatures, IIS-HttpErrors, IIS-HttpRedirect, IIS-ApplicationDevelopment, IIS-NetFxExtensibility45, IIS-HealthAndDiagnostics, IIS-HttpLogging, IIS-Security, IIS-RequestFiltering, IIS-Performance, IIS-WebServerManagementTools, IIS-ManagementConsole, IIS-IIS6ManagementCompatibility, IIS-Metabase, IIS-ASPNET45

# Verify IIS installation
Get-WindowsFeature -Name "IIS-*" | Where-Object {$_.InstallState -eq "Installed"}
```

### Alternative GUI Method:
1. **Server Manager** → Add Roles and Features
2. **Server Roles** → Web Server (IIS)
3. **Role Services**:
   - ✅ Web Server → Common HTTP Features (all)
   - ✅ Application Development → .NET Extensibility 4.8
   - ✅ Health and Diagnostics → HTTP Logging
   - ✅ Security → Request Filtering
   - ✅ Management Tools → IIS Management Console

---

## 3. .NET 8.0 Runtime Installation

### Download and Install .NET 8.0 Hosting Bundle
```powershell
# Download .NET 8.0 Hosting Bundle (includes runtime + IIS integration)
$url = "https://download.microsoft.com/download/8/4/8/848f86f2-5c34-4b5e-b044-02bc4a9d1e8e/dotnet-hosting-8.0.11-win.exe"
$output = "$env:TEMP\dotnet-hosting-8.0.11-win.exe"

# Download
Invoke-WebRequest -Uri $url -OutFile $output

# Install silently
Start-Process -FilePath $output -ArgumentList "/quiet" -Wait

# Restart IIS to recognize new modules
net stop was /y
net start w3svc

# Verify installation
dotnet --list-runtimes
dotnet --info
```

### Manual Download Method:
1. Visit: https://dotnet.microsoft.com/download/dotnet/8.0
2. Download "Hosting Bundle" (not just Runtime)
3. Run installer as Administrator
4. Restart IIS: `iisreset /restart`

---

## 4. MySQL Database Installation

### Download and Install MySQL Server
```powershell
# Download MySQL Installer
$mysqlUrl = "https://dev.mysql.com/get/Downloads/MySQLInstaller/mysql-installer-community-8.0.35.0.msi"
$mysqlOutput = "$env:TEMP\mysql-installer.msi"

Invoke-WebRequest -Uri $mysqlUrl -OutFile $mysqlOutput

# Install MySQL (will open GUI installer)
Start-Process -FilePath "msiexec.exe" -ArgumentList "/i $mysqlOutput" -Wait
```

### MySQL Configuration:
1. **Choose Setup Type**: "Server only" or "Custom"
2. **Configuration**:
   - Server Configuration Type: "Development Computer"
   - Port: `3306` (default)
   - Root Password: **CREATE STRONG PASSWORD**
3. **Authentication**: "Use Strong Password Encryption"
4. **Windows Service**: "Configure MySQL Server as Windows Service"

### Create Database and User:
```sql
-- Connect to MySQL as root
mysql -u root -p

-- Create database
CREATE DATABASE thericco CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Create application user
CREATE USER 'ppsasset'@'localhost' IDENTIFIED BY 'your_secure_password_here';

-- Grant privileges
GRANT ALL PRIVILEGES ON thericco.* TO 'ppsasset'@'localhost';
FLUSH PRIVILEGES;

-- Show databases to verify
SHOW DATABASES;
```

### Test MySQL Connection:
```powershell
# Test MySQL service
Get-Service -Name "MySQL*"

# Test connection
mysql -u root -p -e "SELECT VERSION();"
```

---

## 5. Website Deployment

### Create Website Directory
```powershell
# Create application directory
$appPath = "C:\inetpub\wwwroot\ppsasset"
New-Item -ItemType Directory -Path $appPath -Force

# Set proper permissions
icacls $appPath /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls $appPath /grant "IUSR:(OI)(CI)RX" /T
```

### Transfer Published Files
1. **Copy your `/publish/` folder contents** to `C:\inetpub\wwwroot\ppsasset\`
2. **File structure should be**:
   ```
   C:\inetpub\wwwroot\ppsasset\
   ├── PPSAsset.dll
   ├── PPSAsset.deps.json
   ├── PPSAsset.runtimeconfig.json
   ├── web.config
   ├── wwwroot/
   ├── appsettings.json
   └── [other files from publish folder]
   ```

### Update Configuration Files
Edit `C:\inetpub\wwwroot\ppsasset\appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=thericco;Uid=ppsasset;Pwd=your_secure_password_here;CharSet=utf8mb4;"
  },
  "RecaptchaSettings": {
    "SiteKey": "6LfPawssAAAAAKdFROS6J-c9ftvQEwl9500WGrVi",
    "SecretKey": "6LfPawssAAAAALXf-mXU9dG7t_JbRLWDlo5C6S2j"
  }
}
```

### Create IIS Website
```powershell
# Remove Default Web Site
Remove-Website -Name "Default Web Site"

# Create new website
New-Website -Name "PPSAsset" -PhysicalPath "C:\inetpub\wwwroot\ppsasset" -Port 80

# Create HTTPS binding for port 443 (SSL)
New-WebBinding -Name "PPSAsset" -Protocol "https" -Port 443

# Verify website
Get-Website -Name "PPSAsset"
```

### Configure Application Pool
```powershell
# Create dedicated application pool
New-WebAppPool -Name "PPSAssetPool"

# Configure for .NET Core
Set-ItemProperty -Path "IIS:\AppPools\PPSAssetPool" -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty -Path "IIS:\AppPools\PPSAssetPool" -Name "enable32BitAppOnWin64" -Value $false

# Assign to website
Set-ItemProperty -Path "IIS:\Sites\PPSAsset" -Name "applicationPool" -Value "PPSAssetPool"

# Start the application pool
Start-WebAppPool -Name "PPSAssetPool"
```

---

## 6. Database Migration and Setup

### Run Database Migration
Your application has a migration endpoint. Access it via:
1. **Local testing first**: `http://103.13.231.222/MigrateDatabase`
2. **Check application logs** in Event Viewer for any errors

### Manual Database Setup (if needed):
```sql
-- Connect to your database
mysql -u ppsasset -p thericco

-- Run your SQL scripts in order
SOURCE C:/path/to/your/sql/files/01_create_project_mapping_table.sql;
SOURCE C:/path/to/your/sql/files/05_add_feature_image_icon_columns.sql;
-- Continue with other SQL files...
```

---

## 7. Domain and SSL Configuration

### Domain Configuration
1. **DNS Settings** (at your domain registrar):
   ```
   Type: A Record
   Name: @
   Value: 103.13.231.222
   TTL: 300
   
   Type: A Record
   Name: www
   Value: 103.13.231.222
   TTL: 300
   ```

2. **Test DNS Propagation**:
   ```powershell
   nslookup yourdomain.com
   ping yourdomain.com
   ```

### SSL Certificate Installation

#### Option 1: Let's Encrypt (Free - Recommended)
```powershell
# Install win-acme for automated Let's Encrypt
$acmeUrl = "https://github.com/win-acme/win-acme/releases/latest/download/win-acme.v2.2.9.1701.x64.pluggable.zip"
$acmePath = "C:\win-acme"

# Download and extract
New-Item -ItemType Directory -Path $acmePath -Force
Invoke-WebRequest -Uri $acmeUrl -OutFile "$acmePath\win-acme.zip"
Expand-Archive -Path "$acmePath\win-acme.zip" -DestinationPath $acmePath

# Run win-acme (interactive setup)
cd $acmePath
.\wacs.exe
```

#### Option 2: Self-Signed Certificate (Testing Only)
```powershell
# Create self-signed certificate
$cert = New-SelfSignedCertificate -DnsName "yourdomain.com", "www.yourdomain.com" -CertStoreLocation "cert:\LocalMachine\My"

# Bind to IIS
$binding = Get-WebBinding -Name "PPSAsset" -Protocol "https"
$binding.AddSslCertificate($cert.GetCertHashString(), "my")
```

---

## 8. Security Configurations

### Windows Firewall Configuration
```powershell
# Allow HTTP and HTTPS traffic
New-NetFirewallRule -DisplayName "HTTP Traffic" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
New-NetFirewallRule -DisplayName "HTTPS Traffic" -Direction Inbound -Protocol TCP -LocalPort 443 -Action Allow
New-NetFirewallRule -DisplayName "MySQL" -Direction Inbound -Protocol TCP -LocalPort 3306 -Action Allow -RemoteAddress LocalSubnet

# Block unnecessary ports
New-NetFirewallRule -DisplayName "Block SSH" -Direction Inbound -Protocol TCP -LocalPort 22 -Action Block
New-NetFirewallRule -DisplayName "Block FTP" -Direction Inbound -Protocol TCP -LocalPort 21 -Action Block
```

### IIS Security Headers
Create `C:\inetpub\wwwroot\ppsasset\web.config`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\PPSAsset.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" />
      
      <httpProtocol>
        <customHeaders>
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="X-Frame-Options" value="DENY" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
        </customHeaders>
      </httpProtocol>
      
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
    </system.webServer>
  </location>
</configuration>
```

### MySQL Security
```sql
-- Remove anonymous users
DELETE FROM mysql.user WHERE User='';

-- Remove remote root login
DELETE FROM mysql.user WHERE User='root' AND Host NOT IN ('localhost', '127.0.0.1', '::1');

-- Remove test database
DROP DATABASE IF EXISTS test;

-- Reload privileges
FLUSH PRIVILEGES;
```

---

## 9. Performance Optimization

### IIS Compression
```powershell
# Enable compression
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic, IIS-HttpCompressionDynamic

# Configure compression in IIS Manager or via PowerShell
Set-WebConfigurationProperty -Filter "system.webServer/httpCompression" -Name "directory" -Value "%SystemDrive%\inetpub\temp\IIS Temporary Compressed Files"
Set-WebConfigurationProperty -Filter "system.webServer/httpCompression" -Name "doDynamicCompression" -Value "True"
Set-WebConfigurationProperty -Filter "system.webServer/httpCompression" -Name "doStaticCompression" -Value "True"
```

### Application Pool Optimization
```powershell
# Configure application pool for production
Set-ItemProperty -Path "IIS:\AppPools\PPSAssetPool" -Name "processModel.idleTimeout" -Value "00:00:00"
Set-ItemProperty -Path "IIS:\AppPools\PPSAssetPool" -Name "recycling.periodicRestart.time" -Value "1.05:00:00"
Set-ItemProperty -Path "IIS:\AppPools\PPSAssetPool" -Name "processModel.maxProcesses" -Value 1
```

### MySQL Optimization
Edit `C:\ProgramData\MySQL\MySQL Server 8.0\my.ini`:
```ini
[mysqld]
innodb_buffer_pool_size = 512M
query_cache_type = 1
query_cache_size = 128M
max_connections = 100
```

---

## 10. Monitoring and Maintenance

### Application Monitoring Setup
```powershell
# Create logs directory
New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\ppsasset\logs" -Force

# Setup log rotation (Task Scheduler)
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-Command 'Get-ChildItem C:\inetpub\wwwroot\ppsasset\logs\*.log | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-30)} | Remove-Item'"
$trigger = New-ScheduledTaskTrigger -Daily -At 2am
Register-ScheduledTask -TaskName "PPSAsset-LogCleanup" -Action $action -Trigger $trigger
```

### Health Check URLs
Add these to your monitoring:
- **Application Status**: `http://yourdomain.com`
- **Service Status**: `http://yourdomain.com/ServiceStatus`
- **Database Health**: Check via your hybrid service endpoint

### Backup Strategy
```powershell
# Database backup script
$backupPath = "C:\Backups\MySQL"
New-Item -ItemType Directory -Path $backupPath -Force

# Create backup script
$backupScript = @"
@echo off
set backup_date=%date:~-4,4%-%date:~-10,2%-%date:~-7,2%
mysqldump -u ppsasset -p[password] thericco > $backupPath\thericco_%backup_date%.sql
"@

$backupScript | Out-File -FilePath "C:\Scripts\backup_database.bat" -Encoding ASCII

# Schedule daily backups
$action = New-ScheduledTaskAction -Execute "C:\Scripts\backup_database.bat"
$trigger = New-ScheduledTaskTrigger -Daily -At 3am
Register-ScheduledTask -TaskName "PPSAsset-DatabaseBackup" -Action $action -Trigger $trigger
```

---

## 11. Testing and Verification

### Pre-Go-Live Checklist
- [ ] **IIS Website responds** on port 80
- [ ] **Database connection successful**
- [ ] **Static files loading** (CSS, JS, images)
- [ ] **Project pages accessible**
- [ ] **Contact form submission works**
- [ ] **MySQL service running**
- [ ] **SSL certificate installed**
- [ ] **Domain DNS propagated**
- [ ] **Security headers present**
- [ ] **Application logs created**

### Testing Commands
```powershell
# Test website response
Invoke-WebRequest -Uri "http://103.13.231.222" -UseBasicParsing

# Test database connectivity
mysql -u ppsasset -p -e "SELECT COUNT(*) FROM thericco.projects;"

# Test SSL (after domain setup)
Invoke-WebRequest -Uri "https://yourdomain.com" -UseBasicParsing

# Check IIS logs
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\*.log" | Select-Object -Last 10
```

### Common Troubleshooting
1. **500 Internal Server Error**: Check Event Viewer → Application and Services Logs
2. **Database connection failed**: Verify MySQL service and connection string
3. **Static files not loading**: Check folder permissions and IIS static content handling
4. **SSL issues**: Verify certificate binding in IIS Manager

---

## 12. Post-Deployment Tasks

### Performance Testing
```bash
# Use tools like:
# - Google PageSpeed Insights
# - GTmetrix
# - WebPageTest.org
```

### SEO Configuration
- **Google Analytics**: Add tracking code if not present
- **Google Search Console**: Submit sitemap
- **Robots.txt**: Verify accessibility
- **Schema.org markup**: Test with Google Rich Results Test

### Ongoing Maintenance
- **Weekly**: Check application and IIS logs
- **Monthly**: Review MySQL slow query log
- **Quarterly**: Update .NET runtime and MySQL
- **As needed**: Certificate renewal (automatic with Let's Encrypt)

---

## Emergency Contacts and Documentation
- **VPS Provider Support**: Check vpshispeed.net support channels
- **Application Logs**: `C:\inetpub\wwwroot\ppsasset\logs\`
- **IIS Logs**: `C:\inetpub\logs\LogFiles\W3SVC1\`
- **MySQL Error Log**: `C:\ProgramData\MySQL\MySQL Server 8.0\Data\`

**IMPORTANT**: Always test changes in a staging environment when possible. Keep your database and application backups current.

---

*Generated: November 29, 2025*  
*Target Framework: ASP.NET Core 8.0*  
*Database: MySQL 8.0*  
*Platform: Windows Server IIS*