#!/bin/bash

# Analyze production error with timestamp
echo "============================================="
echo "Production Error Analysis - $(date)"
echo "============================================="

if [ ! -f "/Users/horizon/Downloads/latest_error.log" ]; then
    echo "ERROR: /Users/horizon/Downloads/latest_error.log not found"
    echo "Please provide the latest production error log"
    exit 1
fi

echo "Log file timestamp: $(stat -f %Sm -t '%Y-%m-%d %H:%M:%S' /Users/horizon/Downloads/latest_error.log)"
echo "Log file size: $(wc -l < /Users/horizon/Downloads/latest_error.log) lines"
echo ""

echo "=== Key Error Patterns ==="
grep -i "ClientId.*must be provided" /Users/horizon/Downloads/latest_error.log | head -1
grep -i "OAuth" /Users/horizon/Downloads/latest_error.log | head -1  
grep -i "Authentication" /Users/horizon/Downloads/latest_error.log | head -1
echo ""

echo "=== Configuration Check ==="
echo "Current appsettings.json has OAuth: $(grep -c "Authentication" appsettings.json)"
echo "Current Program.cs has OAuth protection: $(grep -c "hasValidGoogleConfig" Program.cs)"
echo ""

echo "=== Deployment Status ==="
echo "Build timestamp: $(stat -f %Sm -t '%Y-%m-%d %H:%M:%S' bin/Release/net8.0/PPSAsset.dll 2>/dev/null || echo 'Not built')"
echo "=== Analysis complete at $(date) ==="