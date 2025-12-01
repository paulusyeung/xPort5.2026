# Manual Fix Guide for 56 xPort5 Build Errors

This guide provides exact changes to make in Visual Studio for each file.

## How to Use This Guide

For each file:
1. Open the file in Visual Studio
2. Use Ctrl+H (Find and Replace)
3. Follow the instructions for each fix
4. Save the file
5. Move to the next file

---

## File 1: CalcWizard.cs

**Location**: `xPort5\Controls\CalcWizard.cs`

### Fix 1 (Line ~299): Nullable Guid for UomId

**Find** (around line 299):
```csharp
                    T_UnitOfMeasures uom = T_UnitOfMeasures.Load(itemPacking.UomId);
                    if (uom != null)
                    {
                        cboUoM.Text = uom.UomName;
                        cboUoM.SelectedValue = uom.UomId;
                        txtPriceCnyPerUnit.Text = String.Format("{0}/{1}", cboCurrency.Text, cboUoM.Text);

                        cboUoM2.Text = uom.UomName;
                        cboUoM2.SelectedValue = uom.UomId;
                        txtCostCnyPerUnit.Text = String.Format("{0}/{1}", cboCurrency2.Text, cboUoM2.Text);
                    }
```

**Replace with**:
```csharp
                    if (itemPacking.UomId.HasValue)
                    {
                        T_UnitOfMeasures uom = T_UnitOfMeasures.Load(itemPacking.UomId.Value);
                        if (uom != null)
                        {
                            cboUoM.Text = uom.UomName;
                            cboUoM.SelectedValue = uom.UomId;
                            txtPriceCnyPerUnit.Text = String.Format("{0}/{1}", cboCurrency.Text, cboUoM.Text);

                            cboUoM2.Text = uom.UomName;
                            cboUoM2.SelectedValue = uom.UomId;
                            txtCostCnyPerUnit.Text = String.Format("{0}/{1}", cboCurrency2.Text, cboUoM2.Text);
                        }
                    }
```

---

## File 2: Utility.cs

**Location**: `xPort5\Controls\Utility.cs`

### Fix 1 (Line ~3865): Remove Dictionary assignment

**Find**:
```csharp
userPref.MetadataXml = new Dictionary<string, UserDisplayPreference.MetadataAttributes>();
```

**Replace with**:
```csharp
// userPref.MetadataXml = new Dictionary<string, UserDisplayPreference.MetadataAttributes>(); // Removed: MetadataXml is now a string in EF6
```

**Note**: This line appears multiple times. Comment out ALL occurrences.

---

## Files 3-13: Record Files with Nullable Guid Errors

For these files, the pattern is the same: `T_Something.Load(obj.PropertyId)` where PropertyId is nullable.

### General Pattern:

**Find pattern**:
```csharp
T_ClassName.Load(variable.PropertyId)
```

**Replace with**:
```csharp
(variable.PropertyId.HasValue ? T_ClassName.Load(variable.PropertyId.Value) : null)
```

### Specific Files and Properties:

#### PackageRecord.cs (Lines 237, 245)
- Look for: `T_UnitOfMeasures.Load(*.UomId)`
- Replace using pattern above

#### CustomerRecord.cs (Lines 198, 204)
- Look for: `T_Country.Load(*.CountryId)` or similar
- Replace using pattern above

#### SupplierRecord.cs (Lines 187, 193)
- Look for: `T_Country.Load(*.CountryId)` or similar
- Replace using pattern above

#### InvoiceRecord.cs (Lines 266, 272, 278, 284, 290, 339)
- Look for: `T_*.Load(*.SomeId)` patterns
- Replace using pattern above

#### PreOrderRecord.cs (Line 212)
- Look for: `T_*.Load(*.SomeId)`
- Replace using pattern above

#### PurchaseContractRecord.cs (Lines 234, 240, 246, 252, 258)
- Look for: `T_*.Load(*.SomeId)` patterns
- Replace using pattern above

