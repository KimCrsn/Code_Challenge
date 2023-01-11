using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class ReportEmployee
    {
        public int Id { get; set; }
        public string ReportEmployeeId { get; set; } // Id of employee reporting to
    }
}
