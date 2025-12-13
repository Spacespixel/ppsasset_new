# PPS Asset Production Deployment Checklist

## 📋 Pre-Deployment Requirements

### ✅ Prerequisites
- [ ] MySQL 8.0+ installed on production server
- [ ] Production server configured with adequate resources (2GB+ RAM recommended)
- [ ] SSL certificates configured for HTTPS
- [ ] Domain name configured and pointing to server
- [ ] Backup strategy planned and tested

### ✅ Local Development Verification
- [ ] Local application running successfully
- [ ] All projects displaying correctly in local environment
- [ ] Database migration endpoint `/MigrateDatabase` functional
- [ ] Static service fallback working properly

## 🗄️ Database Deployment Steps

### Step 1: Create Production Database Structure
```bash
# Run the database creation script
mysql -u root -p < create-production-database.sql
```

**Verification:**
- [ ] Database `ppsasset_production_db` created successfully
- [ ] User `ppsasset_user` created with proper permissions
- [ ] All 8 required tables created
- [ ] Test project record inserted and queryable

### Step 2: Deploy Master Configuration Data
```bash
# Deploy core project data and configuration
mysql -u root -p < production-deployment-master.sql
```

**Verification:**
- [ ] 6 main projects inserted successfully
- [ ] Project images configuration loaded
- [ ] Location and contact data populated
- [ ] Facilities master data inserted
- [ ] No foreign key constraint errors

### Step 3: Extract Local Development Data (Optional)
```bash
# Extract additional data from local database if needed
mysql -u root -p thericco < extract-local-data.sql > local-data-export.sql

# Review and clean the exported data, then import:
mysql -u ppsasset_user -p ppsasset_production_db < local-data-export.sql
```

**Verification:**
- [ ] Local data extracted without errors
- [ ] No duplicate records created
- [ ] All references maintained properly

### Step 4: Apply Production Configuration
```bash
# Apply production-specific settings and additional data
mysql -u ppsasset_user -p ppsasset_production_db < production-config-update.sql
```

**Verification:**
- [ ] GTM IDs updated for production
- [ ] Contact information updated
- [ ] Concept features added for all projects
- [ ] House types and nearby places populated

## 🚀 Application Deployment

### Step 5: Update Application Configuration

**Update `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ppsasset_production_db;Uid=ppsasset_user;Pwd=YOUR_SECURE_PASSWORD;CharSet=utf8mb4;SslMode=Preferred;"
  },
  "Environment": "Production",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "File": {
      "Path": "/var/log/ppsasset/ppsasset-{Date}.log"
    }
  }
}
```

**Verification:**
- [ ] Connection string updated with production database
- [ ] Environment set to "Production"
- [ ] Logging configuration appropriate for production
- [ ] SSL mode configured properly

### Step 6: Deploy Application Files

**Using systemd service (recommended):**
```bash
# Copy application files to production directory
sudo cp -r /path/to/published-app/* /var/www/ppsasset/

# Set proper permissions
sudo chown -R www-data:www-data /var/www/ppsasset/
sudo chmod -R 755 /var/www/ppsasset/

# Start the application service
sudo systemctl start ppsasset
sudo systemctl enable ppsasset
```

**Verification:**
- [ ] Application files deployed to production directory
- [ ] Proper file permissions set
- [ ] Application service starting successfully
- [ ] No startup errors in logs

## 🔍 Post-Deployment Testing

### Step 7: Verify Database Connectivity
Test these endpoints in order:

1. **Service Status Check:**
```bash
curl https://yourdomain.com/ServiceStatus
```
Expected: Database connectivity confirmed

2. **Homepage Loading:**
```bash
curl https://yourdomain.com/
```
Expected: Homepage loads with project listings

3. **Individual Project Pages:**
```bash
curl https://yourdomain.com/Project/ricco-residence-hathairat
curl https://yourdomain.com/singlehouse/ricco-residence-ramintra/hathairat
```
Expected: Project details load correctly

**Verification:**
- [ ] Service status endpoint returns success
- [ ] Homepage displays all available projects
- [ ] Individual project pages load with complete data
- [ ] No database connection errors in logs
- [ ] Response times acceptable (<2 seconds)

### Step 8: Verify Hybrid Service Architecture
```bash
# Test database fallback by temporarily stopping MySQL
sudo systemctl stop mysql

# Check application still responds (should use static fallback)
curl https://yourdomain.com/

# Restart MySQL and verify database service resumes
sudo systemctl start mysql
curl https://yourdomain.com/ServiceStatus
```

**Verification:**
- [ ] Application remains available during database downtime
- [ ] Static fallback service provides project data
- [ ] Database service resumes after MySQL restart
- [ ] No data loss or corruption

