using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface ICareerProfileRepository
    {
        Task<CareerProfile?> GetByUserIdAsync(Guid userId);

        Task<CareerProfile> CreateAsync(CareerProfile careerProfile);

        Task<CareerProfile> UpdateAsync(CareerProfile careerProfile);
    }
}