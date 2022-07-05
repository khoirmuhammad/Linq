using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst.ViewModels
{
    public class SellInMonthPercentage
    {
        public string Name { get; set; }
        public string EmployeeId { get; set; }
        public string Monthname { get; set; }
        public int Month { get; set; }
        public int TotalActual { get; set; }
        public int TotalTarget { get; set; }
        public double Percentage { get; set; }
    }
}
