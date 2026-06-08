CREATE OR ALTER PROCEDURE usp_RunPayroll
    @Month INT,
    @Year  INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Block duplicate runs (bonus 409 behaviour)
    IF EXISTS (SELECT 1 FROM PayrollRuns WHERE Month = @Month AND Year = @Year)
    BEGIN
        RAISERROR('Payroll run already exists for this month and year.', 16, 1);
        RETURN;
    END

    -- Create the run record
    DECLARE @RunId INT;
    INSERT INTO PayrollRuns (Month, Year, RunDate, IsFinalized)
    VALUES (@Month, @Year, GETDATE(), 1);
    SET @RunId = SCOPE_IDENTITY();

    -- Calculate and insert payroll for each employee
    INSERT INTO PayrollDetails (
        RunId, EmployeeId, BasicSalary, TotalWorkingDays, DaysPresent,
        GrossPay, PFDeduction, ProfessionalTax, NetPay
    )
    SELECT
        @RunId,
        e.EmployeeId,
        e.BasicSalary,
        a.TotalWorkingDays,
        a.DaysPresent,
        -- Gross Pay = (BasicSalary / TotalWorkingDays) * DaysPresent
        CASE 
            WHEN a.TotalWorkingDays = 0 THEN 0
            ELSE ROUND((e.BasicSalary / a.TotalWorkingDays) * a.DaysPresent, 2)
        END AS GrossPay,
        -- PF = 12% of Basic Salary
        ROUND(e.BasicSalary * 0.12, 2) AS PFDeduction,
        -- Professional Tax = flat 200
        200.00 AS ProfessionalTax,
        -- Net Pay = Gross - PF - ProfTax
        CASE
            WHEN a.TotalWorkingDays = 0 THEN 0 - ROUND(e.BasicSalary * 0.12, 2) - 200.00
            ELSE ROUND((e.BasicSalary / a.TotalWorkingDays) * a.DaysPresent, 2)
                 - ROUND(e.BasicSalary * 0.12, 2) - 200.00
        END AS NetPay
    FROM Employees e
    INNER JOIN Attendance a 
        ON e.EmployeeId = a.EmployeeId
        AND a.Month = @Month
        AND a.Year = @Year;

    -- Return the results
    SELECT
        pd.DetailId,
        pd.RunId,
        e.EmployeeId,
        e.Name,
        pd.BasicSalary,
        pd.TotalWorkingDays,
        pd.DaysPresent,
        pd.GrossPay,
        pd.PFDeduction,
        pd.ProfessionalTax,
        pd.NetPay
    FROM PayrollDetails pd
    INNER JOIN Employees e ON pd.EmployeeId = e.EmployeeId
    WHERE pd.RunId = @RunId;
END