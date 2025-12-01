# xPort5 Analysis Report

## Executive Summary

[Pending analysis]

## Project Structure

- **Solution**: 
  
  xPort5.sln

- **Projects**:
  
  - `xPort5`: Main Web Application (Presumed)
  - `xPort5.Bot`: Bot Component
  - `xPort5.DAL`: Data Access Layer
  - `xPort5.EF6`: Entity Framework 6 Implementation

## Technology Stack

- **Framework**: .NET Framework 4.5 (Target)
- **Database**: MS SQL Server
- **ORM**: Entity Framework 6 (Inferred from project name)

## Findings

### Configuration

- **Framework**: .NET Framework 4.5
- **Web Framework**: **Visual WebGui (VWG)** (Gizmox), mimicking desktop UI in browser.
- **UI Controls**: **DevExpress** v15.2.
- **Authentication**: Windows Authentication configured in IIS/ASP.NET, but custom `xPort5.Public.Logon` handler defined in VWG config.
- **Session**: In-Process session state.

### Data Access

- **Dual Approach**:
  
  - **Enterprise Library (DAAB)**: Used via `xPort5.DAL.SqlHelper` for raw SQL/Stored Procedure execution.
  
  - **Entity Framework 6**: Used in `xPort5.EF6` (Database First/Model First with .edmx).

- **Database**: MS SQL Server (Connection string `SysDb`).

- **Security**: `NetSqlAzMan` referenced for role-based security.

### Security

- **Hardcoded Credentials**: Connection strings in 
  
  Web.config contain cleartext usernames and passwords (`uid=sa;pwd=...`). **Critical Security Risk**.

- **Windows Auth**: Enabled in IIS.

- **Custom Logon**: `xPort5.Public.Logon` class handles application-level authentication.
  
  - **Vulnerability**: Uses string concatenation for the login query (`"LoginName = ..."`), which is a **SQL Injection** risk, despite the weak `Replace("'", "")` sanitization.
  - **Critical Flaw**: Passwords appear to be stored in **plaintext** (direct comparison in SQL).

- **Role-Based Security**: Uses `NetSqlAzMan`.

### Modernization Opportunities

- **Framework Migration**: The application relies heavily on **Visual WebGui**, which is a niche, legacy framework designed to bring WinForms development to the web. It is likely end-of-life or in maintenance mode.
  
  - **Recommendation**: Plan a rewrite to a modern web stack (e.g., ASP.NET Core with Blazor, React, or Angular). The "Desktop-like" experience can be recreated with modern SPA frameworks.

- **Data Access Unification**: The mix of 
  
  SqlHelper (Enterprise Library) and EF6 is inconsistent.
  
  - **Recommendation**: Standardize on EF Core in a rewrite.

- **Dependency Updates**:
  
  - **DevExpress v15.2**: Very old. Update to latest if maintaining the current app, or replace in a rewrite.

  - **Enterprise Library**: Deprecated.

- **Infrastructure**:
  
  - Move secrets out of Web.config.
  
  - Containerize the application (Docker) for easier deployment, though VWG might have specific IIS dependencies that make this tricky (Windows Containers would be needed).

## Detailed Recommendations

1. **Immediate Actions (Maintenance)**:

  - **Secure Credentials**: Encrypt `Web.config` connection strings or move to environment variables.

  - **Audit Permissions**: Verify `NetSqlAzMan` roles and ensure least privilege.

  - **Backup**: Ensure database and code are backed up before any changes.

2. **Long-term Strategy (Modernization)**:

   - **Phase 1: API Layer**: Expose the business logic (currently in `xPort5.DAL` and `xPort5.EF6`) as a REST/gRPC API using ASP.NET Core.

   - **Phase 2: Frontend Rewrite**: Build a new frontend (Blazor or React) that consumes the API, replacing the Visual WebGui forms one by one or module by module.

   - **Phase 3: Database Refactoring**: Once the frontend is decoupled, refactor the database and data access layer to use EF Core exclusively and remove `SqlHelper`.
