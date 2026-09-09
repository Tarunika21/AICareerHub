using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class ResumeProjectService
        : IResumeProjectService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IResumeProjectRepository _projectRepository;

        public ResumeProjectService(
            IResumeRepository resumeRepository,
            IResumeProjectRepository projectRepository)
        {
            _resumeRepository = resumeRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ResumeProjectDto>?> GetAllAsync(
            Guid resumeId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var projects =
                await _projectRepository.GetByResumeIdAsync(
                    resumeId);

            return projects.Select(MapToDto);
        }

        public async Task<ResumeProjectDto?> GetByIdAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var project =
                await _projectRepository.GetByIdAsync(
                    projectId,
                    resumeId);

            return project == null
                ? null
                : MapToDto(project);
        }

        public async Task<ResumeProjectDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeProjectDto createDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var project = new ResumeProject
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                Name = createDto.Name.Trim(),
                Description = createDto.Description.Trim(),
                Technologies = createDto.Technologies.Trim(),
                ProjectUrl = createDto.ProjectUrl?.Trim()
            };

            var createdProject =
                await _projectRepository.CreateAsync(
                    project);

            return MapToDto(createdProject);
        }

        public async Task<ResumeProjectDto?> UpdateAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId,
            UpdateResumeProjectDto updateDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var project =
                await _projectRepository.GetByIdAsync(
                    projectId,
                    resumeId);

            if (project == null)
            {
                return null;
            }

            project.Name = updateDto.Name.Trim();
            project.Description = updateDto.Description.Trim();
            project.Technologies = updateDto.Technologies.Trim();
            project.ProjectUrl = updateDto.ProjectUrl?.Trim();

            var updatedProject =
                await _projectRepository.UpdateAsync(
                    project);

            return MapToDto(updatedProject);
        }

        public async Task<bool> DeleteAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return false;
            }

            var project =
                await _projectRepository.GetByIdAsync(
                    projectId,
                    resumeId);

            if (project == null)
            {
                return false;
            }

            await _projectRepository.DeleteAsync(project);

            return true;
        }

        private static ResumeProjectDto MapToDto(
            ResumeProject project)
        {
            return new ResumeProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Technologies = project.Technologies,
                ProjectUrl = project.ProjectUrl
            };
        }
    }
}