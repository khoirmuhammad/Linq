using ApiDatabaseFirst.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<SellController> _logger;
        public SellController(AppDBContext context, ILogger<SellController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetSellInMonthPercentageBySP")]
        public async Task<IActionResult> GetSellInMonthPercentageBySP()
        {
            try
            {
                var response = await _context.SellsInMonthPercentage.FromSqlRaw("exec spGetTotalSaleByMonthWithPercentage")
                            .ToListAsync();             

                return StatusCode(200, response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("GetSpvSellInMonthPercentageBySP")]
        public async Task<IActionResult> GetSpvSellInMonthPercentageBySP()
        {
            try
            {
                var response = await _context.SellsInMonthPercentage.FromSqlRaw("exec spGetTotalSupervisorSaleByMonthWithPercentage")
                            .ToListAsync();

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }

        }



        [HttpGet]
        [Route("GetSellInMonthPercentageByLinq")] //spGetTotalSaleByMonthWithPercentage
        public async Task<IActionResult> GetSellInMonthPercentageByLinq()
        {
            try
            {
                List<SellInMonthPercentage> response = new List<SellInMonthPercentage>();

                string[] months = new string[3] { "January", "February", "March" };

                var queries = await (from acs in _context.ActualSales
                              join emp in _context.Employees on acs.EmployeeId equals emp.Id
                              join tgt in _context.Targets on emp.Id equals tgt.EmployeeId
                              group new
                              {
                                  emp,
                                  acs,
                                  tgt
                              } by new
                              {
                                  emp.Name,
                                  acs.EmployeeId,
                                  acs.SaleDate.Month,
                                  tgt.MonthTarget
                              } into grp
                              select new
                              {
                                  grp.Key.Name,
                                  grp.Key.EmployeeId,
                                  grp.Key.Month,
                                  grp.Key.MonthTarget,
                                  TotalActual = grp.Sum(x => x.acs.Total),
                                  Percentage = Math.Round(Convert.ToDouble((double)grp.Sum(x => (double)x.acs.Total) * 100 / grp.Key.MonthTarget), 2, MidpointRounding.ToEven)
                              }).ToListAsync();

                foreach(var query in queries.OrderBy(x => x.Month))
                {
                    response.Add(new SellInMonthPercentage
                    {
                        Name = query.Name,
                        EmployeeId = query.EmployeeId,
                        Month = query.Month,
                        Monthname = months[query.Month - 1],
                        TotalActual = query.TotalActual,
                        TotalTarget = query.MonthTarget,
                        Percentage = query.Percentage
                    });
                }

                return StatusCode(200, response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSpvSellInMonthPercentageByLinq")] //spGetTotalSupervisorSaleByMonthWithPercentage
        public async Task<IActionResult> GetSpvSellInMonthPercentageByLinq()
        {
            try
            {
                List<SellInMonthPercentage> response = new List<SellInMonthPercentage>();

                string[] months = new string[3] { "January", "February", "March" };

                var queries = await (from emp in _context.Employees
                               join acs in _context.ActualSales on emp.Id equals acs.EmployeeId
                               join spv in _context.Employees on emp.SupervisorID equals spv.Id
                               join tgt in _context.Targets on spv.Id equals tgt.EmployeeId
                               group new
                               {
                                   emp,
                                   acs,
                                   spv,
                                   tgt
                               } by new
                               {
                                   emp.SupervisorID,
                                   spv.Name,
                                   tgt.MonthTarget,
                                   acs.SaleDate.Month
                               } into grp
                               where grp.Key.SupervisorID != null
                               select new
                               {
                                   grp.Key.Name,
                                   grp.Key.SupervisorID,
                                   grp.Key.Month,
                                   grp.Key.MonthTarget,
                                   TotalActual = grp.Sum(x => x.acs.Total),
                                   Percentage = Math.Round(Convert.ToDouble((double)grp.Sum(x => (double)x.acs.Total) * 100 / grp.Key.MonthTarget), 2, MidpointRounding.ToEven)
                               }).ToListAsync();

                foreach (var query in queries.OrderBy(x => x.SupervisorID))
                {
                    response.Add(new SellInMonthPercentage
                    {
                        Name = query.Name,
                        EmployeeId = query.SupervisorID,
                        Month = query.Month,
                        Monthname = months[query.Month - 1],
                        TotalActual = query.TotalActual,
                        TotalTarget = query.MonthTarget,
                        Percentage = query.Percentage
                    });
                }

                return StatusCode(200, response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
