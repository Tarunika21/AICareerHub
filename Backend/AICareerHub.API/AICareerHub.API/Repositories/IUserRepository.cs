using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}