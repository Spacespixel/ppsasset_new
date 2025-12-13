# ProjectID Mapping Implementation - Quick Summary

## What Was Changed

### New File Created
- **Services/ProjectMappingService.cs** - Service to map string ProjectIDs to numeric MappedProjectIDs

### Files Modified
1. **Services/RegistrationService.cs**
   - Added `IProjectMappingService` dependency
   - Modified `SaveAsync()` to retrieve and use numeric MappedProjectID
   - Enhanced logging with both string and numeric IDs

2. **Program.cs**
   - Registered `IProjectMappingService` in dependency injection container

## Why This Was Needed

The `tr_transaction` table expects numeric ProjectID values for backward compatibility with old data, but the new system uses string ProjectIDs (e.g., "ricco-residence-hathairat"). The `sy_project_mapping` table provides the mapping via `MappingID`.

## How It Works

```
User Registration
    ↓
Input: ProjectID = "ricco-residence-hathairat"
    ↓
RegistrationService calls ProjectMappingService
    ↓
Query sy_project_mapping: WHERE ProjectID = "ricco-residence-hathairat"
    ↓
Get MappingID = 1 (numeric)
    ↓
Insert into tr_transaction with ProjectID = 1
    ↓
Transaction saved with numeric ProjectID ✓
```

## Data Change Example

### Before (Would have been storing string)
```
tr_transaction.ProjectID = "ricco-residence-hathairat"
```

### After (Now stores numeric)
```
tr_transaction.ProjectID = 1  ← From sy_project_mapping.MappingID
```

## Key Features

✅ **Automatic Mapping** - Happens transparently during registration
✅ **Backward Compatible** - Works with legacy data in same table
✅ **Logged Traceability** - Both IDs recorded in logs
✅ **Error Handling** - Graceful fallback if mapping not found
✅ **Database Driven** - Mappings managed in database, no code changes needed

## Database Queries Used

### Get MappedProjectID
```sql
SELECT MappingID FROM sy_project_mapping
WHERE ProjectID = @ProjectId AND IsActive = 1
LIMIT 1
```

### Get ProjectID (reverse lookup)
```sql
SELECT ProjectID FROM sy_project_mapping
WHERE MappingID = @MappingId AND IsActive = 1
LIMIT 1
```

## How to Verify

1. **Register a user on a project page**
   - ProjectID shows as string in form

2. **Check database**
   ```sql
   SELECT * FROM tr_transaction
   WHERE TransactionDate > NOW() - INTERVAL 1 HOUR;
   ```
   - ProjectID column should contain numeric value (e.g., 1, 2, 3)

3. **Check logs**
   - Should show: "Inserted transaction XXX for project ricco-residence-hathairat (MappedProjectID: 1)"

## Current Project Mappings

From `sy_project_mapping`:
```
SG06 → MappingID: 1  (Ricco Residence Ramintra Hathairat)
SG05 → MappingID: 2  (Ricco Residence Ramintra Chatuchot)
SG03 → MappingID: 3  (Ricco Residence Prime Wongwaen Hathairat)
SG04 → MappingID: 4  (Ricco Residence Prime Wongwaen Chatuchot)
TH02 → MappingID: 5  (Ricco Town Phahonyothin Saimai 53)
TH01 → MappingID: 6  (Ricco Town Wongwaen Lumlukka)
TH03 → MappingID: 7  (Twin House Lamlukka)
```

## Implementation Files Reference

| File | Purpose | Changes |
|------|---------|---------|
| `Services/ProjectMappingService.cs` | NEW - Maps ProjectID to MappedProjectID | Interface + Implementation |
| `Services/RegistrationService.cs` | Modified - Uses mapping in registration | Added dependency, updated SaveAsync() |
| `Program.cs` | Modified - Register service | Added service registration |

## Testing Steps

1. **Unit Test**: Create service instance, test mapping
2. **Integration Test**: Register user, verify transaction ProjectID is numeric
3. **Manual Test**:
   - Go to project page
   - Fill registration form
   - Submit
   - Check database for numeric ProjectID

## No Breaking Changes

✅ API endpoints unchanged
✅ UI/Frontend unchanged
✅ Registration form unchanged
✅ No database schema changes
✅ Backward compatible with existing data

## Questions?

For detailed information, see: `.claude/ProjectID_Mapping_Implementation.md`
