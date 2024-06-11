using Moq;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UserManagementAPITest
{
    public class CompanyServiceTests
    {
        private readonly Mock<UserManagementDbContext> _mockContext;
        private readonly CompanyService _companyService;

        public CompanyServiceTests()
        {
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<UserManagementDbContext>(options);
            _companyService = new CompanyService(_mockContext.Object);
        }

        [Fact]
        public async Task GetCompaniesAsync_ReturnsAllCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new() { Id = 1, Name = "Company1" },
                new() { Id = 2, Name = "Company2" }
            };

            _mockContext.Setup(db => db.Companies).Returns(companies.AsQueryable().BuildMockDbSet().Object);

            // Act
            var result = await _companyService.GetCompaniesAsync();

            // Assert
            Assert.Equal(companies, result);
        }

        [Fact]
        public async Task GetCompanyAsync_ReturnsCompany_WhenCompanyExists()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Company1" };
            _mockContext.Setup(db => db.Companies.FindAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _companyService.GetCompanyAsync(1);

            // Assert
            Assert.Equal(company, result);
        }

        [Fact]
        public async Task GetCompanyAsync_ReturnsNull_WhenCompanyDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(db => db.Companies.FindAsync(1)).ReturnsAsync((Company)null);

            // Act
            var result = await _companyService.GetCompanyAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCompanyAsync_ThrowsException_WhenCompanyIsNull()
        {
            // Arrange
            Company company = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _companyService.CreateCompanyAsync(company));
        }

        [Fact]
        public async Task CreateCompanyAsync_ReturnsCreatedCompany()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Company1" };
            _mockContext.Setup(db => db.Companies.AddAsync(company, It.IsAny<CancellationToken>())).ReturnsAsync((EntityEntry<Company>)null);

            // Act
            var result = await _companyService.CreateCompanyAsync(company);

            // Assert
            Assert.Equal(company, result);
        }

        [Fact]
        public async Task UpdateCompanyAsync_ThrowsException_WhenCompanyIsNull()
        {
            // Arrange
            Company company = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _companyService.UpdateCompanyAsync(company));
        }

        [Fact]
        public async Task UpdateCompanyAsync_ThrowsException_WhenCompanyIdIsNotPositive()
        {
            // Arrange
            var company = new Company { Id = 0, Name = "Company1" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _companyService.UpdateCompanyAsync(company));
        }

        [Fact]
        public async Task UpdateCompanyAsync_ReturnsUpdatedCompany()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Company1" };
            _mockContext.Setup(db => db.Companies.Update(company)).Returns((EntityEntry<Company>)null);

            // Act
            var result = await _companyService.UpdateCompanyAsync(company);

            // Assert
            Assert.Equal(company, result);
        }

        [Fact]
        public async Task DeleteCompanyAsync_ThrowsException_WhenCompanyDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(db => db.Companies.FindAsync(1)).ReturnsAsync((Company)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _companyService.DeleteCompanyAsync(1));
        }

        [Fact]
        public async Task DeleteCompanyAsync_ReturnsDeletedCompany()
        {
            // Arrange
            var company = new Company { Id = 1, Name = "Company1" };
            _mockContext.Setup(db => db.Companies.FindAsync(1)).ReturnsAsync(company);
            _mockContext.Setup(db => db.Companies.Remove(company)).Returns((EntityEntry<Company>)null);

            // Act
            var result = await _companyService.DeleteCompanyAsync(1);

            // Assert
            Assert.Equal(company, result);
        }
    }
}