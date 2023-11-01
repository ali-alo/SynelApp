using CsvHelper.Configuration;
using CsvHelper;
using SynelApp.Data;
using SynelApp.Models;
using System.Globalization;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace SynelApp.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<Employee> _validator;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(ApplicationDbContext context, IValidator<Employee> validator,
            ILogger<EmployeeRepository> logger)
        {
            _context = context;
            _validator = validator;
            _logger = logger;
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return _context.Employees.OrderBy(e => e.Forenames).AsNoTracking();
        }

        public async Task<int> ProcessCsvData(IFormFile file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            var employees = new List<Employee>();

            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    // specify mapping from csv row to the .net Employee type
                    csv.Context.RegisterClassMap<EmployeeMap>();
                    while (csv.Read())
                    {
                        try
                        {
                            Employee employee = csv.GetRecord<Employee>() ?? new Employee();
                            bool isValid = await ValidateEmployee(employee);
                            // if the employee obj passed the validation and the payrollNum is unique among staged employees 
                            if (isValid && !employees.Any(e => e.PayrollNumber == employee.PayrollNumber))
                                employees.Add(employee);
                        }
                        catch (CsvHelperException ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }
                }
            }
            // add rows that were processes and passed the validation to the db
            await _context.Employees.AddRangeAsync(employees);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateEmployee(Employee employee)
        {
            ValidationResult result = await _validator.ValidateAsync(employee);
            // check that no other employee has the same payroll number after applying changes
            return result.IsValid && !_context.Employees.Any(e => e.PayrollNumber == employee.PayrollNumber && e.Id != employee.Id);
        }

        public async Task<bool> UpdateEmployee(Employee employee)
        {
            if (!await ValidateEmployee(employee))
                return false;
            
            // the employee record was not tracked before, need to call Update method explicitly
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetEmployee(int id)
        {
            return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
