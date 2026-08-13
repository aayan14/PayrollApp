CREATE OR ALTER PROCEDURE sp_GetAttendanceForPayroll
    @Month INT,
    @Year  INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.EmployeeId, e.Name, e.BasicSalary,
        a.TotalWorkingDays, a.DaysPresent
    FROM Employees e
    INNER JOIN Attendance a 
        ON e.EmployeeId = a.EmployeeId
        AND a.Month = @Month AND a.Year = @Year;
END
