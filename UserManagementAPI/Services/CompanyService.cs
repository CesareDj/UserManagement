using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly UserManagementDbContext _context;

        public CompanyService(UserManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<Company>> GetCompaniesAsync()
        {
            return _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetCompanyAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company?> UpdateCompanyAsync(int id, Company company)
        {
            if (id != company.Id)
            {
                return null;
            }

            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company?> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return null;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return company;
        }
    }
}
