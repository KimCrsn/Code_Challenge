using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class Employee
    {
        public String EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        public ReportEmployee ReportEmployee { get; set; }
        public List<Employee> DirectReports { get; set; } = new List<Employee>(); // Empty list default instad of null to reduce type of check
    }
}
