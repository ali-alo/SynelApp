using SynelApp.Models;

namespace SynelApp.Repositories
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetEmployees();
        Task<int> ProcessCsvData(IFormFile file);
        Task<bool> ValidateEmployee(Employee employee);
        Task<bool> UpdateEmployee(Employee employee);
        Task<Employee?> GetEmployee(int id);
    }
}
