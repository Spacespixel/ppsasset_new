# Server State Assessment Script for PPS Asset VPS
# This script analyzes the current state of the Windows VPS before deployment
# Run this on the VPS (103.13.231.222:33899) via RDP

Write-Host "=== PPS Asset VPS Server State Assessment ===" -ForegroundColor Magenta
Write-Host "Server: 103.13.231.222:33899" -ForegroundColor Cyan
Write-Host "Assessment Time: $(Get-Date)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Magenta

# Function to safely execute commands and capture results
function Safe-Execute {
    param([string]$Command, [string]$Description)
    
    Write-Host "`n--- $Description ---" -ForegroundColor Yellow
    try {
        Invoke-Expression $Command
    }
    catch {
        Write-Host "Error executing: $Description" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
    }
}

# 1. Check IIS Websites
Safe-Execute "Get-Website | Format-Table Name, State, PhysicalPath, @{Name='Bindings';Expression={(\$_.bindings.Collection | ForEach-Object { \"\$(\$_.protocol):\$(\$_.bindingInformation)\" }) -join ', '}} -AutoSize" "Current IIS Websites"

# 2. Check Application Pools
Safe-Execute "Get-IISAppPool | Format-Table Name, State, @{Name='Runtime';Expression={\$_.managedRuntimeVersion}}, @{Name='ProcessModel';Expression={\$_.processModel.identityType}} -AutoSize" "Current Application Pools"

# 3. Check Used Ports
Safe-Execute "Get-NetTCPConnection -State Listen | Select-Object LocalAddress, LocalPort, OwningProcess | Sort-Object LocalPort | Format-Table -AutoSize" "Used Network Ports"

# 4. Check Directory Structure - IIS Standard
Write-Host "`n--- IIS Standard Directory Structure ---" -ForegroundColor Yellow
$iisStandardPaths = @(
    "C:\inetpub\wwwroot",
    "C:\inetpub\wwwroot\ppsasset",
    "C:\inetpub\sites",
    "C:\inetpub\sites\ppsasset"
)

foreach ($path in $iisStandardPaths) {
    if (Test-Path $path) {
        Write-Host "✓ EXISTS: $path" -ForegroundColor Green
        $items = Get-ChildItem $path -ErrorAction SilentlyContinue
        if ($items) {
            Write-Host "  Contents: $($items.Count) items" -ForegroundColor White
            $items | Select-Object -First 5 | ForEach-Object { Write-Host "    - $($_.Name)" -ForegroundColor Gray }
            if ($items.Count -gt 5) { Write-Host "    ... and $($items.Count - 5) more items" -ForegroundColor Gray }
        }
    } else {
        Write-Host "✗ NOT FOUND: $path" -ForegroundColor Red
    }
}

# 5. Check Directory Structure - Production Multi-Tenant
Write-Host "`n--- Production Multi-Tenant Directory Structure ---" -ForegroundColor Yellow
$prodPaths = @(
    "C:\Production",
    "C:\Production\web",
    "C:\Production\web\ppsasset.com",
    "C:\Production\logs",
    "C:\Production\logs\ppsasset"
)

foreach ($path in $prodPaths) {
    if (Test-Path $path) {
        Write-Host "✓ EXISTS: $path" -ForegroundColor Green
        $items = Get-ChildItem $path -ErrorAction SilentlyContinue
        if ($items) {
            Write-Host "  Contents: $($items.Count) items" -ForegroundColor White
            $items | ForEach-Object { Write-Host "    - $($_.Name)" -ForegroundColor Gray }
        }
    } else {
        Write-Host "✗ NOT FOUND: $path" -ForegroundColor Red
    }
}

# 6. Check MySQL Status and Databases
Write-Host "`n--- MySQL Database Status ---" -ForegroundColor Yellow
try {
    $mysqlService = Get-Service -Name "MySQL*" -ErrorAction SilentlyContinue
    if ($mysqlService) {
        Write-Host "✓ MySQL Service Found: $($mysqlService.Name) - Status: $($mysqlService.Status)" -ForegroundColor Green
        
        # Try to list databases (requires MySQL to be in PATH)
        try {
            Write-Host "Attempting to list databases..." -ForegroundColor Cyan
            $databases = mysql -u root -p -e "SHOW DATABASES;" 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ Database connection successful" -ForegroundColor Green
                Write-Host "Databases:" -ForegroundColor White
                $databases | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
            } else {
                Write-Host "⚠ MySQL found but connection failed (password required)" -ForegroundColor Yellow
            }
        }
        catch {
            Write-Host "⚠ MySQL found but unable to query databases" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ MySQL Service not found" -ForegroundColor Red
    }
}
catch {
    Write-Host "✗ Error checking MySQL status: $_" -ForegroundColor Red
}

# 7. Check .NET Runtime
Safe-Execute "dotnet --list-runtimes 2>$null" ".NET Runtime Status"

