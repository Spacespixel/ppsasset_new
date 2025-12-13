# ProjectID Mapping Implementation Guide

## Overview
This document describes the implementation of mapping string-based ProjectIDs to numeric MappedProjectIDs from the `sy_project_mapping` table for backward compatibility with legacy transaction data in the `tr_transaction` table.

## Problem Statement
The new PPS Asset system uses string-based ProjectIDs (e.g., "ricco-residence-hathairat"), but the legacy `tr_transaction` table stores numeric ProjectID values for backward compatibility with old data. The `sy_project_mapping` table contains a `MappingID` that provides this conversion.

### Before Implementation
```sql
-- tr_transaction stores string ProjectID
INSERT INTO tr_transaction (ProjectID, FirstName, ...)
VALUES ('ricco-residence-hathairat', 'John', ...)
```

### After Implementation
```sql
-- tr_transaction stores numeric MappedProjectID for old data compatibility
INSERT INTO tr_transaction (ProjectID, FirstName, ...)
VALUES (1, 'John', ...)  -- MappingID from sy_project_mapping
```

## Architecture

### 1. New Service: ProjectMappingService
**File**: `Services/ProjectMappingService.cs`

#### Interface: IProjectMappingService
```csharp
public interface IProjectMappingService
{
    /// <summary>
    /// Get the numeric MappedProjectID for a given project string ID
    /// </summary>
    Task<int?> GetMappedProjectIdAsync(string projectId);

    /// <summary>
    /// Get project string ID from numeric MappedProjectID
    /// </summary>
    Task<string?> GetProjectIdFromMappedIdAsync(int mappedProjectId);
}
```

#### Implementation: ProjectMappingService
- Connects to MySQL database
- Queries `sy_project_mapping` table by ProjectID
- Returns MappingID (numeric) for transaction recording
- Includes logging for debugging and auditing
- Handles null/empty cases gracefully

### 2. Updated Service: RegistrationService
**File**: `Services/RegistrationService.cs`

#### Key Changes
1. **Dependency Injection**: Added `IProjectMappingService` to constructor
2. **Mapping Logic**: Retrieves numeric MappedProjectID before saving transaction
3. **Parameter Update**: Uses numeric MappedProjectID instead of string ProjectID
4. **Enhanced Logging**: Logs both string ProjectID and numeric MappedProjectID

#### Code Flow
```csharp
public async Task SaveAsync(RegistrationInputModel input, HttpRequest request)
{
    // Get numeric MappedProjectID for transaction recording
    var mappedProjectId = await _projectMappingService.GetMappedProjectIdAsync(input.ProjectID);

    // Use numeric ID in database insert
    parameters.Add("@ProjectID", mappedProjectId ?? 0);

    // Log both IDs for traceability
    _logger.LogInformation(
        "Inserted transaction {TransactionId} for project {ProjectId} (MappedProjectID: {MappedProjectId})",
        transactionId, input.ProjectID, mappedProjectId ?? 0);
}
```

### 3. Service Registration
**File**: `Program.cs`

Added service registration:
```csharp
// ProjectMappingService - maps string project IDs to numeric MappedProjectID
// for backward compatibility with old transaction data
builder.Services.AddScoped<IProjectMappingService, ProjectMappingService>();
```

## Database Tables

### sy_project_mapping Table
Structure:
```sql
CREATE TABLE sy_project_mapping (
    MappingID INT AUTO_INCREMENT PRIMARY KEY,
    ProjectID VARCHAR(50) NOT NULL,           -- String ID (e.g., "SG06")
    ProjectType VARCHAR(50) NOT NULL,          -- "singlehouse", "townhome", etc.
    ProjectName VARCHAR(255) NOT NULL,         -- "thericcoresidence", etc.
    Location VARCHAR(255) NOT NULL,            -- URL location format
    UrlPattern VARCHAR(500),                   -- Full URL pattern
    IsActive BOOLEAN DEFAULT 1,                -- Enable/disable mapping
    CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;
```

### tr_transaction Table
Relevant columns:
```sql
CREATE TABLE tr_transaction (
    TransactoinID VARCHAR(15) NOT NULL PRIMARY KEY,
    ProjectID VARCHAR(15) DEFAULT NULL,        -- Now stores numeric MappedProjectID
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) DEFAULT NULL,
    TransactionDate DATETIME DEFAULT NULL,
    -- ... other fields
) ENGINE=InnoDB;
```

## Query Mapping

### Get MappedProjectID from ProjectID
```sql
SELECT MappingID
FROM sy_project_mapping
WHERE ProjectID = @ProjectId
  AND IsActive = 1
LIMIT 1
```

Example:
- Input: "SG06"
- Output: 1

### Get ProjectID from MappedProjectID
```sql
SELECT ProjectID
FROM sy_project_mapping
WHERE MappingID = @MappingId
  AND IsActive = 1
LIMIT 1
```

Example:
- Input: 1
- Output: "SG06"

## Data Flow

### Transaction Recording Process
```
1. User submits registration form
   ├─ ProjectID = "ricco-residence-hathairat"
   └─ Other fields (name, email, etc.)

2. RegisterProject action receives input
   └─ Calls RegistrationService.SaveAsync()

3. RegistrationService.SaveAsync()
   ├─ Calls IProjectMappingService.GetMappedProjectIdAsync("ricco-residence-hathairat")
   │  └─ Queries sy_project_mapping for MappingID
   │
   ├─ Gets numeric MappedProjectID (e.g., 1)
   │
   └─ Inserts into tr_transaction
      └─ ProjectID field = 1 (numeric)

4. Transaction recorded with numeric ProjectID
   └─ Backward compatible with legacy data
```

