namespace PayrollApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public decimal BasicSalary { get; set; }
        public int DepartmentId { get; set; }
    }
}