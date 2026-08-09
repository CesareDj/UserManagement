using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface ICountryService
    {
        IAsyncEnumerable<CountryResponse> GetCountriesAsync();
        Task<CountryResponse?> GetCountryAsync(int id);
        Task<Country> CreateCountryAsync(Country country);
        Task<Country?> UpdateCountryAsync(Country country);
        Task<Country?> DeleteCountryAsync(int id);
    }
}
