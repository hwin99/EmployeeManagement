using EmployeeManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
