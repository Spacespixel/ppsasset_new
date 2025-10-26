#!/bin/bash

# Database Setup Script for PPS Asset Project
# This script will help you set up the MySQL database locally

echo "=== PPS Asset Database Setup ==="
echo ""

# Check if MySQL is installed
if ! command -v mysql &> /dev/null; then
    echo "❌ MySQL is not installed or not in PATH"
    echo ""
    echo "Please install MySQL first:"
    echo "• On macOS: brew install mysql"
    echo "• On Windows: Download from https://dev.mysql.com/downloads/mysql/"
    echo "• Or use XAMPP: https://www.apachefriends.org/"
    exit 1
fi

echo "✅ MySQL found"

# Get database credentials
echo ""
echo "Please enter your MySQL credentials:"
read -p "Host (default: localhost): " DB_HOST
DB_HOST=${DB_HOST:-localhost}

read -p "Username (default: root): " DB_USER
DB_USER=${DB_USER:-root}

read -s -p "Password: " DB_PASSWORD
echo ""

# Test connection
echo ""
echo "Testing MySQL connection..."

mysql -h "$DB_HOST" -u "$DB_USER" -p"$DB_PASSWORD" -e "SELECT 1;" &> /dev/null

if [ $? -eq 0 ]; then
    echo "✅ MySQL connection successful"
else
    echo "❌ MySQL connection failed"
    echo "Please check your credentials and try again"
    exit 1
fi

# Run the setup script
echo ""
echo "Setting up database schema..."

mysql -h "$DB_HOST" -u "$DB_USER" -p"$DB_PASSWORD" < setup_database.sql

if [ $? -eq 0 ]; then
    echo "✅ Database setup completed successfully!"
    echo ""
    echo "📝 Next steps:"
    echo "1. Update your connection string in appsettings.json:"
    echo "   \"Server=$DB_HOST;Database=thericco;Uid=$DB_USER;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;\""
    echo ""
    echo "2. Run your application: dotnet run"
    echo ""
    echo "3. Test the migration endpoint: http://localhost:5000/MigrateDatabase"
    echo ""
    echo "4. Check service status: http://localhost:5000/ServiceStatus"
else
    echo "❌ Database setup failed"
    echo "Please check the error messages above"
    exit 1
fi