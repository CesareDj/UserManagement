using Newtonsoft.Json;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class DatabaseInitializationService : IDatabaseInitializationService
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public DatabaseInitializationService(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        public async Task InitializeAsync()
        {
            var jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", "Users.json");
            if (File.Exists(jsonFilePath))
            {
                var json = await File.ReadAllTextAsync(jsonFilePath);
                var userDtos = JsonConvert.DeserializeObject<List<UserDto>>(json);
                await _userService.CreateUsersAsync(userDtos);
            }
            else
            {
                Console.WriteLine("No se pudo encontrar el archivo Users.json en la carpeta Data.");
            }
        }
    }
}
