CREATE OR ALTER PROCEDURE usp_GetPayrollByMonthYear
    @Month INT,
    @Year  INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        pd.DetailId, pd.RunId, e.EmployeeId, e.Name,
        pd.BasicSalary, pd.TotalWorkingDays, pd.DaysPresent,
        pd.GrossPay, pd.PFDeduction, pd.ProfessionalTax, pd.NetPay
    FROM PayrollDetails pd
    INNER JOIN Employees e ON pd.EmployeeId = e.EmployeeId
    INNER JOIN PayrollRuns pr ON pd.RunId = pr.RunId
    WHERE pr.Month = @Month AND pr.Year = @Year;
END