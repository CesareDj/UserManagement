using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService(UserManagementDbContext context) : IUserService
    {
        private readonly UserManagementDbContext _context = context;

        public Task<List<User>> GetUsersAsync()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User?> GetUserAsync(int id)
        {
            return _context.Users.FindAsync(id).AsTask();
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.First) || string.IsNullOrEmpty(userDto.Last) || string.IsNullOrEmpty(userDto.Country) || string.IsNullOrEmpty(userDto.Company))
            {
                throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
            }

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == userDto.Country);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == userDto.Company);

            if (country == null || company == null)
            {
                throw new ArgumentException("Country or Company does not exist.");
            }

            var user = new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                First = userDto.First,
                Last = userDto.Last,
                CountryId = country.Id,
                CompanyId = company.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> CreateUsersAsync(List<UserDto> userDtos)
        {
            List<User> users = [];

            foreach (UserDto userDto in userDtos)
            {
                if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.First) || string.IsNullOrEmpty(userDto.Last) || string.IsNullOrEmpty(userDto.Country) || string.IsNullOrEmpty(userDto.Company))
                {
                    throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
                }

                Country? country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == userDto.Country);
                
                if (country == null)
                {
                    country = _context.Countries.Add(new Country { Name = userDto.Country }).Entity;
                    await _context.SaveChangesAsync();
                }

                Company? company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == userDto.Company);

                if (company == null)
                {
                    company = _context.Companies.Add(new Company { Name = userDto.Company }).Entity;
                    await _context.SaveChangesAsync();
                }

                users.Add(new User
                {
                    Id = userDto.Id,
                    Email = userDto.Email,
                    First = userDto.First,
                    Last = userDto.Last,
                    CompanyId = company.Id,
                    CreatedAt = userDto.CreatedAt ?? DateTime.UtcNow,
                    CountryId = country.Id
                });
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return users;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.First) || string.IsNullOrEmpty(user.Last) || user.CountryId == 0 || user.CompanyId == 0)
            {
                throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
            }

            if (id != user.Id)
            {
                throw new ArgumentException("Id does not match.");
            }

            var existingUser = await _context.Users.FindAsync(id) ?? throw new ArgumentException("User does not exist.");
            var country = await _context.Countries.FindAsync(user.CountryId);
            var company = await _context.Companies.FindAsync(user.CompanyId);

            if (country == null || company == null)
            {
                throw new ArgumentException("Country or Company does not exist.");
            }

            existingUser.Email = user.Email;
            existingUser.First = user.First;
            existingUser.Last = user.Last;
            existingUser.CountryId = country.Id;
            existingUser.CompanyId = company.Id;

            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User?> DeleteUserAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);

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