using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using Xunit;

namespace UserManagementAPITest
{
    public class CountryServiceTests
    {
        private readonly Mock<UserManagementDbContext> _mockContext;
        private readonly CountryService _countryService;

        public CountryServiceTests()
        {
            DbContextOptions<UserManagementDbContext> options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<UserManagementDbContext>(options);
            _countryService = new CountryService(_mockContext.Object);
        }

        [Fact]
        public async Task GetCountriesAsync_ReturnsAllCountries()
        {
            // Arrange
            List<Country> countries =
            [
                new() { Id = 1, Name = "Country1" },
                new() { Id = 2, Name = "Country2" }
            ];

            _mockContext.Setup(x => x.Countries).ReturnsDbSet(countries);

            // Act
            List<Country> result = await _countryService.GetCountriesAsync();

            // Assert
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task GetCountryAsync_ReturnsCountry_WhenCountryExists()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(country);

            // Act
            Country? result = await _countryService.GetCountryAsync(1);

            // Assert
            Assert.Equal(country, result);
        }

        [Fact]
        public async Task GetCountryAsync_ReturnsNull_WhenCountryDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(value: null);

            // Act
            Country? result = await _countryService.GetCountryAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCountryAsync_ThrowsException_WhenCountryIsNull()
        {
            // Arrange
            Country? country = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _countryService.CreateCountryAsync(country));
        }

        [Fact]
        public async Task CreateCountryAsync_ReturnsCreatedCountry()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.AddAsync(country, It.IsAny<CancellationToken>()));

            // Act
            Country result = await _countryService.CreateCountryAsync(country);

            // Assert
            Assert.Equal(country, result);
        }

        [Fact]
        public async Task UpdateCountryAsync_ThrowsException_WhenCountryDoesNotExist()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(value: null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _countryService.UpdateCountryAsync(country));
        }

        [Fact]
        public async Task UpdateCountryAsync_ThrowsException_WhenCountryIsNull()
        {
            // Arrange
            Country? country = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _countryService.UpdateCountryAsync(country));
        }

        [Fact]
        public async Task UpdateCountryAsync_ThrowsException_WhenCountryIdIsNotPositive()
        {
            // Arrange
            Country country = new() { Id = 0, Name = "Country1" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _countryService.UpdateCountryAsync(country));
        }

        [Fact]
        public async Task UpdateCountryAsync_ReturnsUpdatedCountry()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(country);

            Country updatedCountry = new() { Id = 1, Name = "UpdatedCountry1" };

            // Act
            Country? result = await _countryService.UpdateCountryAsync(updatedCountry);

            // Assert
            Assert.Equal(updatedCountry.Id, result?.Id);
            Assert.Equal(updatedCountry.Name, result?.Name);
        }

        [Fact]
        public async Task DeleteCountryAsync_ThrowsException_WhenCountryDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(value: null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _countryService.DeleteCountryAsync(1));
        }

        [Fact]
        public async Task UpdateCountryAsync_ThrowsException_WhenNewNameIsInvalid()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            Country updatedCountry = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(country);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _countryService.UpdateCountryAsync(updatedCountry));
        }

        [Fact]
        public async Task DeleteCountryAsync_ReturnsDeletedCountry()
        {
            // Arrange
            Country country = new() { Id = 1, Name = "Country1" };
            _mockContext.Setup(db => db.Countries.FindAsync(1)).ReturnsAsync(country);
            _mockContext.Setup(db => db.Countries.Remove(country));

            // Act
            Country? result = await _countryService.DeleteCountryAsync(1);

            // Assert
            Assert.Equal(country, result);
        }
    }
}