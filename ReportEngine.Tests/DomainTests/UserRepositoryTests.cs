global using Moq;
using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;

namespace ReportEngine.Tests.DomainTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private Mock<ReAppContext> _mockContext;
        private Mock<DbSet<User>> _mockSet;
        private UserRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<ReAppContext>(new DbContextOptions<ReAppContext>());
            _mockSet = new Mock<DbSet<User>>();
            _mockContext.Setup(c => c.Set<User>()).Returns(_mockSet.Object);
            _repository = new UserRepository(_mockContext.Object);
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddUser()
        {
            var user = new User { Name = "Test" };
            await _repository.AddAsync(user);
            _mockSet.Verify(s => s.AddAsync(user, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            var data = new List<User> { new User { Name = "A" }, new User { Name = "B" } }.AsQueryable();
            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var result = await _repository.GetAllAsync();
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            var user = new User { Id = 1, Name = "Test" };
            _mockSet.Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default)).ReturnsAsync(user);
            var result = await _repository.GetByIdAsync(1);
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var user = new User { Id = 1, Name = "Test" };
            _mockSet.Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default)).ReturnsAsync(user);
            _mockContext.Setup(c => c.Entry(user).CurrentValues.SetValues(user));
            await _repository.UpdateAsync(user);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            var user = new User { Id = 1, Name = "Test" };
            _mockSet.Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default)).ReturnsAsync(user);
            _mockSet.Setup(s => s.Remove(user));
            await _repository.DeleteAsync(user);
            _mockSet.Verify(s => s.Remove(user), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task DeleteByIdAsync_ShouldDeleteUserById()
        {
            var user = new User { Id = 1, Name = "Test" };
            _mockSet.Setup(s => s.FindAsync(1)).ReturnsAsync(user);
            _mockSet.Setup(s => s.Remove(user));
            var result = await _repository.DeleteByIdAsync(1);
            Assert.AreEqual(1, result);
            _mockSet.Verify(s => s.Remove(user), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
