# Safe Multi-Tenant VPS Deployment Strategy for PPS Asset

## Overview

This deployment strategy ensures safe addition of the PPS Asset website to your Windows VPS (103.13.231.222:33899) without disrupting existing websites. The approach uses **complete isolation** at all levels to prevent conflicts.

## Key Safety Principles

### 1. Complete Resource Isolation
- **Separate Application Pool**: `PPSAsset_AppPool_2024` with independent resource limits
- **Isolated Directory Structure**: `C:\inetpub\sites\ppsasset\` (not in wwwroot)
- **Unique Port Bindings**: HTTP 8080, HTTPS 8443 (configurable)
- **Dedicated Database**: `ppsasset_production_db` with dedicated user
- **Separate Logging**: `C:\Logs\PPSAsset\` independent of other sites

### 2. Zero-Conflict Strategy
- **Pre-deployment Safety Checks**: Verify no naming or port conflicts
- **Configuration Backup**: Automatic IIS configuration backup before changes
- **Unique Naming Convention**: All resources prefixed with `PPSAsset_`
- **Resource Limits**: CPU and memory limits prevent resource starvation

### 3. Rollback Protection
- **Non-destructive Installation**: No modification of existing sites
- **Instant Rollback Capability**: Simple removal commands if issues arise
- **Verification Testing**: Confirms existing sites remain operational

## Directory Structure

```
C:\inetpub\sites\ppsasset\           # Isolated application directory
├── wwwroot\                         # Web content (Published app files go here)
│   ├── PPSAsset.dll
│   ├── appsettings.json
│   ├── web.config
│   └── [other app files]
├── logs\                            # Application-specific logs
├── temp\                            # Temporary files
└── backups\                         # Application backups

C:\Logs\PPSAsset\                    # Isolated system logs
├── ppsasset-{Date}.log
└── stdout\

C:\Backup\IIS_Config_{DateTime}\     # Safety backup
├── existing_websites.csv
└── existing_apppools.csv
```

## Network Configuration

### Port Strategy
- **HTTP**: Port 8080 (instead of 80 to avoid conflicts)
- **HTTPS**: Port 8443 (instead of 443 to avoid conflicts)
- **Domain Binding**: Optional domain.com:80/443 if DNS configured
- **Firewall**: Automatic rules for new ports

### Access URLs
- **Direct Access**: `http://103.13.231.222:8080`
- **SSL Access**: `https://103.13.231.222:8443`
- **Domain Access**: `http://yourdomain.com` (if configured)

## Database Isolation

### MySQL Setup
```sql
-- Dedicated database and user
CREATE DATABASE ppsasset_production_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'ppsasset_user'@'localhost' IDENTIFIED BY 'SECURE_PASSWORD';

-- Minimum required permissions only
GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, INDEX, DROP 
ON ppsasset_production_db.* TO 'ppsasset_user'@'localhost';

FLUSH PRIVILEGES;
```

### Benefits
- **Security**: Limited permissions prevent access to other databases
- **Performance**: Separate database prevents query interference
- **Maintenance**: Independent backup/restore capabilities

## Application Pool Configuration

### Resource Limits
- **CPU Limit**: 50% maximum to prevent server impact
- **Memory Limit**: 1GB private bytes before recycling
- **Process Timeout**: 5 minutes to prevent hanging processes
- **Idle Timeout**: 20 minutes to free resources when not in use

### Recycling Schedule
- **Time-based**: Every 2 hours during low-traffic periods
- **Memory-based**: When memory usage exceeds 1GB
- **Request-based**: After 10,000 requests (prevents memory leaks)

## Security Measures

### Application-Level Security
- **Isolated Permissions**: IIS_IUSRS access only to PPS Asset directories
- **Request Filtering**: File extension and path restrictions
- **Custom Headers**: Application-specific security headers
- **Log Isolation**: Separate log files prevent information leakage

### Network Security
- **Firewall Rules**: Specific rules for PPS Asset ports only
- **SSL Configuration**: Independent certificate management
- **Request Size Limits**: 50MB max upload to prevent abuse

## Monitoring & Maintenance

### Health Checks
- **Application Pool Status**: Monitor for unexpected stops
- **Memory Usage**: Alert if approaching limits
- **Response Time**: Monitor for performance degradation
- **Error Rates**: Track application errors separately

