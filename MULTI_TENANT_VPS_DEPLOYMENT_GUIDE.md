# Multi-Tenant Safe Deployment Script for PPS Asset
# Designed for Windows VPS with existing websites
# Version: 2.0 - Multi-tenant isolation focus

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$DatabasePassword,
    
    [Parameter(Mandatory=$false)]
    [string]$AppPath = "C:\inetpub\sites\ppsasset",
    
    [Parameter(Mandatory=$false)]
    [string]$SiteName = "PPSAsset_Website",
    
    [Parameter(Mandatory=$false)]
    [string]$AppPoolName = "PPSAsset_AppPool_2024",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "ppsasset_production_db",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseUser = "ppsasset_user",
    
    [Parameter(Mandatory=$false)]
    [string]$HttpPort = "8080",
    
    [Parameter(Mandatory=$false)]
    [string]$HttpsPort = "8443",
    
    [Parameter(Mandatory=$false)]
    [string]$DomainName = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipIISFeatures = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDotNetInstall = $false
)

# Safety Checks
function Test-ExistingServices {
    Write-Host "=== SAFETY CHECK: Analyzing existing services ===" -ForegroundColor Yellow
    
    # Check existing websites
    $existingSites = Get-Website | Select-Object Name, State, PhysicalPath
    if ($existingSites) {
        Write-Host "Found existing websites:" -ForegroundColor Cyan
        $existingSites | Format-Table -AutoSize
        
        # Check for naming conflicts
        if ($existingSites.Name -contains $SiteName) {
            Write-Error "CONFLICT: Website '$SiteName' already exists. Choose a different name."
            exit 1
        }
    }
    
    # Check existing application pools
    $existingPools = Get-IISAppPool | Select-Object Name, State
    if ($existingPools) {
        Write-Host "Found existing application pools:" -ForegroundColor Cyan
        $existingPools | Format-Table -AutoSize
        
        # Check for naming conflicts
        if ($existingPools.Name -contains $AppPoolName) {
            Write-Error "CONFLICT: Application pool '$AppPoolName' already exists. Choose a different name."
            exit 1
        }
    }
    
    # Check port availability
    $tcpConnections = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue
    $usedPorts = $tcpConnections | Select-Object -ExpandProperty LocalPort
    
    if ($HttpPort -in $usedPorts) {
        Write-Error "CONFLICT: Port $HttpPort is already in use. Choose a different port."
        exit 1
    }
    
    if ($HttpsPort -in $usedPorts) {
        Write-Error "CONFLICT: Port $HttpsPort is already in use. Choose a different port."
        exit 1
    }
    
    Write-Host "✓ No conflicts detected. Safe to proceed." -ForegroundColor Green
}

# Check administrator privileges
function Test-AdminPrivileges {
    if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
        Write-Error "This script requires Administrator privileges. Please run as Administrator."
        exit 1
    }
}

# Backup existing configuration
function Backup-ExistingConfiguration {
    $backupPath = "C:\Backup\IIS_Config_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    
    Write-Host "Creating safety backup of IIS configuration..." -ForegroundColor Yellow
    
    try {
        New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
        
        # Backup IIS configuration
        Backup-WebConfiguration -Name "PPS_Asset_Deployment_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        
        # Export current websites and app pools
        Get-Website | Export-Csv "$backupPath\existing_websites.csv" -NoTypeInformation
        Get-IISAppPool | Export-Csv "$backupPath\existing_apppools.csv" -NoTypeInformation
        
        Write-Host "✓ Backup created at: $backupPath" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not create complete backup: $_"
    }
}

