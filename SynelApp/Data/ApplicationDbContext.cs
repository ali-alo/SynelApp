using Microsoft.EntityFrameworkCore;
using SynelApp.Models;

namespace SynelApp.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // add unique constraint to a PayrollNumber column
            builder.Entity<Employee>()
                .HasIndex(e => e.PayrollNumber)
                .IsUnique();
        }
    }
}
