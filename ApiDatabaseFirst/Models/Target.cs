using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public Byte DailyTraget { get; set; }
        public Byte MonthTarget { get; set; }
    }
}
