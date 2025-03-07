using EmployeeAdmin2.Data;
using EmployeeAdmin2.Model;
using EmployeeAdmin2.Model.Entities;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeAdmin2.Controllers
{
    // localhost:5000/api/employees
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(int page = 1, int limit = 10)
        {

            try
            {
                if (page < 1 || limit < 1)
                {
                    return BadRequest("Invalid pagination parameters.");
                }

                var allEmployees = await dbContext.Employees
                                             .Skip((page - 1) * limit)
                                             .Take(limit)
                                             .ToListAsync();

                var response = new
                {
                    PageNumber = page,
                    PageSize = limit,
                    Employees = allEmployees
                };

                return Ok(response);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest($"Invalid Patch Request: {ex.Message}");

            }


        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetAllEmployeById(Guid id)
        {
            try
            {
                var employee = await dbContext.Employees.FindAsync(id);
                if (employee is null)
                {
                    return NotFound();
                }
                var employeeList = new List<Employee>() { employee };
                return Ok(employeeList);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest($"Invalid Patch Request: {ex.Message}");

            }

        }

        [HttpGet]
        [Route("/api/searchEmployee/{name}")]
        public async Task<IActionResult> SearchEmployee(string name)
        {
            try
            {
                // If no search term provided, return all employees
                if (string.IsNullOrWhiteSpace(name))
                {
                    var allEmployees = await dbContext.Employees.ToListAsync();
                    return Ok(allEmployees);
                }

                // Perform case-insensitive partial search
                var employees = await dbContext.Employees
                                               .Where(e => EF.Functions.Like(e.Name, name + "%")) // Name starts with 'name'
                                               .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error searching for employees: {ex.Message}");
            }
        }



        [HttpPost]
        public IActionResult addEmployee(AddEmployeeDto addEmployeeDto)
        {
            var employeeEntity = new Employee()
            {
                Name = addEmployeeDto.Name,
                Email = addEmployeeDto.Email,
                Phone = addEmployeeDto.Phone,
                Salary = addEmployeeDto.Salary
            };
            dbContext.Employees.Add(employeeEntity);
            dbContext.SaveChanges();
            return Ok(employeeEntity);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] AddEmployeeDto addEmployeeDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await dbContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                employee.Name = addEmployeeDto.Name;
                employee.Email = addEmployeeDto.Email;
                employee.Phone = addEmployeeDto.Phone;
                employee.Salary = addEmployeeDto.Salary;

                dbContext.Employees.Update(employee);
                await dbContext.SaveChangesAsync();
                return NoContent();


            }
            catch (JsonPatchException ex)
            {
                return BadRequest($"Invalid Patch Request: {ex.Message}");

            }
        }

        [HttpDelete]
        [Route("/api/deleteEmployee/{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted Successfully");
        }
    }

}