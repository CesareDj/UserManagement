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

        public async Task<Company> CreateCompanyAsync(Company? company)
        {
            ArgumentNullException.ThrowIfNull(company);

            _context.Companies.Add(company!);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company?> UpdateCompanyAsync(Company? company)
        {
            ArgumentNullException.ThrowIfNull(company);

            if (company.Id <= 0)
            {
                throw new ArgumentException("Company ID is not valid.");
            }

            Company? existingCompany = await _context.Companies.FindAsync(company.Id) ?? throw new ArgumentException($"No company with ID {company.Id} exists.");

            if (string.IsNullOrEmpty(company.Name) || existingCompany.Name == company.Name)
            {
                throw new ArgumentException("New name is invalid.");
            }

            existingCompany.Name = company.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Companies.Any(e => e.Id == company.Id))
                {
                    throw new ArgumentException($"No company with ID {company.Id} exists.");
                }
                else
                {
                    throw;
                }
            }

            return existingCompany;
        }

        public async Task<Company?> DeleteCompanyAsync(int id)
        {
            Company? company = await _context.Companies.FindAsync(id);
            ArgumentNullException.ThrowIfNull(company, $"No company with ID {id} exists.");

            _context.Companies.Remove(company!);
            await _context.SaveChangesAsync();

            return company;
        }
    }
}