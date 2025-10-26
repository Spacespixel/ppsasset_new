# Database Changes Specification for PPS Asset Project

## Overview
This document tracks all database schema changes, data migrations, and structural modifications made to the PPS Asset database system.

## Database Information
- **Database Name:** `thericco`
- **Engine:** MySQL 8.0
- **Character Set:** utf8mb4
- **Collation:** utf8mb4_unicode_ci
- **Environment:** Local Development

## Schema Evolution

### Phase 1: Initial Schema Setup
**Date:** October 11, 2025
**Files:** `.claude/setup_database.sql`

#### Original Tables (from legacy system)
```sql
-- Core project table
sy_project (
    ProjectID varchar(15) PRIMARY KEY,
    ProjectName varchar(500),
    ProjectAddress varchar(1000),
    ProjectType varchar(255),
    ProjectNameEN varchar(100),
    ProjectEmail varchar(255)
)

-- Transaction tracking
tr_transaction (
    TransactoinID varchar(15) PRIMARY KEY,
    ProjectID varchar(15),
    FirstName varchar(255),
    LastName varchar(255),
    -- ... additional customer fields
)

-- Running number management
sy_runningnumber (
    RunningNumberID int AUTO_INCREMENT PRIMARY KEY,
    RunningNumberDocCode varchar(5),
    RunningNumberCurrentYear varchar(4),
    RunningNumber int
)
```

### Phase 2: Extended Content Management Schema
**Date:** October 11, 2025
**Purpose:** Enable database-driven content management

#### Schema Extensions Applied

##### 2.1 Extended sy_project Table
**Change Type:** ALTER TABLE ADD COLUMN
```sql
ALTER TABLE sy_project ADD COLUMN (
    ProjectSubtitle VARCHAR(1000) NULL COMMENT "Project tagline/subtitle",
    ProjectDescription TEXT NULL COMMENT "Main project description",
    ProjectConcept TEXT NULL COMMENT "Project concept/theme",
    ProjectStatus ENUM("Available", "SoldOut", "ComingSoon") DEFAULT "Available",
    ProjectSize VARCHAR(100) NULL COMMENT "Total project area",
    TotalUnits INT NULL COMMENT "Total number of units/houses",
    LandSize VARCHAR(100) NULL COMMENT "Land size per unit",
    UsableArea VARCHAR(100) NULL COMMENT "Usable area per unit",
    Developer VARCHAR(255) NULL COMMENT "Developer company name",
    PriceRange VARCHAR(100) NULL COMMENT "Price range",
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

##### 2.2 New Content Management Tables

**Project Images Table**
```sql
CREATE TABLE sy_project_images (
    ImageID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    ImageType ENUM('Hero', 'Logo', 'Gallery', 'Facility', 'FloorPlan') NOT NULL,
    ImagePath VARCHAR(500) NOT NULL,
    ImageAlt VARCHAR(255) NULL,
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE
);
```

**Project House Types**
```sql
CREATE TABLE sy_project_house_types (
    HouseTypeID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    HouseTypeCode VARCHAR(50) NOT NULL,
    HouseTypeName VARCHAR(255) NOT NULL,
    DisplayName VARCHAR(255) NOT NULL,
    Description TEXT NULL,
    Bedrooms INT NULL,
    Bathrooms INT NULL,
    Parking INT NULL,
    LandSize VARCHAR(100) NULL,
    UsableArea VARCHAR(100) NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE
);
```

**Project Facilities**
```sql
CREATE TABLE sy_project_facilities (
    FacilityID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    FacilityCode VARCHAR(50) NOT NULL,
    FacilityName VARCHAR(255) NOT NULL,
    Description TEXT NULL,
    Icon VARCHAR(100) NULL,
    Category ENUM('Recreation', 'Security', 'Landscaping', 'Parking', 'Other') DEFAULT 'Other',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE
);
```

**Project Features**
```sql
CREATE TABLE sy_project_features (
    FeatureID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    FeatureTitle VARCHAR(255) NOT NULL,
    FeatureDescription TEXT NULL,
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE
);
```

**Project Floor Plans**
```sql
CREATE TABLE sy_project_floor_plans (
    FloorPlanID INT AUTO_INCREMENT PRIMARY KEY,
    HouseTypeID INT NOT NULL,
    FloorPlanCode VARCHAR(50) NOT NULL,
    FloorPlanName VARCHAR(255) NOT NULL,
    ImagePath VARCHAR(500) NOT NULL,
    FloorType ENUM('Facade', 'GroundFloor', 'SecondFloor', 'ThirdFloor', 'Other') DEFAULT 'Other',
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (HouseTypeID) REFERENCES sy_project_house_types(HouseTypeID) ON DELETE CASCADE
);
```

**Project Locations**
```sql
CREATE TABLE sy_project_locations (
    LocationID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    District VARCHAR(255) NULL,
    Province VARCHAR(255) NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE,
    UNIQUE KEY unique_project_location (ProjectID)
);
```

**Project Nearby Places**
```sql
CREATE TABLE sy_project_nearby_places (
    NearbyPlaceID INT AUTO_INCREMENT PRIMARY KEY,
    LocationID INT NOT NULL,
    PlaceName VARCHAR(255) NOT NULL,
    Category VARCHAR(100) NULL,
    Distance VARCHAR(50) NULL,
    TravelTime VARCHAR(50) NULL,
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (LocationID) REFERENCES sy_project_locations(LocationID) ON DELETE CASCADE
);
```

**Project Contacts**
```sql
CREATE TABLE sy_project_contacts (
    ContactID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(15) NOT NULL,
    Phone VARCHAR(50) NULL,
    LineId VARCHAR(100) NULL,
    Facebook VARCHAR(255) NULL,
    OfficeHoursWeekdays VARCHAR(100) NULL,
    OfficeHoursWeekends VARCHAR(100) NULL,
    OfficeHoursHolidays VARCHAR(100) NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjectID) REFERENCES sy_project(ProjectID) ON DELETE CASCADE,
    UNIQUE KEY unique_project_contact (ProjectID)
);
```

### Phase 3: Migration Fixes
**Date:** October 12, 2025
**Issue:** ProjectID column size limitation

#### 3.1 ProjectID Size Extension
**Problem:** Original ProjectID varchar(15) too small for project identifiers like "ricco-residence-hathairat" (24 characters)

**Solution Applied:**
```sql
-- Extend main table
ALTER TABLE sy_project MODIFY ProjectID varchar(50) NOT NULL;

