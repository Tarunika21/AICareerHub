using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);

        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto changePasswordDto);

        Task<bool> DeleteUserAsync(Guid id);
    }
}
