#!/bin/bash
# Script to extract all table structures from local database

echo "-- ==============================================================="
echo "-- COMPLETE TABLE STRUCTURES FROM LOCAL THERICCO DATABASE"
echo "-- Generated on: $(date)"
echo "-- ==============================================================="
echo ""

# Get list of tables
TABLES=$(mysql -u root thericco -e "SHOW TABLES;" | grep -v "Tables_in_thericco")

for table in $TABLES; do
    echo "-- ==============================================================="
    echo "-- TABLE: $table"
    echo "-- ==============================================================="
    mysql -u root thericco -e "SHOW CREATE TABLE $table;" | tail -1 | cut -f2 | sed 's/\\n/\n/g'
    echo ";"
    echo ""
done

echo "-- ==============================================================="
echo "-- END OF TABLE STRUCTURES"
echo "-- ==============================================================="