using Newtonsoft.Json;
using UserManagementAPI.DTOs;

namespace UserManagementAPI.Services
{
    public class DatabaseInitializationService(IUserService userService, IWebHostEnvironment env) : IDatabaseInitializationService
    {
        private readonly IUserService _userService = userService;
        private readonly IWebHostEnvironment _env = env;

        public async Task InitializeAsync()
        {
            string jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", "Users.json");

            if (File.Exists(jsonFilePath))
            {
                await _userService.CreateUsersAsync(JsonConvert.DeserializeObject<List<UserDto>>(await File.ReadAllTextAsync(jsonFilePath)));
            }
            else
            {
                Console.WriteLine("No se pudo encontrar el archivo Users.json en la carpeta Data.");
            }
        }
    }
}