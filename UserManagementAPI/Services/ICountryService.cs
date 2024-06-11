using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface ICountryService
    {
        Task<List<Country>> GetCountriesAsync();
        Task<Country?> GetCountryAsync(int id);
        Task<Country> CreateCountryAsync(Country country);
        Task<Country?> UpdateCountryAsync(Country country);
        Task<Country?> DeleteCountryAsync(int id);
    }
}
