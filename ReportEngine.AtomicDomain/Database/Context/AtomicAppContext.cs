using Microsoft.EntityFrameworkCore;
using ReportEngine.AtomicDomain.Entities;

namespace ReportEngine.AtomicDomain.Database.Context;

public class AtomicAppContext : DbContext
{
    public AtomicAppContext(DbContextOptions<AtomicAppContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
}