## Implementation Benefits

### 1. Backward Compatibility
- Old and new transaction data coexist in same table
- Numeric ProjectID works with legacy systems
- No data migration required

### 2. Traceability
- Logs contain both string and numeric ProjectIDs
- Easy to debug mapping issues
- Full audit trail maintained

### 3. Flexibility
- Easy to add new project mappings
- Mappings managed in database
- Can disable/enable without code changes

### 4. Error Handling
- Graceful handling of missing mappings (defaults to 0)
- Detailed error logging
- No transaction failures due to mapping issues

## Testing

### Unit Test Scenarios

1. **Happy Path**: Valid ProjectID maps to MappedProjectID
   ```csharp
   var mappedId = await service.GetMappedProjectIdAsync("SG06");
   Assert.Equal(1, mappedId);
   ```

2. **Invalid ProjectID**: Returns null
   ```csharp
   var mappedId = await service.GetMappedProjectIdAsync("INVALID");
   Assert.Null(mappedId);
   ```

3. **Null Input**: Returns null safely
   ```csharp
   var mappedId = await service.GetMappedProjectIdAsync(null);
   Assert.Null(mappedId);
   ```

4. **Database Error**: Logs error and returns null
   ```csharp
   // Service handles connection errors gracefully
   ```

### Integration Test

1. Register user for "ricco-residence-hathairat"
2. Check `tr_transaction` table
3. Verify ProjectID field contains numeric MappedProjectID
4. Verify logs contain both IDs

### Example Test Result
```
Transaction recorded:
├─ TransactionID: 250113010934901
├─ ProjectID: 1 (numeric MappedProjectID)
├─ FirstName: test
├─ Email: test@gmail.com
└─ Log: "Inserted transaction 250113010934901 for project SG06 (MappedProjectID: 1)"
```

## Migration Guide

### For Existing Projects

1. **Ensure sy_project_mapping is populated**
   ```sql
   SELECT COUNT(*) FROM sy_project_mapping WHERE IsActive = 1;
   ```

2. **Verify ProjectID mappings**
   ```sql
   SELECT ProjectID, MappingID FROM sy_project_mapping;
   ```

3. **Deploy changes**
   - Update to latest code
   - No database schema changes required
   - Service registration automatic

4. **Verify new transactions**
   ```sql
   SELECT ProjectID FROM tr_transaction
   WHERE TransactionDate > NOW() - INTERVAL 1 HOUR;
   -- ProjectID should be numeric for new transactions
   ```

### Handling Legacy Data

Legacy transactions may have string ProjectIDs. To convert:
```sql
-- View mapping for conversion
SELECT * FROM sy_project_mapping
WHERE IsActive = 1;

-- Update legacy transactions (if needed)
UPDATE tr_transaction t
SET t.ProjectID = (
    SELECT CAST(m.MappingID AS CHAR)
    FROM sy_project_mapping m
    WHERE m.ProjectID = t.ProjectID
    AND m.IsActive = 1
)
WHERE t.ProjectID IN (
    SELECT ProjectID FROM sy_project_mapping
);
```

## Configuration

### Database Connection
Uses standard connection string from `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User=...;Password=..."
  }
}
```

### Logging
Service uses structured logging:
```csharp
_logger.LogInformation("Found MappedProjectID {MappedProjectId} for ProjectID {ProjectId}");
_logger.LogWarning("No MappedProjectID found for ProjectID {ProjectId}");
_logger.LogError(ex, "Error retrieving MappedProjectID for ProjectID {ProjectId}");
```

## Troubleshooting

### MappedProjectID Returns Null

**Symptoms**: New transactions have ProjectID = 0

**Causes**:
1. ProjectID not found in sy_project_mapping
2. Mapping marked as inactive (IsActive = 0)
3. Database connection issue

**Solution**:
1. Check sy_project_mapping table
   ```sql
   SELECT * FROM sy_project_mapping WHERE IsActive = 1;
   ```
2. Verify ProjectID exists
   ```sql
   SELECT * FROM sy_project_mapping WHERE ProjectID = 'YOUR_PROJECT_ID';
   ```
3. Check application logs for errors

### Mapping Service Timeout

**Symptoms**: Registration slow or times out

**Causes**:
1. Database query slow
2. Network latency
3. Large sy_project_mapping table

**Solution**:
1. Add index if missing
   ```sql
   CREATE INDEX idx_sy_project_mapping_projectid
   ON sy_project_mapping(ProjectID, IsActive);
   ```
2. Monitor query performance
3. Consider caching for high-traffic scenarios

## Future Enhancements

### 1. Caching Layer
```csharp
// Add caching to reduce database queries
public class CachedProjectMappingService : IProjectMappingService
{
    private readonly IMemoryCache _cache;
    private readonly ProjectMappingService _innerService;
    // Implementation...
}
```

### 2. Bulk Mapping
```csharp
// Get multiple mappings in single query
Task<Dictionary<string, int?>> GetMultipleMappedIdsAsync(params string[] projectIds);
```

### 3. Reverse Mapping Endpoint
```csharp
// API endpoint to look up ProjectID from MappedProjectID
[HttpGet("/api/projects/{mappedId}/lookup")]
public async Task<IActionResult> LookupProjectId(int mappedId) { }
```

## Summary

This implementation provides:
- ✅ Backward compatibility with legacy transaction data
- ✅ Transparent mapping between string and numeric ProjectIDs
- ✅ Database-driven configuration (no code changes needed for new mappings)
- ✅ Comprehensive logging and error handling
- ✅ Easy integration via dependency injection
- ✅ Scalable architecture for future enhancements
