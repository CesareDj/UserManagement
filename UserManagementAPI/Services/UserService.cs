using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManagementDbContext _context;

        public UserService(UserManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            var country = _context.Countries.FirstOrDefault(c => c.Name == userDto.Country)
                          ?? _context.Countries.Add(new Country { Name = userDto.Country }).Entity;

            var company = _context.Companies.FirstOrDefault(c => c.Name == userDto.Company)
                           ?? _context.Companies.Add(new Company { Name = userDto.Company }).Entity;

            var user = new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                First = userDto.First,
                Last = userDto.Last,
                CompanyId = company.Id,
                CreatedAt = userDto.CreatedAt,
                CountryId = country.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> CreateUsersAsync(List<UserDto> userDtos)
        {
            var users = new List<User>();

            foreach (var userDto in userDtos)
            {
                var country = _context.Countries.FirstOrDefault(c => c.Name == userDto.Country)
                              ?? _context.Countries.Add(new Country { Name = userDto.Country }).Entity;

                var company = _context.Companies.FirstOrDefault(c => c.Name == userDto.Company)
                               ?? _context.Companies.Add(new Company { Name = userDto.Company }).Entity;

                users.Add(new User
                {
                    Id = userDto.Id,
                    Email = userDto.Email,
                    First = userDto.First,
                    Last = userDto.Last,
                    CompanyId = company.Id,
                    CreatedAt = userDto.CreatedAt,
                    CountryId = country.Id
                });
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return users;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            if (id != user.Id)
            {
                return null;
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
