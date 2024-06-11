using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class CompanyService(UserManagementDbContext context) : ICompanyService
    {
        private readonly UserManagementDbContext _context = context;

        public Task<List<Company>> GetCompaniesAsync()
        {
            return _context.Companies.ToListAsync();
        }

        public Task<Company?> GetCompanyAsync(int id)
        {
            return _context.Companies.FindAsync(id).AsTask();
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company?> UpdateCompanyAsync(Company company)
        {
            if (company == null || company.Id <= 0)
            {
                throw new ArgumentException("Company is null or Company ID is not valid.");
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(company.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return company;
        }

        public async Task<Company?> DeleteCompanyAsync(int id)
        {
            Company company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return null;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return company;
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}