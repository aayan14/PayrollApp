CREATE OR ALTER PROCEDURE usp_GetAllEmployees
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeId, Name, BasicSalary, DepartmentId 
    FROM Employees;
END
