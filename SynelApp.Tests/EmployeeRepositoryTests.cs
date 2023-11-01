using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SynelApp.Data;
using SynelApp.Models;
using SynelApp.Repositories;
using System.Text;

namespace SynelApp.Tests
{
    public class EmployeeRepositoryTests
    {

        [Fact]
        public async Task ValidateEmployee_Returns_True_On_Valid_Row()
        {
            // Arrange
            //create a fake db
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DB_{ValidateEmployee_Returns_False_On_Invalid_Rows}").Options; // making sure the db name is unique

            // Imitate some data
            var employees = new Employee[]
            {
                new Employee
                {
                    Id = 1,
                    PayrollNumber = "RF321",
                    Forenames = "Roger Federer",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
                new Employee
                {
                    Id = 2,
                    PayrollNumber = "ND321",
                    Forenames = "Novak Djokovic",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
            }.AsQueryable();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                // Add data to the db
                context.Employees.AddRange(employees);
                context.SaveChanges();

                // Add dependency injection
                var mockValidator = new Mock<IValidator<Employee>>();
                var employeeValidator = new EmployeeValidator();
                var mockLogger = new Mock<ILogger<EmployeeRepository>>();

                Employee validEmployee = new Employee // a valid employee
                {
                    PayrollNumber = "AM333",
                    Forenames = "Andy Murray",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "United Kingdom",
                    Address2 = "London",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                };

                mockValidator.Setup(validator => validator.ValidateAsync(validEmployee, CancellationToken.None))
                    .ReturnsAsync(employeeValidator.Validate(validEmployee));

                var repo = new EmployeeRepository(context, mockValidator.Object, mockLogger.Object);

                // Act
                var isValid = await repo.ValidateEmployee(validEmployee);

                // Assert
                Assert.True(isValid);
            }
        }

        // Similar method to the one above, just a negative test
        [Fact]
        public async Task ValidateEmployee_Returns_False_On_Invalid_Rows()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DB_{nameof(ValidateEmployee_Returns_False_On_Invalid_Rows)}").Options; // making sure the db name is unique

            var employees = new Employee[]
            {
                new Employee
                {
                    Id = 1,
                    PayrollNumber = "RF321",
                    Forenames = "Roger Federer",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
                new Employee
                {
                    Id = 2,
                    PayrollNumber = "ND321",
                    Forenames = "Novak Djokovic",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
            }.AsQueryable();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Employees.AddRange(employees);
                context.SaveChanges();

                var mockValidator = new Mock<IValidator<Employee>>();
                var employeeValidator = new EmployeeValidator();
                var mockLogger = new Mock<ILogger<EmployeeRepository>>();

                Employee invalidEmployee1 = new Employee // Invalid Employee  1 (Payroll number is repeated with employee ID 1)
                {
                    Id = 3,
                    PayrollNumber = "RF321",
                    Forenames = "Andy Murray",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "United Kingdom",
                    Address2 = "London",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                };

                Employee invalidEmployee2 = new Employee // Invalid Employee (Telephone must not be null)
                {
                    Id = 4,
                    PayrollNumber = "DS777",
                    Forenames = "Denis Istomin",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "",
                    Mobile = "33337777",
                    Address = "Uzbekistan",
                    Address2 = "Tashkent",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                };

                Employee invalidEmployee3 = new Employee // Invalid Employee (Employee is too old)
                {
                    Id = 5,
                    PayrollNumber = "AR891",
                    Forenames = "Andrey Rublev",
                    DateOfBirth = DateTime.Now.AddYears(-200),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Russian",
                    Address2 = "Moscow",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                };

                mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Employee>(), CancellationToken.None))
                    .ReturnsAsync((Employee employee, CancellationToken token) => employeeValidator.Validate(employee));

                var repo = new EmployeeRepository(context, mockValidator.Object, mockLogger.Object);

                // Act
                var isValidEmp1 = await repo.ValidateEmployee(invalidEmployee1);
                var isValidEmp2 = await repo.ValidateEmployee(invalidEmployee2);
                var isValidEmp3 = await repo.ValidateEmployee(invalidEmployee3);

                // Assert
                Assert.False(isValidEmp1);  // none of them shall pass the validation
                Assert.False(isValidEmp2);
                Assert.False(isValidEmp3);
            }
        }

