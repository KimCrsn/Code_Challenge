using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using CodeChallenge.Data;
using CodeChallenge.Repositories;
using System.Globalization;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        private readonly EmployeeContext _employeeContext;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
            _employeeService = employeeService;
        }


        // ---------------------------------------- Posts ----------------------------------------------------- 
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpPost("{id}")]
        public IActionResult CreateCompensation([FromBody] Compensation compensation, string id)
        {
            _logger.LogDebug($"Recieved employee compensation create request for employee by Id'");

            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            if (_employeeService.GetCompById(id) != null)
            {
                return BadRequest("Employee already has compensation assigned");
            }

            compensation.Employee = employee;
            double value;
            if (Double.TryParse(compensation.Salary, out value)) { compensation.Salary = value.ToString("C"); }
            _employeeService.Create(compensation);

            // CreatedAtRouteResult object
            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Id }, compensation);
        }

    // ---------------------------------------- Gets ----------------------------------------------------- 

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet("reporting/{id}")]
        public IActionResult GetReportingStructureByEmployeeId(String id)
        {
            _logger.LogDebug($"Recieved number of employee reports get request for '{id}'");

            ReportingStructure report = _employeeService.GetReportingStructure(id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpGet("compensation/{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            _logger.LogDebug($"Received employee compensation get request for '{id}'");

            var compensation = _employeeService.GetCompById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        // ---------------------------------------- Puts ----------------------------------------------------- 

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody] Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
    }
}
