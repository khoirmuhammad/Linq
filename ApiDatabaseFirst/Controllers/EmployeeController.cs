using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDBContext _context;
        public EmployeeController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            var response = await _context.Employees.ToListAsync();

            return StatusCode(200, response);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _context.Employees.FindAsync(id);

            if (response != null)
                return StatusCode(200, response);
            else
                return StatusCode(404, "Data Not Found");
        }

        [HttpGet]
        [Route("GetByQueryId")]
        public async Task<IActionResult> GetByQueryId([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusCode(404, "Data Not Found");

            var response = await _context.Employees.FindAsync(id);

            if (response != null)
                return StatusCode(200, response);
            else
                return StatusCode(404, "Data Not Found");
        }
    }
}
