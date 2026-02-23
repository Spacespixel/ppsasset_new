# Production Logging Setup Instructions

## Log File Creation Commands (Run on VPS)

```bash
# Create log directory
sudo mkdir -p /var/log/ppsasset

# Set ownership to www-data (or your app user)
sudo chown -R www-data:www-data /var/log/ppsasset

# Set permissions for log directory
sudo chmod 755 /var/log/ppsasset

# Create initial log file (optional)
sudo touch /var/log/ppsasset/ppsasset-$(date +%Y%m%d).log
sudo chown www-data:www-data /var/log/ppsasset/ppsasset-*.log
sudo chmod 644 /var/log/ppsasset/ppsasset-*.log
```

## Logging Configuration Applied

### 1. Enhanced appsettings.Production.json
- **Debug level logging** for detailed troubleshooting
- **File logging** to `/var/log/ppsasset/ppsasset-{Date}.log`
- **Console logging** for immediate visibility
- **10MB file size limit** with 10 rolling files

### 2. HomeController.cs Enhanced Logging
- **Detailed Index() method logging** to trace every step
- **Exception handling** with full context logging
- **Request/response tracking** with timing information

### 3. Program.cs Request Middleware
- **HTTP request/response logging** with timing
- **Error tracking** with exception details
- **Remote IP and User-Agent logging**

## Expected Log Output

After restart, you should see detailed logs showing:

1. **Application startup** - service registrations, configuration loading
2. **HTTP requests** - method, path, timing, status codes
3. **Index action execution** - step-by-step processing
4. **Database operations** - project loading, navigation data
5. **Error details** - full exception information with context

## Viewing Logs

```bash
# View latest log file
sudo tail -f /var/log/ppsasset/ppsasset-$(date +%Y%m%d).log

# View all log files
ls -la /var/log/ppsasset/

# Search for errors
sudo grep -i "error\|exception" /var/log/ppsasset/ppsasset-*.log

# Search for specific requests
sudo grep "Index action" /var/log/ppsasset/ppsasset-*.log
```

## Next Steps

1. **Restart the application** on production VPS
2. **Create log directory** using commands above
3. **Make a test request** to http://103.13.231.222:8081
4. **Check logs** to identify the exact cause of HTTP 500 error

This comprehensive logging will reveal:
- Missing view files
- Service registration failures  
- Database connection issues
- Missing dependencies
- Configuration problems
- Controller execution failures