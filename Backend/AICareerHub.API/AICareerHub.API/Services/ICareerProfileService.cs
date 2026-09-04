using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface ICareerProfileService
    {
        Task<CareerProfileDto?> GetByUserIdAsync(Guid userId);

        Task<CareerProfileDto> CreateAsync(
            Guid userId,
            CreateCareerProfileDto createDto);

        Task<CareerProfileDto?> UpdateAsync(
            Guid userId,
            UpdateCareerProfileDto updateDto);
    }
}