### Log Management
```
C:\Logs\PPSAsset\
├── ppsasset-20241129.log          # Daily application logs
├── stdout\                        # Standard output logs
└── error\                         # Error-specific logs
```

### Backup Strategy
- **Application Files**: Weekly backup of `C:\inetpub\sites\ppsasset\`
- **Database**: Daily backup of `ppsasset_production_db`
- **Configuration**: Automatic backup before any changes
- **Logs**: 30-day retention with compression

## Deployment Process

### Phase 1: Safety Verification
1. **Administrator Check**: Verify script runs with admin privileges
2. **Conflict Detection**: Scan for existing name/port conflicts
3. **Resource Assessment**: Check available memory and disk space
4. **Backup Creation**: Create restoration points

### Phase 2: Infrastructure Setup
1. **Directory Creation**: Establish isolated file structure
2. **Permissions**: Configure security boundaries
3. **Application Pool**: Create with resource limits
4. **Website**: Configure with unique bindings

### Phase 3: Application Configuration
1. **Configuration Files**: Generate isolated settings
2. **Database Setup**: Create dedicated database and user
3. **Firewall Rules**: Open required ports
4. **Service Startup**: Initialize isolated services

### Phase 4: Verification
1. **Functionality Test**: Verify PPS Asset responds correctly
2. **Existing Sites Check**: Confirm other sites still operational
3. **Resource Usage**: Monitor initial resource consumption
4. **Security Validation**: Test access restrictions

## Troubleshooting Guide

### Common Issues

#### Port Already in Use
```bash
# Check what's using the port
netstat -ano | findstr :8080
# Change to different port in deployment script
-HttpPort "8081"
```

#### Application Pool Won't Start
```bash
# Check event logs
Get-WinEvent -LogName System | Where-Object {$_.Id -eq 5004}
# Verify .NET installation
dotnet --list-runtimes
```

#### Database Connection Issues
```bash
# Test database connection
mysql -u ppsasset_user -p ppsasset_production_db
# Verify user permissions
SHOW GRANTS FOR 'ppsasset_user'@'localhost';
```

### Emergency Rollback

If deployment causes issues with existing sites:

```powershell
# Immediate rollback commands
Remove-Website -Name "PPSAsset_Website"
Remove-WebAppPool -Name "PPSAsset_AppPool_2024"
Remove-Item "C:\inetpub\sites\ppsasset" -Recurse -Force

# Restore IIS configuration
Restore-WebConfiguration -Name "[backup_name]"

# Restart IIS to ensure clean state
iisreset /restart
```

## Performance Considerations

### Server Resource Management
- **Memory**: PPS Asset limited to 1GB maximum
- **CPU**: 50% limit prevents server impact
- **Disk I/O**: Separate directories for better I/O distribution
- **Network**: Unique ports prevent bandwidth contention

### Application Optimization
- **Static Content**: CDN integration recommended
- **Database**: Connection pooling with limits
- **Caching**: Redis cache with isolated instance
- **Image Optimization**: WebP conversion and compression

## Maintenance Schedule

### Daily Tasks
- Monitor application pool status
- Check error logs for issues
- Verify disk space availability

### Weekly Tasks
- Backup application files
- Update security patches
- Performance monitoring review

### Monthly Tasks
- SSL certificate validation
- Database maintenance
- Log file cleanup
- Performance optimization

## Success Metrics

### Technical Metrics
- **Uptime**: 99.9% target for PPS Asset
- **Response Time**: < 2 seconds average
- **Memory Usage**: < 800MB steady state
- **Error Rate**: < 0.1% of requests

### Safety Metrics
- **Existing Sites Uptime**: 100% (no impact)
- **Resource Conflicts**: Zero incidents
- **Security Breaches**: Zero incidents
- **Rollback Events**: < 1 per quarter

## Contact & Support

### Immediate Issues
1. Check application pool status
2. Review error logs at `C:\Logs\PPSAsset\`
3. Verify database connectivity
4. Check firewall rules

### Emergency Contacts
- **Database Issues**: Check MySQL service status
- **IIS Problems**: Review Windows Event Logs
- **Network Issues**: Verify firewall and port settings

---

This multi-tenant deployment strategy ensures your PPS Asset website runs safely alongside existing websites without conflicts or resource competition. The isolation approach provides security, stability, and easy maintenance while protecting your existing infrastructure.