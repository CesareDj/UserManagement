using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class CountryService : ICountryService
    {
        private readonly UserManagementDbContext _context;

        public CountryService(UserManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<Country>> GetCountriesAsync()
        {
            return _context.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryAsync(int id)
        {
            return await _context.Countries.FindAsync(id);
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
            var country = await _context.Countries.FindAsync(id);
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
