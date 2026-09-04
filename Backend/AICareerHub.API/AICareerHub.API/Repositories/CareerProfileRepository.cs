using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class CareerProfileRepository : ICareerProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public CareerProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CareerProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.CareerProfiles
                .FirstOrDefaultAsync(profile => profile.UserId == userId);
        }

        public async Task<CareerProfile> CreateAsync(CareerProfile careerProfile)
        {
            _context.CareerProfiles.Add(careerProfile);

            await _context.SaveChangesAsync();

            return careerProfile;
        }

        public async Task<CareerProfile> UpdateAsync(CareerProfile careerProfile)
        {
            _context.CareerProfiles.Update(careerProfile);

            await _context.SaveChangesAsync();

            return careerProfile;
        }
    }
}