using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.Tests.DomainTests
{
    [TestClass]
    public class GenericRepositoryTests
    {
        private Mock<ReAppContext> _mockContext;
        private GenericRepository _genericRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<ReAppContext>();
            _genericRepository = new GenericRepository(_mockContext.Object);
        }
    }
}
