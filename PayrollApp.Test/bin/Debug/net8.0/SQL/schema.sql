-- SQL/schema.sql

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
CREATE TABLE Departments (
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employees' AND xtype='U')
CREATE TABLE Employees (
    EmployeeId   INT PRIMARY KEY IDENTITY(1,1),
    Name         NVARCHAR(100) NOT NULL,
    BasicSalary  DECIMAL(10,2) NOT NULL,
    DepartmentId INT NOT NULL,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Attendance' AND xtype='U')
CREATE TABLE Attendance (
    AttendanceId  INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId    INT NOT NULL,
    Month         INT NOT NULL,
    Year          INT NOT NULL,
    DaysPresent   INT NOT NULL,
    TotalWorkingDays INT NOT NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PayrollRuns' AND xtype='U')
CREATE TABLE PayrollRuns (
    RunId       INT PRIMARY KEY IDENTITY(1,1),
    Month       INT NOT NULL,
    Year        INT NOT NULL,
    RunDate     DATETIME NOT NULL DEFAULT GETDATE(),
    IsFinalized BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_PayrollRun_MonthYear UNIQUE (Month, Year)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PayrollDetails' AND xtype='U')
CREATE TABLE PayrollDetails (
    DetailId        INT PRIMARY KEY IDENTITY(1,1),
    RunId           INT NOT NULL,
    EmployeeId      INT NOT NULL,
    BasicSalary     DECIMAL(10,2) NOT NULL,
    TotalWorkingDays INT NOT NULL,
    DaysPresent     INT NOT NULL,
    GrossPay        DECIMAL(10,2) NOT NULL,
    PFDeduction     DECIMAL(10,2) NOT NULL,
    ProfessionalTax DECIMAL(10,2) NOT NULL,
    NetPay          DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (RunId) REFERENCES PayrollRuns(RunId),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);