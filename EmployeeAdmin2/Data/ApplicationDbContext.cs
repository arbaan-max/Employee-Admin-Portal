using EmployeeAdmin2.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdmin2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
