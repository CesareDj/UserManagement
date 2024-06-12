using Newtonsoft.Json;
using UserManagementAPI.DTOs;

namespace UserManagementAPI.Services
{
    public class DatabaseInitializationService(IUserService userService, IWebHostEnvironment env) : IDatabaseInitializationService
    {
        private readonly IUserService _userService = userService;
        private readonly IWebHostEnvironment _env = env;

        async Task IDatabaseInitializationService.InitializeAsync()
        {
            string jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", "Users.json");

            if (File.Exists(jsonFilePath))
            {
                bool anyUserExists = await _userService.AnyUserExistsAsync();

                if (!anyUserExists)
                {
                    List<UserDto> users = JsonConvert.DeserializeObject<List<UserDto>>(await File.ReadAllTextAsync(jsonFilePath)) ?? [];

                    if (users.Count > 0)
                    {
                        await _userService.CreateUsersAsync(users);
                    }
                    else
                    {
                        Console.WriteLine("El contenido del archivo Users.json no se pudo deserializar a una lista de UserDto o está vacío.");
                    }
                }
                else
                {
                    Console.WriteLine("Ya existen usuarios en la base de datos. No se inicializarán datos.");
                }
            }
            else
            {
                Console.WriteLine("No se pudo encontrar el archivo Users.json en la carpeta Data.");
            }
        }
    }
}