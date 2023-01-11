using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public int Id { get; set; } // Auto incremented by in memory db
        public string Salary { get; set; }
        public DateTime EffectiveDate { get; set; } // Utc time ex: "2023-01-31T22:14:23.4551483Z"
        public Employee Employee { get; set; }
    }
}
