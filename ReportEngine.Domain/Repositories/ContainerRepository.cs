using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories;

public class ContainerRepository
{
    private readonly ReAppContext _context;

    public ContainerRepository(ReAppContext context)
    {
        _context = context;
    }

}
