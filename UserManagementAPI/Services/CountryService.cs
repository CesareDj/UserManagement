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

        public async Task<Country> CreateCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<Country?> UpdateCountryAsync(int id, Country country)
        {
            if (id != country.Id)
            {
                return null;
            }

            _context.Entry(country).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<Country?> DeleteCountryAsync(int id)
        {
            Country country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return null;
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return country;
        }
    }
}