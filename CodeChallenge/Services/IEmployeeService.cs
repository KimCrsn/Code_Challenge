using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        ReportingStructure GetReportingStructure(string id);
        Compensation GetCompById(string id);
        Employee Create(Employee employee);
        Compensation Create(Compensation compensation);
        Employee Replace(Employee originalEmployee, Employee newEmployee);
    }
}
