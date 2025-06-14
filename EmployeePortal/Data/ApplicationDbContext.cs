using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace EmployeePortal.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }
        public DbSet<Employee>Employees { get; set; }
    }
}
