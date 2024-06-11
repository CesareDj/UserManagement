using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompaniesAsync();
        Task<Company?> GetCompanyAsync(int id);
        Task<Company> CreateCompanyAsync(Company company);
        Task<Company?> UpdateCompanyAsync(int id, Company company);
        Task<Company?> DeleteCompanyAsync(int id);
    }
}