#### QuotationRecord.cs (Line 309)
- Look for: `T_*.Load(*.SomeId)`
- Replace using pattern above

#### ItemRecord.cs (Lines 347, 363)
- Look for: `T_*.Load(*.SomeId)` patterns
- Replace using pattern above

#### EditItemRecord.cs (Lines 371, 390, 471)
- Look for: `T_*.Load(*.SomeId)` patterns
- Replace using pattern above

---

## File 14: ShippingInfo.cs

**Location**: `xPort5\Controls\ShippingInfo.cs`

### Fix (Lines 251, 317): Ambiguous Utility references

**Find**:
```csharp
Utility.
```

**Replace with**:
```csharp
xPort5.Controls.Utility.
```

**Note**: Only replace if NOT already qualified (i.e., doesn't already have `xPort5.Controls.` or `xPort5.Common.` before it)

---

## Files 15-16: DateTime Conversion Errors

### InvoiceRecord.cs (Lines 246, 303)

**Find patterns like**:
```csharp
DateTime someVar = record.ShipDate;
```

**Replace with**:
```csharp
DateTime someVar = record.ShipDate.GetValueOrDefault();
```

### PurchaseContractRecord.cs (Line 215)

Same pattern as above - add `.GetValueOrDefault()` to nullable DateTime properties.

---

## File 17: EditItemRecord.cs

**Location**: `xPort5\Controls\EditItemRecord.cs`

### Fix 1 (Lines 429, 446): ToString overload errors

**Find**:
```csharp
someGuidId.ToString("D")
```
or
```csharp
someGuidId.ToString("N")
```

**Replace with**:
```csharp
someGuidId.ToString()
```

### Fix 2: Ambiguous Utility

Same as ShippingInfo.cs - qualify `Utility.` with `xPort5.Controls.Utility.`

---

## Files 18-19: Nullable Int Errors

### ItemList.cs (Line 346)

**Find pattern**:
```csharp
int someVar = record.Quantity;
```

**Replace with**:
```csharp
int someVar = record.Quantity.GetValueOrDefault();
```

### ItemRecord.cs (Lines 367, 374)

Same pattern - add `.GetValueOrDefault()` to nullable int properties like `Quantity`, `Count`, etc.

---

## Quick Reference: Common Patterns

### Pattern 1: Nullable Guid
```csharp
// Before
T_Something.Load(obj.PropertyId)

// After
(obj.PropertyId.HasValue ? T_Something.Load(obj.PropertyId.Value) : null)
```

### Pattern 2: Nullable DateTime
```csharp
// Before
DateTime var = obj.DateProperty;

// After
DateTime var = obj.DateProperty.GetValueOrDefault();
```

### Pattern 3: Nullable Int
```csharp
// Before
int var = obj.IntProperty;

// After
int var = obj.IntProperty.GetValueOrDefault();
```

### Pattern 4: ToString with format
```csharp
// Before
guid.ToString("D")

// After
guid.ToString()
```

### Pattern 5: Ambiguous Utility
```csharp
// Before
Utility.Something()

// After
xPort5.Controls.Utility.Something()
```

---

## After Making All Changes

1. **Build the solution**: Press Ctrl+Shift+B
2. **Check for remaining errors**: View Error List (Ctrl+\\, E)
3. **If errors remain**: Note which ones and let me know
4. **If build succeeds**: Test the application!

---

## Tips

- Use **Ctrl+H** for Find and Replace in current file
- Use **Ctrl+Shift+H** for Find and Replace across entire solution
- Enable "Match case" and "Match whole word" for precise replacements
- Use "Find Next" (F3) to review each occurrence before replacing
- Save frequently (Ctrl+S)

---

## Estimated Time

- ~5-10 minutes per file
- Total: ~30-60 minutes for all fixes

Good luck! Let me know if you encounter any issues.
