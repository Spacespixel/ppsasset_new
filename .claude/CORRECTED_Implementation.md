# CORRECTED: ProjectID Mapping Implementation

## What Was Fixed

The initial implementation was incorrect because it was querying `MappingID` (auto-increment primary key) instead of `MappedProjectID` (the legacy code column).

### What You Actually Want

```
Input:  ProjectID = "ricco-residence-hathairat" (from form)
Query:  sy_project_mapping WHERE ProjectID = "ricco-residence-hathairat"
Output: MappedProjectID = "SG06" (legacy code)
Store:  tr_transaction.ProjectID = "SG06"
```

## Corrected Table Schema

Your `sy_project_mapping` table has these relevant columns:

```sql
CREATE TABLE sy_project_mapping (
    -- ... other columns ...
    ProjectID VARCHAR(50),           -- Internal string ID (e.g., "ricco-residence-hathairat")
    MappedProjectID VARCHAR(50),     -- Legacy code (e.g., "SG06", "SG01", "TH01", "TH02", "TH03")
    IsActive BOOLEAN
) ENGINE=InnoDB;
```

### Example Data
```
ProjectID                          | MappedProjectID
-----------------------------------|----------------
ricco-residence-wongwaen-chatuchot | SG01
ricco-residence-wongwaen-hathairat | SG02
ricco-residence-prime-hathairat    | SG03
ricco-residence-prime-chatuchot    | SG04
ricco-residence-chatuchot          | SG05
ricco-residence-hathairat          | SG06
ricco-town-phahonyothin-watchraphol| TH01
ricco-town-wongwaen-lamlukka       | TH02
ricco-town-phahonyothin-saimai53   | TH03
```

## Corrected Service Implementation

### File: `Services/ProjectMappingService.cs`

**Key Change**: Query `MappedProjectID` column instead of `MappingID`

```csharp
public async Task<string?> GetMappedProjectIdAsync(string projectId)
{
    // Query sy_project_mapping to get MappedProjectID
    const string query = @"
        SELECT MappedProjectID
        FROM sy_project_mapping
        WHERE ProjectID = @ProjectId
        AND IsActive = 1
        LIMIT 1";

    var result = await connection.QueryFirstOrDefaultAsync<string>(
        query,
        new { ProjectId = projectId });

    return result;  // Returns "SG06", "TH01", etc.
}
```

### File: `Services/RegistrationService.cs`

**Key Changes**:
1. Uses string MappedProjectID (not numeric)
2. Defaults to empty string if not found (not 0)

```csharp
// Get the MappedProjectID (legacy code like SG06, TH01)
var mappedProjectId = await _projectMappingService.GetMappedProjectIdAsync(input.ProjectID);

// Store in transaction with legacy code
parameters.Add("@ProjectID", mappedProjectId ?? string.Empty);  // e.g., "SG06"
```

## How It Works Now

```
1. User registers on project page "ricco-residence-hathairat"
   └─ ProjectID = "ricco-residence-hathairat"

2. RegistrationService.SaveAsync() calls ProjectMappingService
   └─ Query: SELECT MappedProjectID FROM sy_project_mapping
      WHERE ProjectID = "ricco-residence-hathairat"

3. Database returns: "SG06"

4. Insert into tr_transaction
   └─ ProjectID field = "SG06" ✓ (legacy format, backward compatible)

5. Transaction saved successfully with legacy code
```

## Data Flow Comparison

### Before Fix (WRONG ❌)
```
"ricco-residence-hathairat"
  → Query MappingID (wrong column)
  → Get 1, 2, 3... (auto-increment IDs)
  → Store 0 (defaults when not found)
  → ProjectID = 0 ❌
```

### After Fix (CORRECT ✅)
```
"ricco-residence-hathairat"
  → Query MappedProjectID (correct column)
  → Get "SG06"
  → Store "SG06"
  → ProjectID = "SG06" ✓
```

## Testing

### 1. Verify Table Structure
```sql
SELECT ProjectID, MappedProjectID
FROM sy_project_mapping
WHERE IsActive = 1;
```

Expected output:
```
ricco-residence-hathairat          | SG06
ricco-residence-chatuchot          | SG05
ricco-residence-prime-hathairat    | SG03
... etc
```

### 2. Test Registration
1. Go to project page (e.g., ricco-residence-hathairat)
2. Fill registration form
3. Submit
4. Check database:
   ```sql
   SELECT ProjectID, FirstName, TransactionDate
   FROM tr_transaction
   ORDER BY TransactionDate DESC
   LIMIT 1;
   ```

Expected result:
```
ProjectID | FirstName | TransactionDate
----------|-----------|------------------
SG06      | test      | 2025-11-13 ...
```

### 3. Check Logs
Expected log message:
```
Inserted transaction 250113XXXXXXXXX for project ricco-residence-hathairat (MappedProjectID: SG06)
```

## Key Differences from Initial Implementation

| Aspect | Before (Wrong) | After (Correct) |
|--------|---|---|
| Query Column | `MappingID` | `MappedProjectID` |
| Return Type | `int?` | `string?` |
| Fallback Value | `0` | `string.Empty` |
| Example Value | 1, 2, 3 | "SG06", "TH01", "TH02" |
| Storage | Numeric | String (legacy format) |

## Files Modified

1. **Services/ProjectMappingService.cs**
   - Changed return type from `int?` to `string?`
   - Changed query to select `MappedProjectID` instead of `MappingID`
   - Updated interface signatures

2. **Services/RegistrationService.cs**
   - Changed fallback from `?? 0` to `?? string.Empty`
   - Updated logging to show string MappedProjectID

3. **Program.cs** (no changes needed - already registered)

## Backward Compatibility

✅ Stores legacy format ("SG06", "TH01", etc.) for compatibility with old data
✅ No database schema changes required
✅ Transparent to UI and API
✅ Works with existing sy_project_mapping table structure

## Summary

You maintain the sy_project_mapping table with:
- `ProjectID` = Internal system IDs (string)
- `MappedProjectID` = Legacy codes (string: SG01-SG06, TH01-TH03)

The implementation now correctly maps string ProjectID → MappedProjectID and stores the legacy code in transactions.
