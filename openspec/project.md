# Project Context

## Purpose
xPort5 is a legacy .NET 4.5.2 web application built with Visual WebGui, currently undergoing a modernization process. The immediate goal is to migrate the data access layer from a custom DAL using Stored Procedures to Entity Framework 6 (EF6) to improve maintainability and testability. The long-term vision is to migrate the entire application to a modern stack (Python/FastAPI backend, React/TypeScript frontend, PostgreSQL database).

## Tech Stack
- **Framework**: .NET Framework 4.5.2
- **Web Framework**: Visual WebGui (VWG) 4.5.x
- **ORM**: Entity Framework 6 (migrating from custom DAL)
- **Database**: MS SQL Server
- **UI Components**: DevExpress 15.2, Gizmox WebGUI Controls
- **Authentication**: Windows Authentication / Custom Forms Authentication

## Project Conventions

### Code Style
- PascalCase for C# classes and methods.
- Standard .NET coding conventions.
- **Code Comments**: Use format `// yyyy-mm-dd AI-Model-Name: comment` for AI-generated comments (e.g., `// 2025-11-25 Gemini 2.0 Flash: This method provides backward compatibility`).

### Architecture Patterns
- **Active Record**: The legacy DAL and the new EF6 compatibility layer follow the Active Record pattern (static `Load`, `Save` methods on entities).
- **Compatibility Layer**: `xPort5.EF6` implements a compatibility layer to mimic the old DAL API while using EF6 under the hood.

### Testing Strategy
- Manual verification of forms and controls.
- Future: Automated unit tests for the new EF6 layer.

### Git Workflow
- Standard feature branch workflow.

## Domain Context
The application handles ERP-like functionality including Articles (Products), Partners (Customers/Suppliers), and Orders. It manages complex relationships between these entities and supports various business workflows.

## Important Constraints
- Must maintain backward compatibility with existing VWG forms during the migration.
- Legacy Stored Procedures may contain business logic that needs to be preserved or ported.
- `xPort5.Bot` already uses EF6, while `xPort5` (Main App) relies on the legacy DAL.

## External Dependencies
- Google GData APIs (Calendar, Documents)
- NetSqlAzMan (Authorization)
- Microsoft ReportViewer
