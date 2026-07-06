using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.Tests.DomainTests;

[TestClass]
public class GenericRepositoryTests
{
    private GenericRepository _genericRepository;
    private Mock<ReAppContext> _mockContext;

    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<ReAppContext>();
        _genericRepository = new GenericRepository(_mockContext.Object);
    }
}
