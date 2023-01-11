
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Controllers;
using CodeChallenge.Data;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Kim's added tests below!
        [TestMethod]
        public void GetReportingStructureByEmployeeId_ProvidedExistingEmployeeId_ExpectedResults()
        {
            // Arrange
            var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";
            var expectedNumOfReports = 2; // Informing
            var expectedFirstName = "Ringo";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reporting/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reoprtingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedNumOfReports, reoprtingStructure.NumberOfReports);
            Assert.AreEqual(expectedFirstName, reoprtingStructure.Employee.FirstName);
        }

        [TestMethod]
        public void CreateCompensation_ProvidedExistingEmployeeId_ExpectedResults()
        {
            // Arrange
            var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";

            var compensation = new Compensation()
            {
                Salary = "$1,000,000.00",
                EffectiveDate = DateTime.UtcNow
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{employeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.Employee);
            Assert.IsNotNull(newCompensation.EffectiveDate);
            Assert.AreEqual(newCompensation.Salary, compensation.Salary);
            Assert.AreEqual(newCompensation.Employee.FirstName, "Ringo");
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_ProvidedAnExistingEmployeeId_ReturnsOk()
        {
            // Arrange
            /* Opening a new context to manually add a compensation to the database since there isn't one in json seed data.
            Also to keep seperated, the create controller functionality */
            var _employeeContext = new EmployeeContext(new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase("EmployeeDB").Options);

            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";
            EmployeeRespository _employeeRepository = new EmployeeRespository(_employeeContext);
            Employee employee = _employeeRepository.GetById(employeeId); // Retrieval of employee

            var compensation = new Compensation()
            {
                Salary = "$1,000,000.00",
                EffectiveDate = DateTime.UtcNow,
                Employee = employee
            };
            _employeeRepository.Add(compensation);
            _employeeContext.SaveChanges();

            var expectedSalary = "$1,000,000.00";
            var expectedFirstName = "Pete";
            var expectedEffectiveDate = DateTime.UtcNow.AddDays(3);

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensationResult = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedFirstName, compensationResult.Employee.FirstName);
            Assert.AreEqual(expectedSalary, compensationResult.Salary);
            Assert.IsTrue(expectedEffectiveDate > compensationResult.EffectiveDate);
        }
    }
}
