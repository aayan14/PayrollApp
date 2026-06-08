
IF NOT EXISTS (SELECT 1 FROM Departments)
BEGIN
    INSERT INTO Departments (Name) VALUES
    ('Engineering'),
    ('Human Resources');
END

IF NOT EXISTS (SELECT 1 FROM Employees)
BEGIN
    INSERT INTO Employees (Name, BasicSalary, DepartmentId) VALUES
    ('Ravi Sharma',    30000.00, 1),  
    ('Priya Mehta',    25000.00, 1),  
    ('Amit Kulkarni',  28000.00, 1),  
    ('Sneha Patil',    22000.00, 2),  
    ('Rahul Desai',    26000.00, 2);  
END


IF NOT EXISTS (SELECT 1 FROM Attendance)
BEGIN
    INSERT INTO Attendance (EmployeeId, Month, Year, DaysPresent, TotalWorkingDays) VALUES
    (1, MONTH(GETDATE()), YEAR(GETDATE()), 24, 26),  
    (2, MONTH(GETDATE()), YEAR(GETDATE()), 26, 26),  
    (3, MONTH(GETDATE()), YEAR(GETDATE()), 20, 26),  
    (4, MONTH(GETDATE()), YEAR(GETDATE()), 22, 26),  
    (5, MONTH(GETDATE()), YEAR(GETDATE()), 0,  26);  
END