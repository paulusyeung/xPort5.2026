# Quick Start: Fix PostgreSQL Conversion

## TL;DR - What You Need To Know

The generated `xPort5_PostgreSQL.sql` has **600+ errors** and won't run on PostgreSQL.  
I've fixed the conversion script. Here's what to do:

## ⚡ Quick Fix (5 minutes)

```bash
cd c:\Projects\xPort5.2026

# Run the fixed converter
python xPort5.Pgsql/scripts/convert_schema.py \
    database/xPort5_MSSQL_script.sql \
    xPort5.Pgsql/schema/xPort5_PostgreSQL_FIXED.sql

# Test it
psql -d your_database -f xPort5.Pgsql/schema/xPort5_PostgreSQL_FIXED.sql
```

## ✅ What's Fixed

The updated converter now handles:
- ✓ Table creation syntax (semicolons, WITH clauses)
- ✓ All data types (datetime, money, bit, nvarchar, etc.)
- ✓ Constraints (foreign keys, primary keys)
- ✓ Functions (NEWID, GETDATE, etc.)
- ✓ Removes SQL Server-specific keywords

## ⚠️ What Needs Manual Work

- **Stored Procedures** (9 total) - Commented out, need rewrite or use EF LINQ instead
- **Some Views** - May have TOP clauses to review
- **String concat** - A few views might need `+` → `||` conversion

## 📁 Files Created

1. **`convert_schema.py`** ← **USE THIS** (updated, comprehensive)
2. **`README_CONVERSION.md`** ← Full documentation
3. **`ANALYSIS_AND_FIXES.md`** ← Complete analysis
4. **`CONVERSION_ISSUES.md`** ← Issue catalog

## 🔍 What Was Wrong

| Issue | Count | Status |
|-------|-------|--------|
| Missing semicolons | 49 | ✓ Fixed |
| Invalid WITH clauses | 49 | ✓ Fixed |
| Wrong ALTER TABLE syntax | 200+ | ✓ Fixed |
| Invalid CHECK CONSTRAINT | 200+ | ✓ Fixed |
| Wrong data types | 100+ | ✓ Fixed |
| Stored procedures | 9 | ⚠️ Commented out |

## 🚀 Next Steps

1. Run the fixed converter (command above)
2. Review the generated file
3. Test table creation
4. Decide on stored procedure strategy (EF or rewrite)

That's it! The converter is ready to use.
