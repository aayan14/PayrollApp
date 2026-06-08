CREATE OR ALTER PROCEDURE usp_GetPayrollSlip
    @RunId      INT,
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        pd.DetailId, pd.RunId, e.EmployeeId, e.Name,
        pd.BasicSalary, pd.TotalWorkingDays, pd.DaysPresent,
        pd.GrossPay, pd.PFDeduction, pd.ProfessionalTax, pd.NetPay
    FROM PayrollDetails pd
    INNER JOIN Employees e ON pd.EmployeeId = e.EmployeeId
    WHERE pd.RunId = @RunId AND pd.EmployeeId = @EmployeeId;
END