## 📊 Performance and Monitoring

### Step 9: Database Performance Optimization
```sql
-- Run these commands in production database
USE ppsasset_production_db;

-- Analyze table statistics
ANALYZE TABLE sy_project;
ANALYZE TABLE sy_project_images;
ANALYZE TABLE sy_project_house_types;

-- Verify indexes are being used
EXPLAIN SELECT * FROM sy_project WHERE ProjectType = 'SingleHouse';
EXPLAIN SELECT * FROM sy_project_images WHERE ProjectID = 'ricco-residence-hathairat';
```

**Verification:**
- [ ] All tables analyzed successfully
- [ ] Queries using appropriate indexes
- [ ] Query execution plans optimized

### Step 10: Setup Monitoring and Backups
```bash
# Setup daily database backup
sudo crontab -e
# Add: 0 2 * * * mysqldump -u ppsasset_user -p ppsasset_production_db > /backup/ppsasset-$(date +\%Y\%m\%d).sql

# Setup log rotation
sudo nano /etc/logrotate.d/ppsasset

# Monitor disk space and memory usage
df -h
free -m
```

**Verification:**
- [ ] Automated backup script configured and tested
- [ ] Log rotation configured to prevent disk space issues
- [ ] System resources monitored and adequate
- [ ] Alert mechanisms in place for failures

## 🔒 Security Checklist

### Step 11: Production Security Hardening
- [ ] Database user has minimal required permissions only
- [ ] Database password is strong and securely stored
- [ ] MySQL remote root access disabled
- [ ] Test databases and anonymous users removed
- [ ] Application logs don't contain sensitive information
- [ ] HTTPS enforced for all traffic
- [ ] Security headers configured in web server

## ✅ Final Deployment Verification

### Complete System Test
- [ ] **Homepage:** All projects display with correct information
- [ ] **Project Pages:** Individual project data loads completely
- [ ] **Images:** All project images load correctly
- [ ] **Navigation:** URL routing works for both legacy and PPS formats
- [ ] **Performance:** Page load times under 3 seconds
- [ ] **Mobile:** Responsive design works on mobile devices
- [ ] **SEO:** Meta tags and structured data present
- [ ] **Analytics:** GTM tracking configured and firing
- [ ] **Forms:** Contact forms submit successfully (if applicable)

### Load Testing (Optional but Recommended)
```bash
# Simple load test with curl
for i in {1..50}; do 
  curl -o /dev/null -s -w "%{http_code} %{time_total}\n" https://yourdomain.com/
done

# Or use dedicated tools like Apache Bench
ab -n 100 -c 10 https://yourdomain.com/
```

## 📞 Emergency Rollback Plan

### If Deployment Fails:
1. **Database Issues:**
   ```bash
   # Restore from backup if needed
   mysql -u root -p ppsasset_production_db < backup-file.sql
   
   # Or switch to static service only by commenting out database service registration
   ```

2. **Application Issues:**
   ```bash
   # Revert to previous application version
   sudo systemctl stop ppsasset
   sudo cp -r /backup/previous-version/* /var/www/ppsasset/
   sudo systemctl start ppsasset
   ```

3. **Configuration Issues:**
   ```bash
   # Revert connection string to working configuration
   sudo cp /backup/appsettings.json.backup /var/www/ppsasset/appsettings.json
   sudo systemctl restart ppsasset
   ```

## 📋 Post-Go-Live Tasks

### Immediately After Launch:
- [ ] Monitor application logs for errors
- [ ] Verify Google Analytics/GTM tracking
- [ ] Test contact forms and lead capture
- [ ] Monitor server resources (CPU, memory, disk)
- [ ] Document any issues and resolutions

### Within 24 Hours:
- [ ] Verify database backup completed successfully
- [ ] Check search engine indexing status
- [ ] Monitor website performance metrics
- [ ] Validate SSL certificate installation
- [ ] Test disaster recovery procedures

### Within 1 Week:
- [ ] Analyze user behavior and performance metrics
- [ ] Optimize any identified performance bottlenecks
- [ ] Update documentation with production-specific details
- [ ] Schedule regular maintenance windows
- [ ] Plan content updates and feature enhancements

---

## 📞 Support Contacts

**Database Issues:** Review logs in `/var/log/mysql/`
**Application Issues:** Review logs in `/var/log/ppsasset/`
**Server Issues:** Review system logs with `journalctl -u ppsasset`

**Emergency Contacts:**
- System Administrator: [Contact Details]
- Database Administrator: [Contact Details]
- Development Team: [Contact Details]

---

*This checklist should be completed in order and each step verified before proceeding to the next.*