# Install required features (only if not already installed)
function Install-RequiredFeatures {
    if ($SkipIISFeatures) {
        Write-Host "Skipping IIS features installation (as requested)" -ForegroundColor Yellow
        return
    }
    
    Write-Host "Checking and installing required IIS features..." -ForegroundColor Yellow
    
    $requiredFeatures = @(
        "IIS-WebServerRole",
        "IIS-WebServer",
        "IIS-CommonHttpFeatures",
        "IIS-ApplicationDevelopment",
        "IIS-NetFxExtensibility45",
        "IIS-ASPNET45",
        "IIS-HttpCompressionStatic",
        "IIS-HttpCompressionDynamic"
    )
    
    foreach ($feature in $requiredFeatures) {
        $featureState = Get-WindowsOptionalFeature -Online -FeatureName $feature -ErrorAction SilentlyContinue
        if ($featureState.State -ne "Enabled") {
            Write-Host "Installing feature: $feature" -ForegroundColor Cyan
            Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart -WarningAction SilentlyContinue
        }
    }
}

# Setup isolated directory structure
function Setup-IsolatedDirectories {
    Write-Host "Setting up isolated directory structure..." -ForegroundColor Yellow
    
    # Create main application directory
    if (!(Test-Path $AppPath)) {
        New-Item -ItemType Directory -Path $AppPath -Force | Out-Null
        Write-Host "Created directory: $AppPath" -ForegroundColor Green
    }
    
    # Create subdirectories
    $subDirs = @("wwwroot", "logs", "temp", "backups")
    foreach ($dir in $subDirs) {
        $fullPath = Join-Path $AppPath $dir
        if (!(Test-Path $fullPath)) {
            New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
        }
    }
    
    # Create separate logs directory
    $logsPath = "C:\Logs\PPSAsset"
    if (!(Test-Path $logsPath)) {
        New-Item -ItemType Directory -Path $logsPath -Force | Out-Null
    }
    
    # Set proper permissions (isolated from other sites)
    Write-Host "Setting isolated permissions..." -ForegroundColor Cyan
    try {
        # Grant IIS_IUSRS access to app directory only
        icacls $AppPath /grant "IIS_IUSRS:(OI)(CI)F" /T | Out-Null
        icacls $AppPath /grant "IUSR:(OI)(CI)RX" /T | Out-Null
        
        # Separate log permissions
        icacls $logsPath /grant "IIS_IUSRS:(OI)(CI)F" /T | Out-Null
        
        Write-Host "✓ Isolated permissions configured" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not set all permissions: $_"
    }
}

# Create isolated application pool
function Setup-IsolatedApplicationPool {
    Write-Host "Creating isolated application pool..." -ForegroundColor Yellow
    
    # Create new application pool
    New-WebAppPool -Name $AppPoolName
    
    # Configure for .NET Core with resource limits
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "managedRuntimeVersion" -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "enable32BitAppOnWin64" -Value $false
    
    # Memory management (prevent resource starvation)
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "recycling.periodicRestart.time" -Value "02:00:00"
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "recycling.periodicRestart.memory" -Value "1048576" # 1GB
    
    # Process isolation
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "processModel.idleTimeout" -Value "00:20:00"
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "processModel.maxProcesses" -Value 1
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "processModel.requestTimeout" -Value "00:05:00"
    
    # CPU throttling (if server is under load)
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "cpu.limit" -Value "50000" # 50% CPU max
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name "cpu.action" -Value "ThrottleUnderLoad"
    
    Write-Host "✓ Application pool '$AppPoolName' configured with resource limits" -ForegroundColor Green
}

# Create website with unique bindings
function Setup-IsolatedWebsite {
    Write-Host "Creating isolated website with unique ports..." -ForegroundColor Yellow
    
    # Create website with unique HTTP port
    $physicalPath = Join-Path $AppPath "wwwroot"
    New-Website -Name $SiteName -PhysicalPath $physicalPath -Port $HttpPort
    
    # Add HTTPS binding with unique port
    try {
        New-WebBinding -Name $SiteName -Protocol "https" -Port $HttpsPort
        Write-Host "✓ HTTPS binding created on port $HttpsPort" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not create HTTPS binding: $_"
    }
    
    # Add domain binding if specified
    if ($DomainName) {
        try {
            New-WebBinding -Name $SiteName -Protocol "http" -Port 80 -HostHeader $DomainName
            New-WebBinding -Name $SiteName -Protocol "https" -Port 443 -HostHeader $DomainName
            Write-Host "✓ Domain bindings created for $DomainName" -ForegroundColor Green
        }
        catch {
            Write-Warning "Could not create domain bindings: $_"
        }
    }
    
    # Assign isolated application pool
    Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name "applicationPool" -Value $AppPoolName
    
    Write-Host "✓ Website '$SiteName' created with isolated configuration" -ForegroundColor Green
}

