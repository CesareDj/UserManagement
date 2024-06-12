using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        Task<bool> AnyUserExistsAsync();
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserAsync(int id);
        Task<User> CreateUserAsync(UserDto userDto);
        Task<List<User>> CreateUsersAsync(List<UserDto> userDtos);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(int id);
    }
}