# 8. Check IIS Features
Write-Host "`n--- IIS Features Status ---" -ForegroundColor Yellow
$iisFeatures = @(
    "IIS-WebServerRole",
    "IIS-AspNetCoreModuleV2", 
    "IIS-ASPNET45",
    "IIS-HttpCompressionStatic"
)

foreach ($feature in $iisFeatures) {
    try {
        $featureState = Get-WindowsOptionalFeature -Online -FeatureName $feature -ErrorAction SilentlyContinue
        if ($featureState) {
            if ($featureState.State -eq "Enabled") {
                Write-Host "✓ $feature: Enabled" -ForegroundColor Green
            } else {
                Write-Host "✗ $feature: $($featureState.State)" -ForegroundColor Red
            }
        } else {
            Write-Host "? $feature: Unknown" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "? $feature: Error checking" -ForegroundColor Yellow
    }
}

# 9. Check Active Processes
Safe-Execute "Get-Process | Where-Object {(\$_.ProcessName -like '*iis*') -or (\$_.ProcessName -like '*mysql*') -or (\$_.ProcessName -like '*dotnet*')} | Select-Object ProcessName, Id, CPU, WorkingSet | Format-Table -AutoSize" "Relevant Running Processes"

# 10. Check System Resources
Write-Host "`n--- System Resources ---" -ForegroundColor Yellow
$memory = Get-CimInstance Win32_OperatingSystem
$totalMemGB = [math]::Round($memory.TotalVisibleMemorySize / 1MB, 2)
$freeMemGB = [math]::Round($memory.FreePhysicalMemory / 1MB, 2)
$usedMemGB = $totalMemGB - $freeMemGB

Write-Host "Memory: $usedMemGB GB used / $totalMemGB GB total ($([math]::Round(($usedMemGB/$totalMemGB)*100,1))% used)" -ForegroundColor White

$cpu = Get-CimInstance Win32_Processor | Measure-Object -Property LoadPercentage -Average
Write-Host "CPU Usage: $([math]::Round($cpu.Average, 1))%" -ForegroundColor White

# 11. Check Disk Space
$disks = Get-CimInstance Win32_LogicalDisk | Where-Object {$_.DriveType -eq 3}
foreach ($disk in $disks) {
    $totalGB = [math]::Round($disk.Size / 1GB, 2)
    $freeGB = [math]::Round($disk.FreeSpace / 1GB, 2)
    $usedGB = $totalGB - $freeGB
    $usedPercent = [math]::Round(($usedGB / $totalGB) * 100, 1)
    Write-Host "Disk $($disk.DeviceID) $usedGB GB used / $totalGB GB total ($usedPercent% used)" -ForegroundColor White
}

# 12. Summary and Recommendations
Write-Host "`n=== ASSESSMENT SUMMARY ===" -ForegroundColor Magenta

Write-Host "`nCurrent Deployment Status:" -ForegroundColor Cyan
$ppsAssetSite = Get-Website | Where-Object {$_.Name -like "*pps*" -or $_.Name -like "*asset*"}
$ppsAssetPool = Get-IISAppPool | Where-Object {$_.Name -like "*pps*" -or $_.Name -like "*asset*"}

if ($ppsAssetSite -or $ppsAssetPool -or (Test-Path "C:\inetpub\wwwroot\ppsasset") -or (Test-Path "C:\Production\web\ppsasset.com")) {
    Write-Host "🔵 PPS Asset appears to be ALREADY DEPLOYED" -ForegroundColor Blue
    if ($ppsAssetSite) { Write-Host "  - Found website: $($ppsAssetSite.Name)" -ForegroundColor White }
    if ($ppsAssetPool) { Write-Host "  - Found app pool: $($ppsAssetPool.Name)" -ForegroundColor White }
    Write-Host "  - Recommendation: UPDATE existing deployment" -ForegroundColor Yellow
} else {
    Write-Host "🟢 PPS Asset NOT DETECTED - Fresh deployment possible" -ForegroundColor Green
    Write-Host "  - Recommendation: NEW deployment with multi-tenant structure" -ForegroundColor Yellow
}

Write-Host "`nServer Readiness:" -ForegroundColor Cyan
$mysqlReady = (Get-Service -Name "MySQL*" -ErrorAction SilentlyContinue) -ne $null
$iisReady = (Get-Website).Count -gt 0
$dotnetReady = (dotnet --list-runtimes 2>$null) -ne $null

Write-Host "  - MySQL: $(if($mysqlReady){'✓ Ready'}else{'✗ Needs Installation'})" -ForegroundColor $(if($mysqlReady){'Green'}else{'Red'})
Write-Host "  - IIS: $(if($iisReady){'✓ Ready'}else{'✗ Needs Installation'})" -ForegroundColor $(if($iisReady){'Green'}else{'Red'})
Write-Host "  - .NET 8.0: $(if($dotnetReady){'✓ Ready'}else{'✗ Needs Installation'})" -ForegroundColor $(if($dotnetReady){'Green'}else{'Red'})

Write-Host "`n=== END ASSESSMENT ===" -ForegroundColor Magenta
Write-Host "Copy this output and share with deployment planning." -ForegroundColor Gray