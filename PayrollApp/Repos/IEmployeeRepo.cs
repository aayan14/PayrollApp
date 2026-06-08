using PayrollApp.Models;

namespace PayrollApp.Repos
{
    public interface IEmployeeRepo
    {
        Task<IEnumerable<Employee>> GetAllAsync();
    }
}