using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        int GetReportCount(string id);
        Compensation GetCompById(string id);
        int ReportCounter(List<Employee> employees, int reportCount);
        Employee Add(Employee employee);
        Compensation Add(Compensation compensation);
        Employee Remove(Employee employee);
        Task SaveAsync();
    }
}