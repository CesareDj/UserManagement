using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public Task<List<Company>> GetCompaniesAsync()
        {
            return _companyService.GetCompaniesAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyAsync(int id)
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

        // PUT: api/Companies/5
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