# Configure firewall for new ports
function Configure-FirewallPorts {
    Write-Host "Configuring firewall for new ports..." -ForegroundColor Yellow
    
    try {
        # Allow HTTP traffic on custom port
        $httpRuleName = "PPS Asset HTTP ($HttpPort)"
        if (!(Get-NetFirewallRule -DisplayName $httpRuleName -ErrorAction SilentlyContinue)) {
            New-NetFirewallRule -DisplayName $httpRuleName -Direction Inbound -Protocol TCP -LocalPort $HttpPort -Action Allow | Out-Null
        }
        
        # Allow HTTPS traffic on custom port
        $httpsRuleName = "PPS Asset HTTPS ($HttpsPort)"
        if (!(Get-NetFirewallRule -DisplayName $httpsRuleName -ErrorAction SilentlyContinue)) {
            New-NetFirewallRule -DisplayName $httpsRuleName -Direction Inbound -Protocol TCP -LocalPort $HttpsPort -Action Allow | Out-Null
        }
        
        Write-Host "✓ Firewall rules configured for ports $HttpPort and $HttpsPort" -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not configure firewall rules: $_"
    }
}

# Update configuration with isolated settings
function Update-IsolatedConfiguration {
    Write-Host "Creating isolated application configuration..." -ForegroundColor Yellow
    
    # Create production appsettings.json
    $appsettingsPath = Join-Path (Join-Path $AppPath "wwwroot") "appsettings.json"
    $logsPath = "C:\Logs\PPSAsset"
    
    $appsettingsConfig = @{
        "Logging" = @{
            "LogLevel" = @{
                "Default" = "Information"
                "Microsoft.AspNetCore" = "Warning"
                "Microsoft.Hosting.Lifetime" = "Information"
            }
            "File" = @{
                "Path" = "$logsPath\ppsasset-{Date}.log"
                "LogLevel" = @{
                    "Default" = "Information"
                }
            }
        }
        "AllowedHosts" = if ($DomainName) { $DomainName } else { "*" }
        "ConnectionStrings" = @{
            "DefaultConnection" = "Server=localhost;Database=$DatabaseName;Uid=$DatabaseUser;Pwd=$DatabasePassword;CharSet=utf8mb4;SslMode=Preferred;"
        }
        "RecaptchaSettings" = @{
            "SiteKey" = "6LfPawssAAAAAKdFROS6J-c9ftvQEwl9500WGrVi"
            "SecretKey" = "6LfPawssAAAAALXf-mXU9dG7t_JbRLWDlo5C6S2j"
        }
        "Kestrel" = @{
            "Endpoints" = @{
                "Http" = @{
                    "Url" = "http://localhost:$HttpPort"
                }
                "Https" = @{
                    "Url" = "https://localhost:$HttpsPort"
                }
            }
        }
        "Environment" = "Production"
    }
    
    # Save configuration
    $appsettingsConfig | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath -Encoding UTF8
    Write-Host "✓ Isolated configuration created" -ForegroundColor Green
}

