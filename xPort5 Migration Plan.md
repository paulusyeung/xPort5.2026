# xPort5 Migration Plan: xPort5 to Open Source (Python/JS/Postgres)

## Executive Summary

Rewriting **xPort5** from a legacy .NET 4.5/Visual WebGui application to a modern **Python (Backend)**, **JavaScript/TypeScript (Frontend)**, and **PostgreSQL (Database)** stack is not only doable but highly recommended. This stack is industry-standard, cost-effective, and ideal for Open Source projects due to its vast community support.

## 1. Technology Stack Recommendations

### Backend: **Python + FastAPI**

- **Why**: FastAPI is one of the fastest Python frameworks, modern (async/await), and automatically generates interactive API documentation (Swagger UI), which is excellent for Open Source contributors.
- **Alternative**: Django (if you want a heavy "batteries-included" framework), but FastAPI offers more flexibility for a "Modern UI" focus.

### Frontend: **React + TypeScript + Vite**

- **Why**: React has the largest ecosystem. TypeScript ensures type safety (critical for complex ERP data like Orders/Articles). Vite provides a blazing fast development experience.
- **UI Library**: **Tailwind CSS** + **Shadcn/UI**. This combination allows for the "Premium" and "Modern" look you requested without fighting against a heavy component library.

### Database: **PostgreSQL**

- **Why**: The gold standard for Open Source relational databases. Robust, handles complex queries well, and has great JSON support if needed.
- **ORM**: **SQLAlchemy** (v2.0) or **Prisma Client Python**. SQLAlchemy is the Python standard.

### Infrastructure & DevOps

- **Docker**: Containerize everything (Backend, Frontend, DB) for easy "one-command" setup for contributors.
- **GitHub Actions**: Automated testing and linting.

## 2. Migration Approach

### Phase 1: Database & Data Transfer (The Foundation)

#### *Priority: Data Integrity*

1. **Schema Mapping**: Map MS SQL data types to PostgreSQL (e.g., `UniqueIdentifier` -> `UUID`, `NVarChar` -> `VARCHAR`, `Bit` -> `BOOLEAN`).
   - **Naming Convention**: Convert all Table and Column names to **snake_case** (e.g., `ArticleName` -> `article_name`, `UserProfile` -> `user_profile`). This aligns with PostgreSQL and Python standards.
2. **ETL Pipeline**: Create a Python script (using `pandas` and `sqlalchemy`) to extract data from MS SQL and load it into Postgres.
   - *Note*: Since the current app uses Stored Procedures heavily (`spArticle_SelRec`, etc.), we will move this logic into the **Application Layer** (Python code) rather than rewriting them as Postgres functions. This makes the code database-agnostic and easier to maintain.

### Phase 2: Backend API Implementation

#### *Priority: Business Logic*

1. **Authentication**: Implement modern JWT (JSON Web Tokens) or OAuth2. Replace the insecure `xPort5.Public.Logon` logic.
2. **Core Modules**: Re-implement `xPort5.DAL` logic one module at a time:
   - `Articles` (Products)
   - `Partners` (Customers/Suppliers)
   - `Orders` (The complex logic seen in `OrderIN`, `OrderQT`, etc.)
3. **API Design**: RESTful endpoints (e.g., `GET /api/v1/articles/{id}`).

### Phase 3: Modern UI Development

#### *Priority: User Experience (UX)*

1. **Design System**: Establish the visual language (Colors, Typography, Spacing) using Tailwind.
2. **Component Library**: Build reusable components (Data Tables with sorting/filtering, Forms with validation).
3. **Page Migration**: Re-create the "Desktop-like" views from Visual WebGui as modern web pages.
   - *Improvement*: Instead of opening multiple windows (MDI), use a modern Tabbed Interface or Breadcrumb navigation.

## 3. Tools & Libraries

| Category       | Tool                         | Purpose                                     |
| -------------- | ---------------------------- | ------------------------------------------- |
| **Language**   | Python 3.11+                 | Backend logic                               |
| **Framework**  | FastAPI                      | API Server                                  |
| **Database**   | PostgreSQL 16                | Data Storage                                |
| **ORM**        | SQLAlchemy + Alembic         | Database interaction & Migrations           |
| **Frontend**   | React (TypeScript)           | User Interface                              |
| **Styling**    | Tailwind CSS                 | Styling                                     |
| **State Mgmt** | TanStack Query (React Query) | Server state management (caching, fetching) |
| **Migration**  | `pgloader` / Custom Python   | Moving data from MS SQL to Postgres         |

## 4. Open Source Strategy

To make this a successful Open Source project:

1. **License**: Choose MIT or Apache 2.0.
2. **Documentation**: A great `README.md` is essential. Explain how to spin up the dev environment (Docker Compose).
3. **Contribution Guide**: `CONTRIBUTING.md` explaining coding standards (PEP8 for Python, Prettier for JS).

## 5. Feasibility Assessment

- **Doable?**: **Yes**. The current app is a standard CRUD application with some complex business logic in the DAL.
- **Complexity**: Medium-High. The main challenge is the **Data Migration** (preserving relationships) and interpreting the legacy Stored Procedures.
- **Timeframe**:
  - MVP (Core Data + Basic UI): 4-6 weeks.
  - Full Feature Parity: 3-4 months.
