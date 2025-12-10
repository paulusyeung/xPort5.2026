# PostgreSQL Conversion Issues and Fixes

## Overview
The `xPort5_PostgreSQL.sql` file was generated from `database/xPort5_MSSQL_script.sql` but contains numerous SQL Server-specific syntax errors that prevent it from running on PostgreSQL.

## Critical Issues Found

### 1. **USE Statement** (Line 7)
- **Issue**: `USE [xPort5]` - PostgreSQL doesn't support USE
- **Fix**: Remove or comment out - PostgreSQL uses `\c database_name` in psql CLI

### 2. **CREATE TABLE Missing Semicolons** (~49 occurrences)
- **Issue**: All CREATE TABLE statements missing closing semicolons
- **Example**: Line 32 ends with `)` instead of `);`
- **Fix**: Add `;` after the closing `)` of each CREATE TABLE

### 3. **Invalid WITH Clause** (~49 occurrences)
- **Issue**: SQL Server index options not supported in PostgreSQL
```sql
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, ...
```
- **Fix**: Remove entire WITH clause

### 4. **CLUSTERED/NONCLUSTERED Keywords** (4 occurrences: lines 1400, 1906, 2012, 2089)
- **Issue**: `PRIMARY KEY CLUSTERED` not valid in PostgreSQL
- **Fix**: Remove CLUSTERED/NONCLUSTERED keywords

### 5. **Data Type Issues**

#### NVARCHAR (29+ occurrences)
- **Issue**: `NVARCHAR` and `NCHAR` don't exist in PostgreSQL
- **Fix**: Change to `VARCHAR` or `TEXT`
- **Locations**: Lines 1755, 2783, 2788+, stored procedures

#### DATETIME (multiple occurrences)
- **Issue**: SQL Server's `DATETIME` type
- **Fix**: Change to `TIMESTAMP`
- **Locations**: Stored procedures around lines 2790+

#### MONEY (multiple occurrences)
- **Issue**: SQL Server's `MONEY` type
- **Fix**: Change to `NUMERIC(19,4)` or `DECIMAL(19,4)`
- **Locations**: Stored procedures around lines 2794+

### 6. **TOP Clause** (16 occurrences)
- **Issue**: `SELECT TOP (100) PERCENT` and `SELECT TOP (100)`
- **Fix**: 
  - Remove `TOP X PERCENT` (meaningless)
  - Replace `TOP X` with `LIMIT X` at end of query
- **Locations**: Lines 415, 470, 487, 543, 622, 639, 760, etc.

### 7. **CONVERT Function** (50+ occurrences)
- **Issue**: `CONVERT(VARCHAR(7),OrderIN.INDate,120)` - style parameter doesn't exist in PostgreSQL
- **Fix**: Use `TO_CHAR(column, format)` or `CAST(column AS type)`
- **Locations**: Lines 2872-2959 and throughout stored procedures

### 8. **String Concatenation** (line 1755+)
- **Issue**: SQL Server uses `+` for string concatenation
- **Example**: `'text' + variable + 'text'`
- **Fix**: Use `||` operator: `'text' || variable || 'text'`

### 9. **ALTER TABLE Syntax** (200+ occurrences starting line 2484)
- **Issue**: `ALTER TABLE ... WITH CHECK ADD CONSTRAINT`
- **Fix**: Remove `WITH CHECK ADD`, use just `ADD CONSTRAINT`
- **Issue**: `ALTER TABLE ... CHECK CONSTRAINT` statements
- **Fix**: Remove these entirely (PostgreSQL doesn't need them)

### 10. **Temp Tables** (stored procedures)
- **Issue**: `CREATE TABLE #temp_table` syntax
- **Fix**: Change to `CREATE TEMP TABLE temp_table` or `CREATE TEMPORARY TABLE`
- **Locations**: Lines 2786, 2838, etc.

### 11. **IDENTITY Columns** (line 2840)
- **Issue**: `TermId INT IDENTITY(1,1)`
- **Fix**: Change to `TermId SERIAL` or `TermId INTEGER GENERATED ALWAYS AS IDENTITY`

### 12. **EXEC for Dynamic SQL** (20 occurrences)
- **Issue**: `EXEC(@SQL)` syntax
- **Fix**: Change to `EXECUTE` (without parentheses for string variables)
- **Locations**: Lines 2858, 3071, 3302, 3550, etc.

### 13. **Extended Properties** (lines 23015-23034)
- **Issue**: `EXEC sys.sp_addextendedproperty`
- **Fix**: Remove or convert to `COMMENT ON` statements

### 14. **Stored Procedures**
- **Issue**: `CREATE PROCEDURE` syntax differs significantly
- **Fix**: 
  - Change to `CREATE OR REPLACE FUNCTION`
  - Add `RETURNS void` or appropriate return type
  - Change `AS BEGIN` to `AS $$  BEGIN`
  - Change `END` to `END; $$ LANGUAGE plpgsql;`
  - Review all variables and logic

## Conversion Script Issues

The `convert_schema.py` script has the following problems:

1. **WITH clause regex doesn't match** - only removes WITH when followed by `ON [PRIMARY]`
2. **No semicolon addition** - doesn't add `;` to CREATE TABLE statements  
3. **Incomplete CLUSTERED handling** - doesn't remove keyword properly
4. **No ALTER TABLE conversion** - doesn't handle `WITH CHECK ADD` properly
5. **No CHECK CONSTRAINT removal** - leaves invalid statements
6. **Stored procedure conversion** - too complex, needs complete rewrite
7. **No extended property handling** - doesn't skip these statements

## Recommended Approach

1. **Fix the converter script** to handle all the above issues
2. **Re-generate the SQL file**
3. **Manual review** of stored procedures - they may need complete rewrite
4. **Test incrementally** - create tables first, then constraints, then procedures

## Priority Fixes (Must Fix to Run)

1. Remove `USE` statement
2. Add semicolons to all CREATE TABLE statements
3. Remove all `WITH (...)` clauses
4. Remove `CLUSTERED`/`NONCLUSTERED` keywords
5. Convert all `WITH CHECK ADD` to just `ADD`
6. Remove all `CHECK CONSTRAINT` statements
7. Change `NVARCHAR` to `VARCHAR`
8. Comment out or rewrite all stored procedures

After these fixes, the basic schema (tables and constraints) should load into PostgreSQL.