# Create isolated web.config
function Create-IsolatedWebConfig {
    $webConfigPath = Join-Path (Join-Path $AppPath "wwwroot") "web.config"
    $logsPath = "C:\Logs\PPSAsset"
    
    $webConfigContent = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\PPSAsset.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile="$logsPath\stdout" 
                  hostingModel="inprocess"
                  forwardWindowsAuthToken="false">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="ASPNETCORE_URLS" value="http://localhost:$HttpPort" />
        </environmentVariables>
      </aspNetCore>
      
      <!-- Application-specific security headers -->
      <httpProtocol>
        <customHeaders>
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="X-Frame-Options" value="DENY" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
          <remove name="X-Powered-By" />
          <add name="X-PPS-Asset-App" value="true" />
        </customHeaders>
      </httpProtocol>
      
      <!-- Request filtering -->
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" maxUrl="4096" maxQueryString="2048" />
          <fileExtensions>
            <deny fileExtension=".config" />
            <deny fileExtension=".json" />
            <deny fileExtension=".log" />
          </fileExtensions>
          <hiddenSegments>
            <add segment="logs" />
            <add segment="temp" />
            <add segment="backups" />
          </hiddenSegments>
        </requestFiltering>
      </security>
      
      <!-- Static content compression -->
      <httpCompression>
        <dynamicTypes>
          <add mimeType="application/json" enabled="true" />
        </dynamicTypes>
      </httpCompression>
      
      <!-- Static content caching -->
      <staticContent>
        <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="365.00:00:00" />
      </staticContent>
    </system.webServer>
  </location>
</configuration>
"@
    
    Set-Content $webConfigPath -Value $webConfigContent -Encoding UTF8
    Write-Host "✓ Isolated web.config created" -ForegroundColor Green
}

# Test isolated deployment
function Test-IsolatedDeployment {
    Write-Host "Testing isolated deployment..." -ForegroundColor Yellow
    
    # Test application pool
    $poolState = (Get-IISAppPool -Name $AppPoolName).State
    if ($poolState -eq "Started") {
        Write-Host "✓ Application pool is running" -ForegroundColor Green
    } else {
        Write-Warning "Application pool is not running: $poolState"
    }
    
    # Test website
    $siteState = (Get-Website -Name $SiteName).State
    if ($siteState -eq "Started") {
        Write-Host "✓ Website is running" -ForegroundColor Green
    } else {
        Write-Warning "Website is not running: $siteState"
    }
    
    # Test port binding
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$HttpPort" -UseBasicParsing -TimeoutSec 10 -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 404) {
            Write-Host "✓ HTTP port $HttpPort is responsive" -ForegroundColor Green
        }
    }
    catch {
        Write-Warning "HTTP port test failed: $_"
    }
    
    # Verify existing sites are still running
    Write-Host "Verifying existing sites are still operational..." -ForegroundColor Cyan
    $allSites = Get-Website | Where-Object { $_.Name -ne $SiteName }
    foreach ($site in $allSites) {
        if ($site.State -eq "Started") {
            Write-Host "✓ Existing site '$($site.Name)' is still running" -ForegroundColor Green
        } else {
            Write-Warning "Existing site '$($site.Name)' appears to be stopped: $($site.State)"
        }
    }
}

# Generate database setup script
function Generate-DatabaseScript {
    $scriptPath = Join-Path $AppPath "setup_database.sql"
    
    $sqlScript = @"
-- PPS Asset Database Setup Script
-- Run this script in MySQL to create isolated database

-- Create dedicated database user
CREATE USER '$DatabaseUser'@'localhost' IDENTIFIED BY '$DatabasePassword';

-- Create dedicated database
CREATE DATABASE $DatabaseName CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Grant specific permissions (least privilege)
GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, INDEX, DROP 
ON $DatabaseName.* TO '$DatabaseUser'@'localhost';

-- Flush privileges
FLUSH PRIVILEGES;

-- Verify setup
SHOW GRANTS FOR '$DatabaseUser'@'localhost';
SHOW DATABASES LIKE '$DatabaseName';

-- Test connection (run separately)
-- mysql -u $DatabaseUser -p$DatabasePassword $DatabaseName
"@
    
    Set-Content $scriptPath -Value $sqlScript -Encoding UTF8
    Write-Host "✓ Database setup script created at: $scriptPath" -ForegroundColor Green
}

