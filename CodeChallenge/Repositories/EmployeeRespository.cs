using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    // CRUD and save operations
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        // Testing constructor
        public EmployeeRespository(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }


        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }


        public Employee GetById(string id)
        {
            return _employeeContext.Employees
           .Include(p => p.DirectReports).ThenInclude(x => x.DirectReports)
           .FirstOrDefault(x => x.EmployeeId == id);
        }


        public Compensation GetCompById(string id)
        {
            return _employeeContext.Compensation
           .Include(p => p.Employee).ThenInclude(x => x.DirectReports)
           .FirstOrDefault(x => x.Employee.EmployeeId == id);
        }


        public int GetReportCount(string id)
        {
            Employee employee = _employeeContext.Employees.FirstOrDefault(x => x.EmployeeId == id);
            if (employee == null || employee.DirectReports == null) return 0;
            int numOfReports = ReportCounter(new List<Employee>() { employee }, 0);

            return numOfReports;
        }

        /// <summary>
        /// Uses recursion to loop through and count each employee in the heirarchy of 
        /// child direct report employees until there are no more children to check.
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="reportCount"></param>
        /// <returns></returns>
        public int ReportCounter(List<Employee> employees, int reportCount)
        {
            int reports = reportCount;
            for (int i = 0; i < employees.Count(); i++)
            {
                if (employees[i].DirectReports.Count() > 0 )
                {
                    int count = employees[i].DirectReports.Count();

                    if (count > 0)
                    reports = ReportCounter(employees[i].DirectReports, reportCount += count);
                }
            }

            return reports;
        }


        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }


        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
