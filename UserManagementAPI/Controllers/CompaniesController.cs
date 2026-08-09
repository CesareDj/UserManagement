using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {
        private readonly ICompanyService _companyService = companyService;

        // GET: api/Companies
        [HttpGet]
        public IAsyncEnumerable<CompanyResponse> GetCompaniesAsync()
        {
            return _companyService.GetCompaniesAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyResponse>> GetCompanyAsync(int id)
        {
            var company = await _companyService.GetCompanyAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompanyAsync(Company company)
        {
            var createdCompany = await _companyService.CreateCompanyAsync(company);

            return CreatedAtAction(nameof(GetCompanyAsync), new { id = createdCompany.Id }, createdCompany);
        }

        // PUT: api/Companies
        [HttpPut]
        public async Task<IActionResult> PutCompanyAsync(Company company)
        {
            if (company == null || company.Id <= 0)
            {
                return BadRequest();
            }

            var updatedCompany = await _companyService.UpdateCompanyAsync(company);

            if (updatedCompany == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyAsync(int id)
        {
            var company = await _companyService.DeleteCompanyAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}