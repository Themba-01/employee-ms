using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServerLibrary.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=34.35.64.33,1433;Database=EmployeeDb;User Id=Thembinkosi;Password=Themba1234;TrustServerCertificate=True");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}