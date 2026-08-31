using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    }
}
