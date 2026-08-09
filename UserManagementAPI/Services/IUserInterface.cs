using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        Task<bool> AnyUserExistsAsync();
        IAsyncEnumerable<UserResponse> GetUsersAsync();
        Task<UserResponse?> GetUserAsync(int id);
        Task<User> CreateUserAsync(UserDto userDto);
        Task<List<User>> CreateUsersAsync(List<UserDto> userDtos);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(int id);
    }
}
