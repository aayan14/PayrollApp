namespace PayrollApp.Models
{
    public class PayrollAttendanceDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public decimal BasicSalary { get; set; }
        public int TotalWorkingDays { get; set; }
        public int DaysPresent { get; set; }
    }
}
