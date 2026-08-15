# Employee Payroll Management System

A full-stack payroll management system with role-based access control, JWT authentication, and automated CI/CD deployment to Azure.

![Deploy Status](https://github.com/aayan14/PayrollApp/actions/workflows/deploy.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Azure](https://img.shields.io/badge/Deployed%20on-Azure-0078D4)

**Live Demo:** [https://payroll-app-dugncvcygtc2eyh8.indiasouthcentral-01.azurewebsites.net/login.html](https://payroll-app-dugncvcygtc2eyh8.indiasouthcentral-01.azurewebsites.net/login.html)

**Demo credentials:**
| Role | Username | Password |
|---|---|---|
| Super Admin (Head HR) | `superadmin` | `SuperAdmin@123` |
| Associate HR | *(create one via the Users page after logging in as Super Admin)* | |

---

## Overview

Replaces manual Excel-based payroll processing with a proper full-stack system. HR teams can calculate monthly salaries, track attendance, generate payslips, and manage user access — all with role-based permissions enforced end to end, from the UI down to the API.

## Key Features

- **JWT authentication** with role-based authorization (Super Admin vs Associate HR)
- **Automated payroll calculation** — gross pay, PF deduction, professional tax, net pay
- **Immutable payroll runs** — once finalized, records cannot be edited or deleted
- **Printable payslips** per employee
- **Centralized exception handling** — consistent JSON error responses across the API
- **Input validation** with FluentValidation (business rules like "no future-dated payroll runs")
- **Structured logging** with Serilog (console + rolling file logs)
- **User management** — Super Admins can create and manage HR accounts, passwords hashed with BCrypt
- **CI/CD pipeline** — every push to `main` automatically builds, tests, and deploys to Azure

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API (C#) |
| Database Access | Dapper + ADO.NET |
| Database | Azure SQL Database |
| Frontend | HTML, CSS, Vanilla JS |
| Authentication | JWT Bearer tokens, BCrypt password hashing |
| Validation | FluentValidation |
| Logging | Serilog |
| Testing | xUnit |
| Hosting | Azure App Service |
| CI/CD | GitHub Actions |

## Architecture

Frontend (HTML/JS)
↓
Controllers (thin, no business logic)
↓
Services (business logic, calculation, validation orchestration)
↓
Repositories (Dapper — all DB access via stored procedures)
↓
Azure SQL Database

Cross-cutting concerns handled via middleware:
- **ExceptionHandlingMiddleware** — catches all unhandled exceptions, returns consistent JSON errors
- **FluentValidation auto-validation** — rejects invalid requests before they reach controllers
- **JWT Authentication middleware** — validates tokens on every protected request

## Role Hierarchy

| Action | Super Admin | Associate HR |
|---|---|---|
| Login | ✅ | ✅ |
| View payroll runs | ✅ | ✅ |
| View/print payslips | ✅ | ✅ |
| Trigger payroll run | ✅ | ❌ |
| Create/manage HR users | ✅ | ❌ |

## API Endpoints

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| POST | /api/auth/login | No | Authenticate and receive JWT |
| POST | /api/users | Super Admin | Create a new HR user |
| GET | /api/users | Super Admin | List all HR users |
| GET | /api/employees | Any logged-in user | List all employees |
| POST | /api/payroll/run | Super Admin | Trigger a payroll run |
| GET | /api/payroll/run/{month}/{year} | Any logged-in user | Get saved payroll for a month |
| GET | /api/payroll/{runId}/slip/{employeeId} | Any logged-in user | Get individual payslip |

## CI/CD Pipeline

Every push to `main` triggers a GitHub Actions workflow that:
1. Restores dependencies
2. Builds the solution
3. Runs the full xUnit test suite — **deployment is blocked if any test fails**
4. Publishes the build
5. Deploys automatically to Azure App Service

See [`.github/workflows/deploy.yml`](.github/workflows/deploy.yml).

## Running Locally

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (or any SQL Server instance)
- Visual Studio 2022

### Steps

1. Clone the repository:
```bash
git clone https://github.com/aayan14/PayrollApp.git
cd PayrollApp
```

2. Update `appsettings.json` with your local connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PayrollDB;Trusted_Connection=True;"
}
```

3. Open `PayrollApp.sln` in Visual Studio, press **F5**

The app automatically creates the database schema, seeds sample data, and creates a default Super Admin account on first run.

4. Open `http://localhost:{port}/login.html`

## Running Tests

```bash
dotnet test
```

6 unit tests cover the payroll calculation logic (`PayrollCalculator.cs`), including edge cases like zero attendance and zero working days.

## Design Decisions & Trade-offs

- **Calculation logic lives in C#, not SQL** — moved from stored procedures to `PayrollCalculator.cs` for testability; stored procedures now handle only data access
- **BCrypt over plain hashing** — deliberately slow, salted hashing resistant to brute-force attacks
- **Payroll runs are immutable** — enforced at the database level with a unique constraint on (Month, Year), not just application logic
- **JWT secret stored in Azure App Service Configuration**, never committed to source control
- **Azure SQL Serverless tier** — auto-pause disabled to avoid cold-start delays affecting live demos

## What I'd Add With More Time

- Pagination on `GET /api/payroll`
- API versioning
- Docker support for one-command local setup
- Refresh token rotation instead of fixed 60-minute JWT expiry
- Audit trail — track which user triggered each payroll run
