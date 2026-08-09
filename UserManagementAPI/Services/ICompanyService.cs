using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface ICompanyService
    {
        IAsyncEnumerable<CompanyResponse> GetCompaniesAsync();
        Task<CompanyResponse?> GetCompanyAsync(int id);
        Task<Company> CreateCompanyAsync(Company company);
        Task<Company?> UpdateCompanyAsync(Company company);
        Task<Company?> DeleteCompanyAsync(int id);
    }
}