-- Update all foreign key columns
ALTER TABLE sy_project_images MODIFY ProjectID varchar(50) NOT NULL;
ALTER TABLE sy_project_house_types MODIFY ProjectID varchar(50) NOT NULL;
ALTER TABLE sy_project_facilities MODIFY ProjectID varchar(50) NOT NULL;
ALTER TABLE sy_project_features MODIFY ProjectID varchar(50) NOT NULL;
ALTER TABLE sy_project_locations MODIFY ProjectID varchar(50) NOT NULL;
ALTER TABLE sy_project_contacts MODIFY ProjectID varchar(50) NOT NULL;
```

**Result:** Migration successful - 6 projects migrated without errors

## Data Migration

### Migration Strategy
**Tool:** Custom C# migration service (`Data/DatabaseMigration.cs`)
**Source:** Static project data from `Services/StaticProjectService.cs`
**Target:** MySQL database tables

### Migration Mapping
**Static Model → Database Columns:**
```csharp
project.Name → ProjectName          // Thai name
project.NameEn → ProjectNameEN      // English name
project.Id → ProjectID              // Unique identifier
project.Details.Location → ProjectAddress
project.Type.ToString() → ProjectType
project.Subtitle → ProjectSubtitle
project.Description → ProjectDescription
project.Details.PriceRange → PriceRange
```

### Migration Results
**Date:** October 12, 2025
**Status:** ✅ SUCCESS
**Projects Migrated:** 6/6
- ricco-residence-hathairat
- ricco-residence-chatuchot
- ricco-town-phahonyothin-saimai53
- ricco-residence-prime-hathairat
- ricco-residence-prime-chatuchot
- ricco-town-wongwaen-lamlukka

## Performance Optimizations

### Indexes Added
```sql
CREATE INDEX idx_project_status ON sy_project(ProjectStatus);
CREATE INDEX idx_project_type ON sy_project(ProjectType);
CREATE INDEX idx_project_modified ON sy_project(ModifiedDate);
CREATE INDEX idx_project_type ON sy_project_images(ProjectID, ImageType);
CREATE INDEX idx_project_category ON sy_project_facilities(ProjectID, Category);
CREATE INDEX idx_project_order ON sy_project_features(ProjectID, SortOrder);
CREATE INDEX idx_housetype_type ON sy_project_floor_plans(HouseTypeID, FloorType);
CREATE INDEX idx_location_category ON sy_project_nearby_places(LocationID, Category);
```

## Application Integration

### Service Architecture
**HybridProjectService** implementation:
1. **DatabaseProjectService** - Primary data source
2. **StaticProjectService** - Fallback for reliability
3. **Automatic failover** - Zero downtime operation

### URL Routing Support
**Legacy Format:** `/Project/{id}`
**PPS Asset Format:** `/{projectType}/{projectName}/{location}`

**Mapping Examples:**
```
singlehouse/ricco-residence-ramintra/hathairat → ricco-residence-hathairat
townhome/thericcotown/phahonyothin_saimai53 → ricco-town-phahonyothin-saimai53
```

## Content Management Workflow

### Real-Time Updates
- ✅ Direct database edits via DBeaver
- ✅ Changes appear immediately on website
- ✅ No application restart required
- ✅ Changes persist across deployments

### Update Methods
1. **DBeaver GUI** - Visual editing
2. **SQL Commands** - Bulk operations
3. **Migration Re-run** - Reset to static data

## Backup and Recovery

### Backup Strategy
```bash
# Database backup
mysqldump -h localhost -u root thericco > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore from backup
mysql -h localhost -u root thericco < backup_file.sql
```

### Recovery Plan
1. **Database failure** → Automatic fallback to static data
2. **Data corruption** → Restore from backup + re-run migration
3. **Schema issues** → Apply schema fixes + data migration

## Monitoring and Maintenance

### Health Checks
- **Service Status Endpoint:** `/Home/ServiceStatus`
- **Migration Endpoint:** `/Home/MigrateDatabase` (dev only)

### Regular Maintenance
- **Database backups** - Daily recommended
- **Index optimization** - Monitor query performance
- **Schema updates** - Document all changes here

## Change Log Summary

| Date | Change Type | Description | Files Modified |
|------|-------------|-------------|----------------|
| 2025-10-11 | Schema Creation | Initial database setup | setup_database.sql |
| 2025-10-11 | Schema Extension | Content management tables | setup_database.sql |
| 2025-10-11 | Code Implementation | Migration and service classes | Data/, Services/ |
| 2025-10-12 | Schema Fix | ProjectID size increase | Direct ALTER TABLE |
| 2025-10-12 | Migration Success | All projects migrated | DatabaseMigration.cs |

## Future Considerations

### Planned Enhancements
- Admin interface for content management
- Image upload functionality
- Multi-language content support
- Advanced search and filtering
- Analytics and reporting

### Schema Evolution
- Version control for schema changes
- Migration scripts for production deployment
- Rollback procedures for failed migrations