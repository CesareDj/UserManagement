using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService(UserManagementDbContext context) : IUserService
    {
        private readonly UserManagementDbContext _context = context;

        public async Task<bool> AnyUserExistsAsync()
        {
            return await _context.Users.AnyAsync();
        }

        public IAsyncEnumerable<UserResponse> GetUsersAsync()
        {
            return _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    First = u.First,
                    Last = u.Last,
                    CountryName = u.Country != null ? u.Country.Name : string.Empty,
                    CompanyName = u.Company != null ? u.Company.Name : string.Empty,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .AsAsyncEnumerable();
        }

        public async Task<UserResponse?> GetUserAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    First = u.First,
                    Last = u.Last,
                    CountryName = u.Country != null ? u.Country.Name : string.Empty,
                    CompanyName = u.Company != null ? u.Company.Name : string.Empty,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            if (userDto.Id < 0)
            {
                throw new ArgumentException("El ID del usuario es inválido.", nameof(userDto));
            }

            if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.First) || string.IsNullOrEmpty(userDto.Last) || string.IsNullOrEmpty(userDto.Country) || string.IsNullOrEmpty(userDto.Company))
            {
                throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
            }

            User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                throw new ArgumentException($"Un usuario con el Email {userDto.Email} ya existe.");
            }

            Country? country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == userDto.Country);

            if (country == null)
            {
                country = new Country { Name = userDto.Country };
                _context.Countries.Add(country);
            }

            Company? company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == userDto.Company);

            if (company == null)
            {
                company = new Company { Name = userDto.Company };
                _context.Companies.Add(company);
            }

            User user = new()
            {
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
            if (userDtos == null || userDtos.Count == 0)
            {
                throw new ArgumentException("La lista de UserDto no puede ser nula o vacía.");
            }

            // Validar todos los DTOs antes de procesar
            foreach (UserDto userDto in userDtos)
            {
                if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.First) || string.IsNullOrEmpty(userDto.Last) || string.IsNullOrEmpty(userDto.Country) || string.IsNullOrEmpty(userDto.Company))
                {
                    throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
                }
            }

            // Extraer nombres únicos de países y empresas
            HashSet<string> uniqueCountryNames = userDtos.Select(d => d.Country!).ToHashSet(StringComparer.OrdinalIgnoreCase);
            HashSet<string> uniqueCompanyNames = userDtos.Select(d => d.Company!).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Consultar países y empresas existentes en una sola query cada uno
            Dictionary<string, Country> existingCountries = await _context.Countries
                .Where(c => uniqueCountryNames.Contains(c.Name))
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

            Dictionary<string, Company> existingCompanies = await _context.Companies
                .Where(c => uniqueCompanyNames.Contains(c.Name))
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

            // Insertar países faltantes en batch
            foreach (string countryName in uniqueCountryNames)
            {
                if (!existingCountries.ContainsKey(countryName))
                {
                    Country newCountry = new() { Name = countryName };
                    _context.Countries.Add(newCountry);
                    existingCountries[countryName] = newCountry;
                }
            }

            // Insertar empresas faltantes en batch
            foreach (string companyName in uniqueCompanyNames)
            {
                if (!existingCompanies.ContainsKey(companyName))
                {
                    Company newCompany = new() { Name = companyName };
                    _context.Companies.Add(newCompany);
                    existingCompanies[companyName] = newCompany;
                }
            }

            // Guardar países y empresas nuevos en un solo SaveChanges
            await _context.SaveChangesAsync();

            // Construir usuarios en memoria
            List<User> users = new(userDtos.Count);

            foreach (UserDto userDto in userDtos)
            {
                User user = new()
                {
                    Email = userDto.Email!,
                    First = userDto.First!,
                    Last = userDto.Last!,
                    CompanyId = existingCompanies[userDto.Company!].Id,
                    CountryId = existingCountries[userDto.Country!].Id
                };

                if (userDto.CreatedAt.HasValue)
                {
                    user.CreatedAt = userDto.CreatedAt.Value;
                }

                users.Add(user);
            }

            // Insertar usuarios en lotes para no saturar el Change Tracker
            const int batchSize = 1000;
            for (int i = 0; i < users.Count; i += batchSize)
            {
                List<User> batch = users.GetRange(i, Math.Min(batchSize, users.Count - i));
                _context.Users.AddRange(batch);
                await _context.SaveChangesAsync();
            }

            return users;
        }

        public async Task<List<User>> CreateUsersAsync(List<User> users)
        {
            if (users == null || users.Count == 0)
            {
                throw new ArgumentException("La lista de usuarios no puede ser nula o vacía.");
            }

            foreach (User user in users)
            {
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.First) || string.IsNullOrEmpty(user.Last) || user.CountryId == 0 || user.CompanyId == 0)
                {
                    throw new ArgumentException("Email, First, Last, CountryId o CompanyId no pueden ser nulos o vacíos.");
                }

                bool countryExists = await _context.Countries.AnyAsync(c => c.Id == user.CountryId);
                bool companyExists = await _context.Companies.AnyAsync(c => c.Id == user.CompanyId);

                if (!countryExists || !companyExists)
                {
                    throw new ArgumentException("Country o Company no existen.");
                }
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return users;
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.First) || string.IsNullOrEmpty(user.Last) || user.CountryId == 0 || user.CompanyId == 0)
            {
                throw new ArgumentException("Email, First, Last, Country or Company cannot be null or empty.");
            }

            var existingUser = await _context.Users.FindAsync(user.Id) ?? throw new ArgumentException("User does not exist.");
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