        [Fact]
        public async Task Can_Process_Correct_Input_CSV()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DB_${Can_Process_Correct_Input_CSV}").Options; // making sure the db name is unique if all tests are run together

            using (var context = new ApplicationDbContext(contextOptions))
            {

                // Create csv file with valid content (content was taken from the description csv file, so 2 employees should be added)
                var csvContent = Encoding.UTF8.GetBytes(
@"Personnel_Records.Payroll_Number,Personnel_Records.Forenames,Personnel_Records.Surname,Personnel_Records.Date_of_Birth,Personnel_Records.Telephone,Personnel_Records.Mobile,Personnel_Records.Address,Personnel_Records.Address_2,Personnel_Records.Postcode,Personnel_Records.EMail_Home,Personnel_Records.Start_Date
COOP08,John ,William,26/01/1955,12345678,987654231,12 Foreman road,London,GU12 6JW,nomadic20@hotmail.co.uk,18/04/2013
JACK13,Jerry,Jackson,11/5/1974,2050508,6987457,115 Spinney Road,Luton,LU33DF,gerry.jackson@bt.com,18/04/2013
                    ");
                var memoryStream = new MemoryStream(csvContent);
                var file = new Mock<IFormFile>();
                file.Setup(f => f.FileName).Returns("test.csv");
                file.Setup(f => f.OpenReadStream()).Returns(memoryStream);
                file.Setup(f => f.Length).Returns(memoryStream.Length);

                var mockLogger = new Mock<ILogger<EmployeeRepository>>();
                var mockValidator = new Mock<IValidator<Employee>>();
                var employeeValidator = new EmployeeValidator();
                mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Employee>(), CancellationToken.None))
                    .ReturnsAsync((Employee employee, CancellationToken token) => employeeValidator.Validate(employee));

                var repo = new EmployeeRepository(context, mockValidator.Object, mockLogger.Object);

                // Act
                await repo.ProcessCsvData(file.Object);

                // Assert
                Assert.Equal(2, context.Employees.Count());  // two rows were processed
            }
        }

        [Fact]
        public async Task Can_Update_Employee()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"DB_{nameof(Can_Update_Employee)}").Options;  // making sure the db name is unique

            var employees = new Employee[]
           {
                new Employee
                {
                    Id = 1,
                    PayrollNumber = "RF321",
                    Forenames = "Roger Federer",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
                new Employee
                {
                    Id = 2,
                    PayrollNumber = "ND321",
                    Forenames = "Novak Djokovic",
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "Europe",
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                },
           }.AsQueryable();

            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Employees.AddRange(employees);
                context.SaveChanges();

                // By the application logic the update is called after Patch request, so the entity is not tracked
                context.ChangeTracker.Clear();

                // Create an Employee entity with the same primary key present in the db
                var employeeUpdated = new Employee
                {
                    Id = 1,
                    PayrollNumber = "RF321",
                    Forenames = "Roger Federer New Name",  // update forenames property
                    DateOfBirth = DateTime.Now.AddYears(-50),
                    Telephone = "777333",
                    Mobile = "33337777",
                    Address = "In Asia now",  // update address property
                    Address2 = "Spain",
                    Postcode = "12345",
                    EmailHome = "tennis@gmail.com",
                    StartDate = DateTime.Now.AddYears(-3)
                };

                var mockValidator = new Mock<IValidator<Employee>>();
                var employeeValidator = new EmployeeValidator();
                var mockLogger = new Mock<ILogger<EmployeeRepository>>();

                mockValidator.Setup(validator => validator.ValidateAsync(employeeUpdated, CancellationToken.None))
                    .ReturnsAsync(employeeValidator.Validate(employeeUpdated));

                var repo = new EmployeeRepository(context, mockValidator.Object, mockLogger.Object);

                // Act
                var result = await repo.UpdateEmployee(employeeUpdated);
                var updatedEmployee = context.Employees.Find(1);

                // Assert
                Assert.True(result);
                Assert.Equal("Roger Federer New Name", updatedEmployee?.Forenames);
                Assert.Equal("In Asia now", updatedEmployee?.Address);
            }
        }
    }
}
