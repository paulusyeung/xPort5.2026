# Task 8.6 Dryrun: Update 2-3 Pilot Forms to Use xPort5.EF6

## Overview
This document outlines what will be modified when updating 2-3 pilot forms in the Settings module to use `xPort5.EF6` instead of `xPort5.DAL`.

## Selected Pilot Forms

Based on the Settings module structure, I recommend updating these 3 forms:

1. **Group** (`xPort5/Settings/Coding/Group/`)
   - `GroupList.cs` - List view form
   - `GroupRecord.cs` - Record edit form

2. **Division** (`xPort5/Settings/Coding/Division/`)
   - `DivisionList.cs` - List view form
   - `DivisionRecord.cs` - Record edit form

3. **Currency** (`xPort5/Order/Coding/Currency/`) - Note: This is in Order module but uses T_Currency
   - `CurrencyList.cs` - List view form
   - `CurrencyRecord.cs` - Record edit form

## Changes Required

### 1. Using Statement Changes

**Files to modify:**
- `xPort5/Settings/Coding/Group/GroupList.cs`
- `xPort5/Settings/Coding/Group/GroupRecord.cs`
- `xPort5/Settings/Coding/Division/DivisionList.cs`
- `xPort5/Settings/Coding/Division/DivisionRecord.cs`
- `xPort5/Order/Coding/Currency/CurrencyList.cs`
- `xPort5/Order/Coding/Currency/CurrencyRecord.cs`

**Change:**
```csharp
// BEFORE:
using xPort5.DAL;

// AFTER:
using xPort5.EF6;
```

### 2. GroupRecord.cs Changes

**File:** `xPort5/Settings/Coding/Group/GroupRecord.cs`

**Line 132:** `T_Group.Load(_GroupId)`
- ✅ No change needed - EF6 compatibility layer has `Load(Guid)` method
- Method signature matches: `public static T_Group Load(Guid GroupId)`

**Line 158:** `T_Group.Load(_GroupId)`
- ✅ No change needed - same as above

**Line 200:** `T_Group.LoadWhere(sql)`
- ✅ No change needed - EF6 compatibility layer has `LoadWhere(string)` method
- Method signature matches: `public static T_Group LoadWhere(string whereClause)`

**Line 154:** `new T_Group()`
- ✅ No change needed - EF6 entity has default constructor

**Line 165:** `group.Save()`
- ✅ No change needed - EF6 compatibility layer has instance `Save()` method
- Method signature matches: `public void Save()`

**Line 233:** `T_Group.Delete(_GroupId)`
- ✅ No change needed - EF6 compatibility layer has `Delete(Guid)` method
- Method signature matches: `public static void Delete(Guid GroupId)`

**Summary for GroupRecord.cs:**
- Change: `using xPort5.DAL;` → `using xPort5.EF6;`
- All method calls remain identical (backward compatible API)

### 3. DivisionRecord.cs Changes

**File:** `xPort5/Settings/Coding/Division/DivisionRecord.cs`

**Line 133:** `T_Division.Load(_DivisionId)`
- ✅ No change needed - EF6 compatibility layer has `Load(Guid)` method

**Line 159:** `T_Division.Load(_DivisionId)`
- ✅ No change needed - same as above

**Line 201:** `T_Group.LoadWhere(sql)` - **NOTE: This appears to be a bug (should be T_Division)**
- ⚠️ This line uses `T_Group` but should use `T_Division` for code validation
- After migration: `T_Division.LoadWhere(sql)`

**Line 155:** `new T_Division()`
- ✅ No change needed - EF6 entity has default constructor

**Line 170:** `div.Save()`
- ✅ No change needed - EF6 compatibility layer has instance `Save()` method

**Line 234:** `T_Division.Delete(_DivisionId)`
- ✅ No change needed - EF6 compatibility layer has `Delete(Guid)` method

**Summary for DivisionRecord.cs:**
- Change: `using xPort5.DAL;` → `using xPort5.EF6;`
- Fix bug on line 201: `T_Group.LoadWhere` → `T_Division.LoadWhere`
- All other method calls remain identical

### 4. CurrencyRecord.cs Changes

**File:** `xPort5/Order/Coding/Currency/CurrencyRecord.cs`

**Line 143:** `T_Currency.Load(_CurrencyId)`
- ✅ No change needed - EF6 compatibility layer has `Load(Guid)` method

**Line 179:** `T_Currency.Load(_CurrencyId)`
- ✅ No change needed - same as above

**Line 224:** `T_Currency.LoadWhere(sql)`
- ✅ No change needed - EF6 compatibility layer has `LoadWhere(string)` method

**Line 175:** `new T_Currency()`
- ✅ No change needed - EF6 entity has default constructor

**Line 195:** `item.Save()`
- ✅ No change needed - EF6 compatibility layer has instance `Save()` method

