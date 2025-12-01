# Phase 2: T_* Entity Migration Pattern

## Status: 3 of 18 Complete

### Completed Entities ✅
1. **T_Category** - Refactored from prototype (314 lines)
2. **T_Color** - Created from template (248 lines)
3. **T_Currency** - Created from template (248 lines)

### Remaining Entities (15)
- T_AgeGrading
- T_BarCode
- T_Charge
- T_City
- T_Class
- T_Country
- T_Dept
- T_Division
- T_Group
- T_Origin
- T_Package
- T_PaymentTerms
- T_Port
- T_Province
- T_Region
- T_Remarks
- T_ShippingMark
- T_UnitOfMeasures

---

## Step-by-Step Pattern

### Step 1: Identify Primary Key Property

View the EF6 entity file to find the primary key property name:

```powershell
# Example for T_AgeGrading
code xPort5.EF6/T_AgeGrading.cs
```

Look for the property that ends with `Id` (e.g., `AgeGradingId`, `ChargeId`, `CityId`)

### Step 2: Copy Template File

Use T_Color.Compatibility.cs as the template (simpler than T_Category):

```powershell
Copy-Item xPort5.EF6\T_Color.Compatibility.cs xPort5.EF6\T_AgeGrading.Compatibility.cs
```

### Step 3: Find and Replace

Replace all occurrences using your editor's find/replace:

| Find | Replace | Example |
|------|---------|---------|
| `T_Color` | `T_{EntityName}` | `T_AgeGrading` |
| `ColorId` | `{EntityName}Id` | `AgeGradingId` |
| `colorId` | `{entityName}Id` | `ageGradingId` |

**PowerShell Script** (automated):
```powershell
$entityName = "AgeGrading"  # Change this for each entity
$idProperty = "${entityName}Id"
$idParam = $idProperty.Substring(0,1).ToLower() + $idProperty.Substring(1)

$content = Get-Content "xPort5.EF6\T_Color.Compatibility.cs" -Raw
$content = $content -replace "T_Color", "T_$entityName"
$content = $content -replace "ColorId", $idProperty
$content = $content -replace "colorId", $idParam
Set-Content "xPort5.EF6\T_$entityName.Compatibility.cs" $content
```

### Step 4: Verify and Build

```powershell
dotnet build xPort5.EF6/xPort5.EF6.csproj
```

Fix any compilation errors (usually just missed replacements).

---

## Batch Creation Script

To create all 15 remaining entities at once:

```powershell
# List of entities to create
$entities = @(
    "AgeGrading",
    "BarCode",
    "Charge",
    "City",
    "Class",
    "Country",
    "Dept",
    "Division",
    "Group",
    "Origin",
    "Package",
    "PaymentTerms",
    "Port",
    "Province",
    "Region",
    "Remarks",
    "ShippingMark",
    "UnitOfMeasures"
)

foreach ($entityName in $entities) {
    Write-Host "Creating T_$entityName.Compatibility.cs..."
    
    $idProperty = "${entityName}Id"
    $idParam = $idProperty.Substring(0,1).ToLower() + $idProperty.Substring(1)
    
    $content = Get-Content "xPort5.EF6\T_Color.Compatibility.cs" -Raw
    $content = $content -replace "T_Color", "T_$entityName"
    $content = $content -replace "ColorId", $idProperty
    $content = $content -replace "colorId", $idParam
    
    Set-Content "xPort5.EF6\T_$entityName.Compatibility.cs" $content
}

Write-Host "Done! Building project..."
dotnet build xPort5.EF6/xPort5.EF6.csproj
```

---

## Special Cases

### Entities with Parent Relationships

Some entities may have parent/child relationships (like T_Category has DeptId). Check the EF6 entity file for nullable foreign key properties.

**Example**: T_Dept might have `ParentDeptId`

If parent filtering is needed, copy the `IgnoreThis` method from T_Category instead of T_Color.

### Entities with Different Property Names

If the primary key doesn't follow the `{EntityName}Id` pattern:
1. Check the EF6 entity file manually
2. Adjust the find/replace accordingly

---

## Verification Checklist

After creating each entity:

- [ ] File compiles without errors
- [ ] All `T_Color` references replaced with correct entity name
- [ ] All `ColorId` references replaced with correct property name
- [ ] Collection class name is correct (e.g., `T_AgeGradingCollection`)
- [ ] LoadCombo methods reference correct entity and property

---

## Estimated Time

- **Per entity**: 5-10 minutes (using script)
- **All 15 entities**: 1.5-2.5 hours
- **Testing**: 30 minutes

**Total**: ~2-3 hours to complete Phase 2

---

## Next Steps After Completion

1. Update `task.md` to mark Phase 2 as complete
2. Update OpenSpec `tasks.md` 
3. Test 2-3 entities in actual VWG forms
4. Proceed to Phase 3 (Partner Management)
