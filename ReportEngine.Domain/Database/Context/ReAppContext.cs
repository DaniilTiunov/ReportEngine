using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Database.Context
{
    public class ReAppContext : DbContext
    {
        public DbSet<ProjectInfo> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }

        public ReAppContext(DbContextOptions<ReAppContext> options) : base(options)
        {
        }

    }
}
