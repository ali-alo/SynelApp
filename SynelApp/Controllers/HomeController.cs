using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SynelApp.Models;
using SynelApp.Repositories;
using System.Diagnostics;

namespace SynelApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _repo;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Employee>> Employees()
        {
            return Ok(_repo.GetEmployees());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCSV(IFormFile file)
        {
            int processedCount = await _repo.ProcessCsvData(file);
            TempData["processed"] = processedCount;
            TempData["fileInfo"] = file == null ? "No file was uploaded." : $"{file.FileName} was successfully uploaded.";
            return RedirectToAction("Index");  // redirect to index page instead of returning View("Index") to prevent submitting form several times
        }

        [HttpPatch]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] JsonPatchDocument<Employee> patchDoc)
        {
            if (id == 0)
            {
                _logger.LogInformation("Incorrect id value in employee patch request");
                return BadRequest("Invalid employee Id for the edit");
            }

            var employee = await _repo.GetEmployee(id);

            if (employee == null)
            {
                _logger.LogInformation($"Passed id: {id}, was not found");
                return NotFound();
            }

            // if we are here, the app located an employee with the specified id
            patchDoc.ApplyTo(employee);

            // try to update the employee in the db now
            bool result = await _repo.UpdateEmployee(employee);
            if (result) 
            {
                _logger.LogInformation($"{employee.Id} was updated");
                return Ok(employee);
            }
            // new value is invalid
            // return a clean copy of the employee to revert changes in the grid table
            return BadRequest(await _repo.GetEmployee(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}