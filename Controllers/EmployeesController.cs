using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Get all employees with optional pagination and filtering by department
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(int pageNumber = 1, int pageSize = 5, string? department = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var query = dbContext.Employees.AsQueryable();

            // Apply filtering if department is provided
            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e => (e.Department ?? "").ToLower().Contains(department.ToLower()));
            }

            var totalRecords = await query.CountAsync();
            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (employees.Count == 0)
            {
                return NotFound(new { message = "No employees found matching your criteria." });
            }

            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = employees
            };

            return Ok(response);
        }

        // Get an employee by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee is null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            return Ok(employee);
        }

        // Add a new employee
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDto addEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newEmployee = new Employee()
            {
                FirstName = addEmployeeDto.FirstName,
                LastName = addEmployeeDto.LastName,
                Email = addEmployeeDto.Email,
                Department = addEmployeeDto.Department,
                Salary = addEmployeeDto.Salary
            };

            await dbContext.Employees.AddAsync(newEmployee);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Id }, newEmployee);
        }

        // Update an existing employee
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await dbContext.Employees.FindAsync(id);

            if (employee is null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.Email = updateEmployeeDto.Email;
            employee.Department = updateEmployeeDto.Department;
            employee.Salary = updateEmployeeDto.Salary;

            await dbContext.SaveChangesAsync();

            return Ok(employee);
        }

        // Delete an employee
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee is null)
            {
                return NotFound(new { message = "Employee not found." });
            }

            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Employee successfully deleted." });
        }
    }
}