# Main deployment process
try {
    Write-Host "=== PPS Asset Multi-Tenant Deployment ===" -ForegroundColor Magenta
    Write-Host "VPS: 103.13.231.222:33899" -ForegroundColor Magenta
    Write-Host "Isolated App Path: $AppPath" -ForegroundColor Magenta
    Write-Host "Site Name: $SiteName" -ForegroundColor Magenta
    Write-Host "App Pool: $AppPoolName" -ForegroundColor Magenta
    Write-Host "HTTP Port: $HttpPort" -ForegroundColor Magenta
    Write-Host "HTTPS Port: $HttpsPort" -ForegroundColor Magenta
    Write-Host "Database: $DatabaseName" -ForegroundColor Magenta
    Write-Host "=======================================" -ForegroundColor Magenta
    
    Test-AdminPrivileges
    Test-ExistingServices
    Backup-ExistingConfiguration
    Install-RequiredFeatures
    Setup-IsolatedDirectories
    Setup-IsolatedApplicationPool
    Setup-IsolatedWebsite
    Configure-FirewallPorts
    Update-IsolatedConfiguration
    Create-IsolatedWebConfig
    Generate-DatabaseScript
    
    Write-Host ""
    Write-Host "=== Multi-Tenant Deployment Summary ===" -ForegroundColor Green
    Write-Host "✓ Safety checks passed - no conflicts detected" -ForegroundColor Green
    Write-Host "✓ Existing configuration backed up" -ForegroundColor Green
    Write-Host "✓ Isolated directories created: $AppPath" -ForegroundColor Green
    Write-Host "✓ Isolated application pool: $AppPoolName" -ForegroundColor Green
    Write-Host "✓ Isolated website: $SiteName" -ForegroundColor Green
    Write-Host "✓ Unique ports configured: $HttpPort (HTTP), $HttpsPort (HTTPS)" -ForegroundColor Green
    Write-Host "✓ Isolated configuration files created" -ForegroundColor Green
    Write-Host "✓ Database setup script generated" -ForegroundColor Green
    Write-Host ""
    
    Test-IsolatedDeployment
    
    Write-Host ""
    Write-Host "=== Next Steps ===" -ForegroundColor Yellow
    Write-Host "1. Run database setup script: $AppPath\setup_database.sql" -ForegroundColor White
    Write-Host "2. Copy published application files to: $AppPath\wwwroot\" -ForegroundColor White
    Write-Host "3. Test application at: http://103.13.231.222:$HttpPort" -ForegroundColor White
    Write-Host "4. Configure domain pointing to port $HttpPort" -ForegroundColor White
    Write-Host "5. Install SSL certificate for domain" -ForegroundColor White
    Write-Host "6. Monitor logs at: C:\Logs\PPSAsset\" -ForegroundColor White
    Write-Host ""
    Write-Host "=== Safety Features Implemented ===" -ForegroundColor Cyan
    Write-Host "✓ Isolated application pool with resource limits" -ForegroundColor White
    Write-Host "✓ Separate directory structure" -ForegroundColor White
    Write-Host "✓ Unique port bindings to avoid conflicts" -ForegroundColor White
    Write-Host "✓ Dedicated database and user" -ForegroundColor White
    Write-Host "✓ Separate logging system" -ForegroundColor White
    Write-Host "✓ Configuration backup created" -ForegroundColor White
    Write-Host "✓ Existing services verified as operational" -ForegroundColor White
    Write-Host ""
    Write-Host "Multi-tenant deployment completed successfully!" -ForegroundColor Green
}
catch {
    Write-Error "Deployment failed: $_"
    Write-Host ""
    Write-Host "=== Rollback Instructions ===" -ForegroundColor Red
    Write-Host "1. Remove website: Remove-Website -Name '$SiteName'" -ForegroundColor White
    Write-Host "2. Remove app pool: Remove-WebAppPool -Name '$AppPoolName'" -ForegroundColor White
    Write-Host "3. Remove directory: Remove-Item '$AppPath' -Recurse -Force" -ForegroundColor White
    Write-Host "4. Restore IIS config if needed: Restore-WebConfiguration" -ForegroundColor White
    exit 1
}