using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}