**Line 276:** `T_Currency.Delete(_CurrencyId)`
- ✅ No change needed - EF6 compatibility layer has `Delete(Guid)` method

**Summary for CurrencyRecord.cs:**
- Change: `using xPort5.DAL;` → `using xPort5.EF6;`
- All method calls remain identical (backward compatible API)

### 5. List Forms (GroupList.cs, DivisionList.cs, CurrencyList.cs)

**Files:**
- `xPort5/Settings/Coding/Group/GroupList.cs`
- `xPort5/Settings/Coding/Division/DivisionList.cs`
- `xPort5/Order/Coding/Currency/CurrencyList.cs`

**Current Implementation:**
These forms use **raw SQL queries** with `SqlHelper.Default.ExecuteReader()` to populate ListView controls. They do NOT directly use DAL entity methods.

**Options:**
1. **Option A (Recommended for Pilot):** Keep SQL-based approach for now
   - Only change: `using xPort5.DAL;` → `using xPort5.EF6;` (if used for other purposes)
   - No functional changes to data loading
   - Lower risk, faster migration

2. **Option B (Future Enhancement):** Refactor to use EF6 LoadCollection
   - Replace SQL queries with `T_Group.LoadCollection(whereClause, orderBy, true)`
   - More work, but better aligns with migration goals
   - Can be done in a follow-up task

**Recommendation:** Use Option A for the pilot to minimize risk and validate the Record forms first.

## Summary of Changes

### Files to Modify: 6 files

1. ✅ `xPort5/Settings/Coding/Group/GroupList.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`

2. ✅ `xPort5/Settings/Coding/Group/GroupRecord.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`
   - All method calls remain identical

3. ✅ `xPort5/Settings/Coding/Division/DivisionList.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`

4. ✅ `xPort5/Settings/Coding/Division/DivisionRecord.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`
   - **Bug fix:** Line 201: `T_Group.LoadWhere` → `T_Division.LoadWhere`

5. ✅ `xPort5/Order/Coding/Currency/CurrencyList.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`

6. ✅ `xPort5/Order/Coding/Currency/CurrencyRecord.cs`
   - Change: `using xPort5.DAL;` → `using xPort5.EF6;`
   - All method calls remain identical

### Project Reference Changes

**File:** `xPort5/xPort5.csproj`

**Add project reference:**
```xml
<ProjectReference Include="..\xPort5.EF6\xPort5.EF6.csproj">
  <Project>{573ea8d8-e09d-49fb-8cc2-1dcf78b3cf4e}</Project>
  <Name>xPort5.EF6</Name>
</ProjectReference>
```

**Note:** May need to remove or keep `xPort5.DAL` reference depending on other dependencies.

## Testing Checklist

After making changes, test:

### Group Form
- [ ] Load Group list (GroupList.cs)
- [ ] Create new Group (GroupRecord.cs - Add mode)
- [ ] Edit existing Group (GroupRecord.cs - Edit mode)
- [ ] Delete Group (GroupRecord.cs - Delete)
- [ ] Validate duplicate code detection
- [ ] Test all field updates (Code, Name, Name_Chs, Name_Cht)

### Division Form
- [ ] Load Division list (DivisionList.cs)
- [ ] Create new Division (DivisionRecord.cs - Add mode)
- [ ] Edit existing Division (DivisionRecord.cs - Edit mode)
- [ ] Delete Division (DivisionRecord.cs - Delete)
- [ ] Validate duplicate code detection (verify bug fix works)
- [ ] Test all field updates (Code, Name, Name_Chs, Name_Cht)

### Currency Form
- [ ] Load Currency list (CurrencyList.cs)
- [ ] Create new Currency (CurrencyRecord.cs - Add mode)
- [ ] Edit existing Currency (CurrencyRecord.cs - Edit mode)
- [ ] Delete Currency (CurrencyRecord.cs - Delete)
- [ ] Validate duplicate code detection
- [ ] Test all field updates (Code, Name, Name_Chs, Name_Cht, XchgRate, LocalCurrency)

## Risk Assessment

**Low Risk:**
- Record forms use standard CRUD operations that are fully compatible
- EF6 compatibility layer maintains identical API signatures
- Only namespace changes required

**Medium Risk:**
- List forms use raw SQL - may need refactoring later
- Need to verify project references don't break other code

**Mitigation:**
- Start with Record forms (higher value, lower risk)
- Keep List forms using SQL for now (Option A)
- Test thoroughly before proceeding to more forms

## Estimated Effort

- **Using statement changes:** 5 minutes
- **Bug fix (DivisionRecord.cs):** 2 minutes
- **Project reference update:** 5 minutes
- **Testing:** 30-45 minutes
- **Total:** ~1 hour

## Next Steps After Pilot

If pilot is successful:
1. Update remaining Settings forms (Task 8.8)
2. Consider refactoring List forms to use EF6 LoadCollection (optional enhancement)
3. Remove xPort5.DAL references once all forms migrated (Task 8.12)

