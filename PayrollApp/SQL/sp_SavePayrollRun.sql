CREATE OR ALTER PROCEDURE sp_SavePayrollRun
    @Month INT,
    @Year  INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM PayrollRuns WHERE Month = @Month AND Year = @Year)
    BEGIN
        RAISERROR('Payroll run already exists for this month and year.', 16, 1);
        RETURN;
    END

    INSERT INTO PayrollRuns (Month, Year, RunDate, IsFinalized)
    VALUES (@Month, @Year, GETDATE(), 1);

    SELECT SCOPE_IDENTITY() AS RunId;
END