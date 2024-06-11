using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        // GET: api/Countries
        [HttpGet]
        public Task<List<Country>> GetCountriesAsync()
        {
            return _countryService.GetCountriesAsync();
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountryAsync(int id)
        {
            var country = await _countryService.GetCountryAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountryAsync(Country country)
        {
            var createdCountry = await _countryService.CreateCountryAsync(country);

            return CreatedAtAction(nameof(GetCountryAsync), new { id = createdCountry.Id }, createdCountry);
        }

        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountryAsync(int id, Country country)
        {
            var updatedCountry = await _countryService.UpdateCountryAsync(id, country);

            if (updatedCountry == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryAsync(int id)
        {
            var country = await _countryService.DeleteCountryAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}