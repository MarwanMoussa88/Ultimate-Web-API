using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet("GetEmployeesAdo")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAdo()
        {
            var res = await employeeService.GetEmployeesAdo();
            return Ok(res);
        }

        [HttpGet("GetEmployeesDapper")]
        public async Task<IActionResult> GetEmployeesDapper()
        {
            var res = await employeeService.GetEmployeesDapper();
            return Ok(res);
        }


    }
}
