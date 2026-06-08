# Employee Payroll Run Module

A full-stack payroll management system built with ASP.NET Core 8, Dapper, SQL Server, and vanilla HTML/JS — built as part of a Full Stack Developer technical assessment.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API (C#) |
| DB Access | Dapper + ADO.NET |
| Database | SQL Server LocalDB |
| Frontend | HTML + Vanilla JS |
| Testing | xUnit |

---

## Project Structure

```
PayrollApp/
├── Controllers/         
│   ├── EmployeeController.cs
│   └── PayrollController.cs
├── Services/            
│   ├── IPayrollService.cs
│   ├── PayrollService.cs
│   └── PayrollCalculator.cs
├── Repos/               
│   ├── IEmployeeRepo.cs
│   ├── EmployeeRepo.cs
│   ├── IPayrollRepo.cs
│   └── PayrollRepo.cs
├── Models/              
│   ├── Employee.cs
│   ├── PayrollDetail.cs
│   ├── PayrollRun.cs
│   └── PayrollRunRequest.cs
├── SQL/                 
│   ├── schema.sql
│   ├── dump.sql
│   ├── usp_RunPayroll.sql
│   ├── usp_GetAllEmployees.sql
│   ├── usp_GetPayrollByMonthYear.sql
│   └── usp_GetPayrollSlip.sql
├── wwwroot/
│   └── index.html
└── Program.cs
```

---

## How to Run Locally

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (ships with Visual Studio 2022)
- Visual Studio 2022

### Steps

1. Clone the repository:
```bash
git clone https://github.com/aayan14/PayrollApp.git
cd PayrollApp
```

2. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PayrollDB;Trusted_Connection=True;"
}
```

3. Open `PayrollApp.sln` in Visual Studio 2022

4. Press **F5** to run — the app automatically:
   - Creates the `PayrollDB` database and all tables
   - Seeds 5 employees across 2 departments
   - Seeds attendance records for the current month
   - Creates all stored procedures

5. Open `http://localhost:5091/index.html` for the frontend

6. Open `http://localhost:5091/swagger` to test the API

> No manual SQL steps required. Everything runs automatically on startup.

---

## Database Setup

**Connection string format:**
```
Server=(localdb)\\mssqllocaldb;Database=PayrollDB;Trusted_Connection=True;
```

**To run scripts manually (optional):**

Run the following files in order using SSMS or sqlcmd against your `PayrollDB` database:

1. `SQL/schema.sql` — creates all tables with relationships and constraints
2. `SQL/dump.sql` — inserts seed data (5 employees, 2 departments, attendance)
3. `SQL/usp_RunPayroll.sql` — payroll calculation and save procedure
4. `SQL/usp_GetAllEmployees.sql`
5. `SQL/usp_GetPayrollByMonthYear.sql`
6. `SQL/usp_GetPayrollSlip.sql`

---

## API Endpoints

| Method | Endpoint | Description | Response |
|---|---|---|---|
| GET | /api/employees | List all employees | 200 OK |
| POST | /api/payroll/run | Trigger payroll run `{ month, year }` | 201 Created |
| GET | /api/payroll/run/{month}/{year} | Get saved payroll for month/year | 200 / 404 |
| GET | /api/payroll/{runId}/slip/{employeeId} | Get individual employee payslip | 200 / 404 |

**Each payroll response includes:** EmployeeId, Name, BasicSalary, WorkingDays, DaysPresent, GrossPay, PFDeduction, ProfessionalTax, NetPay

---

## Payroll Calculation Rules

| Component | Rule |
|---|---|
| Gross Pay | (Basic Salary ÷ Total Working Days) × Days Present |
| PF Deduction | 12% of Basic Salary |
| Professional Tax | Flat ₹200 per month |
| Net Pay | Gross Pay − PF − Professional Tax |

**Example — Ravi Sharma (from brief):**

| Component | Calculation | Amount |
|---|---|---|
| Gross Pay | (30,000 ÷ 26) × 24 | ₹27,692.31 |
| PF | 12% of ₹30,000 | ₹3,600.00 |
| Professional Tax | Flat | ₹200.00 |
| Net Pay | 27,692.31 − 3,600 − 200 | ₹23,892.31 |

---

## Running Tests

```bash
dotnet test
```

Or via Visual Studio: **Test → Run All Tests**

6 unit tests covering:
- Exact calculation match from the brief (Ravi Sharma example)
- Full attendance — gross equals basic salary
- Zero days present — gross is zero
- Zero working days — divide by zero protection
- PF always 12% of basic salary

---

## Bonus Features Completed

- ✅ HTTP 409 Conflict if payroll run already exists for the selected month/year
- ✅ Unit tests for net pay calculation logic (xUnit)
- ✅ Printable payslip view per employee (browser print dialog)
- ❌ Pagination — not completed (see below)

---

## Assumptions

- Authorization and HTTPS redirect middleware omitted — not required by the brief
- PF deduction is always 12% of Basic Salary regardless of days present
- Professional Tax is flat ₹200 regardless of days present
- Attendance is seeded statically for the current month. In production this would be populated by a dedicated attendance tracking system
- If no attendance record exists for an employee in the selected month, they are excluded from the payroll run and the run is rolled back
- Calculation logic lives in the stored procedure as specified in the brief. In a production system I would move this to the C# service layer using the `PayrollCalculator` class for better testability and separation of concerns
- All stored procedures use `CREATE OR ALTER` so re-running the app never fails on existing objects
- `PayrollCalculator.cs` exists as a C# mirror of the SP logic, used purely for unit testing

---

## What I Would Add With More Time

**Pagination on GET /payroll**
The current endpoint returns all employees in one response. With more time I would add `pageNumber` and `pageSize` query parameters and return a paginated response with total count.

**Structured Logging with Serilog**
Replace the default ASP.NET logger with Serilog for structured, searchable logs. Would log every payroll run with month, year, employee count and execution time.

**Authentication and Authorization**
Add JWT-based auth with an HR Manager role. Only authenticated HR users should be able to trigger or view payroll runs.

**Input Validation**
Add FluentValidation to validate month (1–12), year (reasonable range), and prevent negative or zero values from reaching the database.

**Move Calculation to C# Service Layer**
Move payroll calculation out of the stored procedure and into `PayrollCalculator.cs` for better separation of concerns, easier unit testing, and simpler business rule changes.

**Export to Excel/PDF**
Allow HR to download the full payroll run as an Excel sheet or PDF report directly from the frontend.

**Docker Support**
Add a `Dockerfile` and `docker-compose.yml` so the app can be spun up without any local SQL Server installation.

**Soft Delete for Employees**
Add an `IsActive` flag to the Employees table so terminated employees are excluded from future payroll runs without losing historical data.
