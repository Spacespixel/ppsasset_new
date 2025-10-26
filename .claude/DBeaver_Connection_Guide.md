# DBeaver Connection Guide for PPS Asset Database

## Connection Information

### Database Details
- **Database Type:** MySQL
- **Host:** localhost
- **Port:** 3306
- **Database Name:** thericco
- **Username:** root
- **Password:** (empty)

## Step-by-Step Connection Setup

### 1. Create New Connection
1. Open DBeaver
2. Click "New Database Connection" (+ icon in toolbar)
3. Select "MySQL" from the database list
4. Click "Next"

### 2. Main Connection Settings
Fill in these exact values:

```
Server Host: localhost
Port: 3306
Database: thericco
Username: root
Password: (leave empty)
```

### 3. Connection Properties (Optional but Recommended)
- **Connection Name:** `PPS Asset Local`
- **Connection Type:** Development
- **Description:** Local MySQL database for PPS Asset project

### 4. Test Connection
1. Click "Test Connection" button
2. You should see "Connected" message
3. If MySQL driver is missing, DBeaver will download it automatically

### 5. Advanced Settings (Optional)
- **Character Set:** utf8mb4
- **Collation:** utf8mb4_unicode_ci

## Database Structure Overview

### Main Tables for Content Management

#### 1. `sy_project` - Main Project Information
- **ProjectID** (varchar(50)) - Primary key, project identifier
- **ProjectName** (varchar(500)) - Thai project name
- **ProjectNameEN** (varchar(100)) - English project name
- **ProjectSubtitle** (varchar(1000)) - Project tagline
- **ProjectDescription** (text) - Main description
- **ProjectType** (varchar(255)) - SingleHouse, Townhome, TwinHouse
- **ProjectStatus** (enum) - Available, SoldOut, ComingSoon
- **PriceRange** (varchar(100)) - Price range information

#### 2. `sy_project_images` - Project Images
- **ProjectID** - Foreign key to sy_project
- **ImageType** - Hero, Logo, Gallery, Facility, FloorPlan
- **ImagePath** - Path to image file
- **ImageAlt** - Alt text for SEO
- **SortOrder** - Display order

#### 3. `sy_project_facilities` - Project Amenities
- **ProjectID** - Foreign key to sy_project
- **FacilityName** - Facility name (Thai)
- **Description** - Facility description
- **Icon** - CSS icon class
- **Category** - Recreation, Security, Landscaping, Parking, Other

#### 4. `sy_project_house_types` - House Type Information
- **ProjectID** - Foreign key to sy_project
- **HouseTypeName** - House type name
- **Bedrooms** - Number of bedrooms
- **Bathrooms** - Number of bathrooms
- **LandSize** - Land area
- **UsableArea** - Usable area

#### 5. `sy_project_contacts` - Contact Information
- **ProjectID** - Foreign key to sy_project
- **Phone** - Contact phone
- **LineId** - LINE ID
- **Facebook** - Facebook page
- **OfficeHours*** - Office hours for different days

## Quick Queries for Content Management

### View All Projects
```sql
SELECT ProjectID, ProjectName, ProjectNameEN, ProjectStatus, PriceRange
FROM sy_project
ORDER BY ProjectName;
```

### View Project with Images
```sql
SELECT p.ProjectName, i.ImageType, i.ImagePath, i.SortOrder
FROM sy_project p
LEFT JOIN sy_project_images i ON p.ProjectID = i.ProjectID
WHERE p.ProjectID = 'ricco-residence-hathairat'
ORDER BY i.ImageType, i.SortOrder;
```

### View Project Facilities
```sql
SELECT p.ProjectName, f.FacilityName, f.Description, f.Category
FROM sy_project p
LEFT JOIN sy_project_facilities f ON p.ProjectID = f.ProjectID
WHERE p.ProjectID = 'ricco-residence-hathairat'
ORDER BY f.Category, f.FacilityName;
```

### Update Project Information
```sql
-- Update project description
UPDATE sy_project
SET ProjectDescription = 'Your new description here',
    PriceRange = 'New price range'
WHERE ProjectID = 'ricco-residence-hathairat';
```

### Add New Image
```sql
-- Add new gallery image
INSERT INTO sy_project_images (ProjectID, ImageType, ImagePath, ImageAlt, SortOrder)
VALUES ('ricco-residence-hathairat', 'Gallery', '/images/new-image.jpg', 'New image description', 10);
```

## Important Notes

### Real-Time Updates
- Changes made in DBeaver appear immediately on the website
- No application restart required
- Database changes are persistent

### Data Safety
- Always backup before major changes
- Test queries with SELECT before UPDATE/DELETE
- Use transactions for multiple related changes

### Character Encoding
- Database uses utf8mb4 for full Unicode support
- Supports Thai characters properly
- Emoji support included

## Troubleshooting

### Connection Issues
1. **Can't connect:** Ensure MySQL is running (`brew services start mysql`)
2. **Access denied:** Check username is `root` with empty password
3. **Database not found:** Verify database name is `thericco`

### Performance Tips
- Use indexes when querying large datasets
- Limit results with `LIMIT` clause for large tables
- Use `WHERE` clauses to filter specific projects

## Integration with Application

The application uses:
- **HybridProjectService:** Database-first with static fallback
- **Real-time content:** Changes appear immediately
- **SEO URLs:** Both legacy and PPS Asset URL formats supported

Current URL formats:
- Legacy: `/Project/ricco-residence-hathairat`
- PPS Asset: `/singlehouse/ricco-residence-ramintra/hathairat`

Both formats load the same project data from the database.