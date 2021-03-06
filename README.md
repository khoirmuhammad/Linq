# LINQ
Learn more about linq

## Introduction
I deliberate creating this repository in order to get deep knowledge for myself, about complex query using linq. 
Also I'm not forget to create equivalent query in store procedure so I able to easy remember, what does the LINQ mean that written by me.

This project is created using Database First Approach

Here, I have several tables relation

### A. Employees, Targets, & ActualSales

<b>A1. Employees</b> : In order to store employee data and supervisor hierarcy

![Test Image 1](https://github.com/khoirmuhammad/Linq/blob/master/Employee.PNG)

<b>A2. Targets</b> : In order to set daily target and monthly target for each employee, either team member or supervisor

![Test Image 2](https://github.com/khoirmuhammad/Linq/blob/master/Target.PNG)

<b>A3. ActualSales</b> : In order to record actual sales for employee respectively. All you guys able to add more transactions.

![Test Image 2](https://github.com/khoirmuhammad/Linq/blob/master/Actual%20Sales.PNG)


What next? You can go to SellController.cs to get more detail about the implementation.


## Case 1
I'm going to show sales per month by all team members. Data Required
#### a. Employee Name : Employees Table
#### b. Employee ID : Employees Table
#### c. Month Name : built-in SQL
#### d. Month : built-in SQL
#### e. Total Target : Targets Table
#### f. Percentage : arithmetic operation

<b>SQL Stored Procedure</b>
------------------------------------------------------------------------------------------------------------------
```
ALTER PROCEDURE [dbo].[spGetTotalSaleByMonthWithPercentage]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	 select emp.Name, 
	  acs.EmployeeId, 
	  DATENAME(MONTH, acs.SaleDate) as Monthname, 
	  DATEPART(MONTH, acs.SaleDate) as Month, 
	  cast(sum(acs.Total) as int) as TotalActual,
	  cast(tgt.MonthTarget as int) as TotalTarget,
	  ROUND(CAST((sum(acs.Total) * 100.0 / tgt.MonthTarget) AS FLOAT), 2) AS Percentage
	  from ActualSales as acs
	  join Employees emp on emp.Id = acs.EmployeeId
	  join Targets tgt on emp.Id = tgt.EmployeeId
	  group by emp.Name, acs.EmployeeId, DATENAME(MONTH, acs.SaleDate), DATEPART(MONTH, acs.SaleDate), tgt.MonthTarget
	  order by month
    
END
```
---------------------------------------------------------------------------------------------------------------------

<b> C# LINQ </b>
---------------------------------------------------------------------------------------------------------------------
```
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
```
---------------------------------------------------------------------------------------------------------------------

## Case 2
I'm going to show sales per month by all supervisors. This will add up the sales of all team members who are under the supervisor hierarchy
#### a. Employee Name : Employees Table (1)
#### b. Employee ID : Employees Table (2)
#### c. Month Name : built-in SQL
#### d. Month : built-in SQL
#### e. Total Target : Targets Table
#### f. Percentage : arithmetic operation

<b> SQL Stored Procedure </b>
----------------------------------------------------------------------------------------------------------------------
```
ALTER PROCEDURE [dbo].[spGetTotalSupervisorSaleByMonthWithPercentage] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select 
emp.SupervisorID as EmployeeId, 
emp2.Name, 
sum(acs.Total) TotalActual, 
cast(tgt.MonthTarget as int) as TotalTarget,
DATENAME(MONTH, acs.SaleDate) as Monthname, 
DATEPART(MONTH, acs.SaleDate) as Month,
ROUND(CAST((sum(acs.Total) * 100.0 / tgt.MonthTarget) AS FLOAT), 2) AS Percentage
from Employees emp 
join ActualSales acs on emp.Id = acs.EmployeeId
join Employees emp2 on emp.SupervisorID = emp2.Id
join Targets tgt on emp2.Id = tgt.EmployeeId
group by emp.SupervisorID, emp2.Name, tgt.MonthTarget, DATENAME(MONTH, acs.SaleDate), DATEPART(MONTH, acs.SaleDate)
having emp.SupervisorID is not null
END
```
------------------------------------------------------------------------------------------------------------------------

<b> C# Linq </b>
------------------------------------------------------------------------------------------------------------------------
```
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
```
