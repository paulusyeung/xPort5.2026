# Bug Fixes - Version 2

## Issues Found in Generated File (v2)

After reviewing `xPort5_PostgreSQL_v2.sql`, found these additional bugs:

### Bug 1: USE Statement Not Removed (Line 8)
**Problem**: `USE "xPort3_Newish"` still present
**Cause**: UTF-8 BOM character at start of line prevented detection
**Fix**: Strip BOM character before checking line content

### Bug 2: CREATE DEFAULT Not Fully Commented (Lines 11-20)
**Problem**: After commenting CREATE DEFAULT line, the AS and function lines still output
**Example**:
```sql
-- CREATE DEFAULT ...
AS
gen_random_uuid()
```
**Fix**: Skip all lines until GO after CREATE DEFAULT

### Bug 3: ASC Keywords Not Removed (Lines 31, 45, etc.)
**Problem**: `"RegionId" ASC` - ASC keyword still present in PRIMARY KEY constraints
**Cause**: ASC removal only applied within specific conditions
**Fix**: Remove ASC/DESC keywords everywhere, not just in certain contexts

### Bug 4: Double Closing Parentheses (Lines 33, 47, etc.)
**Problem**: `);\n)` - Extra closing paren after semicolon
**Cause**: WITH clause removal logic left the final `)` from `ON [PRIMARY])` 
**Fix**: Better detection of CREATE TABLE end using parenthesis depth tracking

## Fixes Applied to converter_schema.py

### 1. BOM Handling
```python
# Remove BOM if present
if line.startswith('\ufeff'):
    line = line[1:]
```

### 2. Skip CREATE DEFAULT Blocks
```python
# Handle CREATE DEFAULT - skip the whole block until GO
if 'CREATE DEFAULT' in line_upper:
    self.skip_until_go = True
    return '-- ' + line.rstrip() + '...'
```

### 3. Remove ASC/DESC Everywhere
```python
# Remove ASC/DESC keywords everywhere (not just in constraints)
converted = re.sub(r'\bASC\b', '', converted, flags=re.IGNORECASE)
converted = re.sub(r'\bDESC\b', '', converted, flags=re.IGNORECASE)
```

### 4. Better Parenthesis Tracking
```python
# Track parentheses depth to detect true end of CREATE TABLE
if self.in_create_table:
    self.paren_depth += converted.count('(') - converted.count(')')
    
# Only close when we're truly at the end
if stripped == ')' and self.paren_depth <= 0:
    converted = ');'
    self.in_create_table = False
```

## How to Regenerate

```bash
cd c:\Projects\xPort5.2026

# Delete the old v2 file
rm xPort5.Pgsql/schema/xPort5_PostgreSQL_v2.sql

# Run the fixed converter
python xPort5.Pgsql/scripts/convert_schema.py \
    database/xPort5_MSSQL_script.sql \
    xPort5.Pgsql/schema/xPort5_PostgreSQL_v3.sql

# Verify the fix
head -n 50 xPort5.Pgsql/schema/xPort5_PostgreSQL_v3.sql
```

## Expected Result

The new file should have:
- ✓ No USE statement
- ✓ CREATE DEFAULT blocks fully skipped/commented
- ✓ No ASC keywords
- ✓ No double closing parens
- ✓ Proper semicolons on CREATE TABLE
- ✓ All WITH clauses removed

## Testing

Check these specific lines in the new file:
```bash
# Should NOT have USE statement
grep -n "^USE " xPort5_PostgreSQL_v3.sql

# Should NOT have standalone AS lines after CREATE DEFAULT
grep -n "^AS$" xPort5_PostgreSQL_v3.sql

# Should NOT have ASC keywords
grep -n " ASC" xPort5_PostgreSQL_v3.sql

# Should NOT have );\n) patterns
grep -A1 ");" xPort5_PostgreSQL_v3.sql | grep "^)$"
```

All should return no results or be empty.
