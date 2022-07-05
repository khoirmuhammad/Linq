using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst.Models
{
    public class ActualSales
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime SaleDate { get; set; }
        public Byte Total { get; set; }
    }
}
