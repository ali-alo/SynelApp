using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using SynelApp.Controllers;
using SynelApp.Models;
using SynelApp.Repositories;

namespace SynelApp.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_Returns_View()
        {
            // Arrange
            var mockRepo = new Mock<IEmployeeRepository>();
            var logger = new LoggerFactory().CreateLogger<HomeController>();
            var controller = new HomeController(logger, mockRepo.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Get_All_Employees()
        {
            // Arrange
            Mock<IEmployeeRepository> mockRepo = new Mock<IEmployeeRepository>();
            var expectedData = new List<Employee>
            {
                new Employee { Id = 1, PayrollNumber ="RF321", Forenames = "Roger Federer" },
                new Employee { Id = 2, PayrollNumber ="ND321", Forenames = "Novak Djokovic"}
            };

            // set up dependencies
            mockRepo.Setup(m => m.GetEmployees()).Returns(expectedData);
            ILogger<HomeController> logger = new LoggerFactory().CreateLogger<HomeController>();
            HomeController controller = new HomeController(logger, mockRepo.Object);

            // Act
            var result = controller.Employees();

            // Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Employee>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);

            // final check, count the number of records returned by the controller
            Assert.Equal(2, employees.Count());
        }

        [Fact]
        public async Task Can_Update_Employee_With_Valid_PatchDoc()
        {
            // Arrange
            Mock<IEmployeeRepository> mockRepo = new Mock<IEmployeeRepository>();
            var employeeId = 1;
            var validPatchDoc = new JsonPatchDocument<Employee>();
            var newValue = "Rafael Nadal";
            validPatchDoc.Replace(e => e.Forenames, newValue);

            var existingEmp = new Employee
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
            };

            // set up dependencies
            mockRepo.Setup(repo => repo.GetEmployee(employeeId)).ReturnsAsync(existingEmp);

            // test of UpdateEmployee function is available in EmployeeRepositoryTests.cs file
            mockRepo.Setup(repo => repo.UpdateEmployee(existingEmp)).ReturnsAsync(true);

            ILogger<HomeController> logger = new LoggerFactory().CreateLogger<HomeController>();
            HomeController controller = new HomeController(logger, mockRepo.Object);

            // Act
            var result = await controller.UpdateEmployee(employeeId, validPatchDoc);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Employee>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var updatedEmployee = Assert.IsType<Employee>(okResult.Value);

            // final check, check the record property was updated
            Assert.Equal(newValue, updatedEmployee.Forenames);
        }

        [Fact]
        public async Task Can_ProcessCSV_And_Show_TempData()
        {
            // Arrange
            var mockRepo = new Mock<IEmployeeRepository>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("test.csv");
            // mocking like the api processed 2 rows successfully, another test for this functionality is needed
            mockRepo.Setup(m => m.ProcessCsvData(file.Object)).ReturnsAsync(2);

            ILogger<HomeController> logger = new LoggerFactory().CreateLogger<HomeController>();
            var controller = new HomeController(logger, mockRepo.Object);
            controller.TempData = tempData;

            // Act
            var result = await controller.ProcessCSV(file.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // check processing result
            Assert.Equal(2, tempData["processed"]);
            Assert.Equal("test.csv was successfully uploaded.", tempData["fileInfo"]);
        }
    }
}
