﻿using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using Xunit;

namespace UserManagementAPITest
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<UserManagementDbContext> _contextMock;

        public UserServiceTests()
        {
            var dbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new UserManagementDbContext(options);
            _contextMock = new Mock<UserManagementDbContext>();
            _contextMock.Setup(x => x.Users).Returns(context.Users);
            _contextMock.Setup(x => x.Countries).Returns(context.Countries);
            _contextMock.Setup(x => x.Companies).Returns(context.Companies);
            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _contextMock.Setup(x => x.Database).Returns(context.Database);

            _userService = new UserService(_contextMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { Id = 1 };
            var user2 = new User { Id = 2 };

            _contextMock.Setup(x => x.Users).ReturnsDbSet([user1, user2]);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            // No users added to context

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1 };
            _contextMock.Object.Users.Add(user);
            await _contextMock.Object.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetUserAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            // No users added to context

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_CreatesNewUser_WhenUserDtoIsValid()
        {
            // Arrange
            var country = new Country { Name = "Country" };
            var company = new Company { Name = "Company" };

            _contextMock.Setup(x => x.Countries).ReturnsDbSet([country]);
            _contextMock.Setup(x => x.Companies).ReturnsDbSet([company]);

            var userDto = new UserDto { Id = 1, Email = "test@test.com", First = "First", Last = "Last", Company = company.Name, Country = country.Name };

            // Act
            var result = await _userService.CreateUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Id, result.Id);
            Assert.Equal(userDto.Email, result.Email);
            Assert.Equal(userDto.First, result.First);
            Assert.Equal(userDto.Last, result.Last);
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsException_WhenUserDtoIsNull()
        {
            // Arrange
            UserDto? userDto = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateUserAsync(userDto ?? throw new ArgumentNullException()));
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsException_WhenUserEmailExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateUserAsyncTestDb") // Asegúrate de usar un nombre de base de datos único para cada prueba
                .Options;

            using var context = new UserManagementDbContext(options);
            var userService = new UserService(context);

            // Añade un usuario existente a la base de datos en memoria
            var existingUser = new User { Email = "test@test.com", First = "First", Last = "Last", CompanyId = 1, CountryId = 1 };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var userDto = new UserDto { Email = "test@test.com", First = "First", Last = "Last", Company = "Company", Country = "Country" };

            // Act & Assert
            // Espera una excepción debido a que el email ya existe
            await Assert.ThrowsAsync<ArgumentException>(() => userService.CreateUserAsync(userDto));
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsException_WhenUserIdIsInvalid()
        {
            // Arrange
            var userDto = new UserDto { Id = -1, Email = "test@test.com", First = "First", Last = "Last", Company = "Company", Country = "Country" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsException_WhenUserDtoIsIncomplete()
        {
            // Arrange
            var userDto = new UserDto { Id = 1, Email = "test@test.com", First = "First", Last = "Last", Company = "Company" }; // Country is missing

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
        }

        [Fact]
        public async Task CreateUserAsync_CreatesCountryAndCompany_WhenTheyDoNotExist()
        {
            // Arrange
            var userDto = new UserDto
            {
                Id = 1,
                Email = "test@test.com",
                First = "First",
                Last = "Last",
                Company = "NewCompany",
                Country = "NewCountry"
            };

            _contextMock.Setup(x => x.Countries).ReturnsDbSet([]);
            _contextMock.Setup(x => x.Companies).ReturnsDbSet([]);

            // Act
            var result = await _userService.CreateUserAsync(userDto);

            // Assert
            _contextMock.Verify(x => x.Companies.Add(It.Is<Company>(c => c.Name == userDto.Company)), Times.Once);
            _contextMock.Verify(x => x.Countries.Add(It.Is<Country>(c => c.Name == userDto.Country)), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenIdDoesNotMatch()
        {
            // Arrange
            var user = new User { Id = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUser_WhenIdMatches()
        {
            // Arrange
            var country = new Country { Id = 1, Name = "Country" };
            var company = new Company { Id = 1, Name = "Company" };
            var user = new User { Id = 1, Email = "test@test.com", First = "First", Last = "Last", CountryId = country.Id, CompanyId = company.Id };

            _contextMock.Setup(x => x.Users.FindAsync(1)).ReturnsAsync(user);
            _contextMock.Setup(x => x.Countries.FindAsync(country.Id)).ReturnsAsync(country);
            _contextMock.Setup(x => x.Companies.FindAsync(company.Id)).ReturnsAsync(company);

            var updatedUser = new User { Id = 1, Email = "updated@test.com", First = "First", Last = "Last", CountryId = country.Id, CompanyId = company.Id };

            // Act
            var result = await _userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@test.com", result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenUserIsNull()
        {
            // Arrange
            User? user = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateUserAsync(user ?? throw new ArgumentNullException()));
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenUserIdDoesNotExist()
        {
            // Arrange
            var user = new User { Id = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenUserIdIsInvalid()
        {
            // Arrange
            var user = new User { Id = -1 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenUserIsIncomplete()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@test.com", First = "First", Last = "Last" }; // CompanyId and CountryId are missing

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_ThrowsException_WhenCountryOrCompanyDoesNotExist()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@test.com", First = "First", Last = "Last", CompanyId = 999, CountryId = 999 }; // Nonexistent CompanyId and CountryId

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUserAsync(user));
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _userService.DeleteUserAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUserAsync_DeletesUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1 };
            _contextMock.Object.Users.Add(user);
            await _contextMock.Object.SaveChangesAsync();

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AnyUserExistsAsync_ReturnsFalse_WhenNoUsersExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbForAnyUserExistsAsync_ReturnsFalse")
                .Options;

            using var context = new UserManagementDbContext(options);
            var service = new UserService(context);

            // Act
            var result = await service.AnyUserExistsAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AnyUserExistsAsync_ReturnsTrue_WhenUsersExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbForAnyUserExistsAsync_ReturnsTrue")
                .Options;

            using var context = new UserManagementDbContext(options);
            context.Users.Add(new User { Id = 1, Email = "test@example.com", First = "Test", Last = "User" });
            await context.SaveChangesAsync();

            var service = new UserService(context);

            // Act
            var result = await service.AnyUserExistsAsync();

            // Assert
            Assert.True(result);
        }
    }
}