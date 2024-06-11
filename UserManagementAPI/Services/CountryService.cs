using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class CountryService(UserManagementDbContext context) : ICountryService
    {
        private readonly UserManagementDbContext _context = context;

        public Task<List<Country>> GetCountriesAsync()
        {
            return _context.Countries.ToListAsync();
        }

        public Task<Country?> GetCountryAsync(int id)
        {
            return _context.Countries.FindAsync(id).AsTask();
        }

        public async Task<Country> CreateCountryAsync(Country? country)
        {
            ArgumentNullException.ThrowIfNull(country);

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<Country?> UpdateCountryAsync(Country? country)
        {
            ArgumentNullException.ThrowIfNull(country);

            if (country.Id <= 0)
            {
                throw new ArgumentException("Country ID is not valid.");
            }

            Country? existingCountry = await _context.Countries.FindAsync(country.Id) ?? throw new ArgumentException($"No country with ID {country.Id} exists.");

            if (string.IsNullOrEmpty(country.Name) || existingCountry.Name == country.Name)
            {
                throw new ArgumentException("New name is invalid.");
            }

            existingCountry.Name = country.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Countries.Any(e => e.Id == country.Id))
                {
                    throw new ArgumentException($"No country with ID {country.Id} exists.");
                }
                else
                {
                    throw;
                }
            }

            return existingCountry;
        }

        public async Task<Country?> DeleteCountryAsync(int id)
        {
            Country? country = await _context.Countries.FindAsync(id);
            ArgumentNullException.ThrowIfNull(country, $"No country with ID {id} exists.");

            _context.Countries.Remove(country!);
            await _context.SaveChangesAsync();

            return country;
        }
    }
}