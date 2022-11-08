using api.Adapters;
using api.Context;
using api.Dtos;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class WellProjectService
{
    private readonly DcdDbContext _context;
    private readonly ProjectService _projectService;
    private readonly ILogger<WellProjectService> _logger;

    public WellProjectService(DcdDbContext context, ProjectService projectService, ILoggerFactory loggerFactory)
    {
        _context = context;
        _projectService = projectService;
        _logger = loggerFactory.CreateLogger<WellProjectService>();
    }

    public IEnumerable<WellProject> GetWellProjects(Guid projectId)
    {
        if (_context.WellProjects != null)
        {
            return _context.WellProjects
                .Include(c => c.OilProducerCostProfile)
                .Include(c => c.GasProducerCostProfile)
                .Include(c => c.WaterInjectorCostProfile)
                .Include(c => c.GasInjectorCostProfile)
                .Include(c => c.WellProjectWells!).ThenInclude(wpw => wpw.DrillingSchedule)
                .Where(d => d.Project.Id.Equals(projectId));
        }

        return new List<WellProject>();
    }

    public WellProjectDto CopyWellProject(Guid wellProjectId, Guid sourceCaseId)
    {
        var source = GetWellProject(wellProjectId);
        var newWellProjectDto = WellProjectDtoAdapter.Convert(source);
        newWellProjectDto.Id = Guid.Empty;
        if (newWellProjectDto.OilProducerCostProfile != null)
        {
            newWellProjectDto.OilProducerCostProfile.Id = Guid.Empty;
        }
        if (newWellProjectDto.GasProducerCostProfile != null)
        {
            newWellProjectDto.GasProducerCostProfile.Id = Guid.Empty;
        }
        if (newWellProjectDto.WaterInjectorCostProfile != null)
        {
            newWellProjectDto.WaterInjectorCostProfile.Id = Guid.Empty;
        }
        if (newWellProjectDto.GasInjectorCostProfile != null)
        {
            newWellProjectDto.GasInjectorCostProfile.Id = Guid.Empty;
        }

        var wellProject = NewCreateWellProject(newWellProjectDto, sourceCaseId);
        var dto = WellProjectDtoAdapter.Convert(wellProject);

        return dto;
    }

    public async Task<ProjectDto> CreateWellProject(WellProject wellProject, Guid sourceCaseId)
    {
        var project = _projectService.GetProject(wellProject.ProjectId);
        wellProject.Project = project;
        _context.WellProjects!.Add(wellProject);
        await _context.SaveChangesAsync();
        SetCaseLink(wellProject, sourceCaseId, project);
        return _projectService.GetProjectDto(project.Id);
    }

    public async Task<WellProject> NewCreateWellProject(WellProjectDto wellProjectDto, Guid sourceCaseId)
    {
        var wellProject = WellProjectAdapter.Convert(wellProjectDto);
        var project = _projectService.GetProject(wellProject.ProjectId);
        wellProject.Project = project;
        var createdWellProject = _context.WellProjects!.Add(wellProject);
        await _context.SaveChangesAsync();
        SetCaseLink(wellProject, sourceCaseId, project);
        return createdWellProject.Entity;
    }

    private void SetCaseLink(WellProject wellProject, Guid sourceCaseId, Project project)
    {
        var case_ = project.Cases!.FirstOrDefault(o => o.Id == sourceCaseId);
        if (case_ == null)
        {
            throw new NotFoundInDBException($"Case {sourceCaseId} not found in database.");
        }

        case_.WellProjectLink = wellProject.Id;
        _context.SaveChanges();
    }

    public async Task<ProjectDto> DeleteWellProject(Guid wellProjectId)
    {
        _logger.LogWarning("An example of a Warning trace..");
        _logger.LogError("An example of an Error level message");

        var wellProject = await GetWellProject(wellProjectId);
        _context.WellProjects!.Remove(wellProject);
        DeleteCaseLinks(wellProjectId);
        await _context.SaveChangesAsync();
        return _projectService.GetProjectDto(wellProject.ProjectId);
    }

    private void DeleteCaseLinks(Guid wellProjectId)
    {
        foreach (var c in _context.Cases!)
        {
            if (c.WellProjectLink == wellProjectId)
            {
                c.WellProjectLink = Guid.Empty;
            }
        }
    }

    public ProjectDto UpdateWellProject(WellProjectDto updatedWellProject)
    {
        var existing = GetWellProject(updatedWellProject.Id).Result;
        WellProjectAdapter.ConvertExisting(existing, updatedWellProject);

        _context.WellProjects!.Update(existing);
        _context.SaveChanges();
        return _projectService.GetProjectDto(updatedWellProject.ProjectId);
    }

    public WellProjectDto NewUpdateWellProject(WellProjectDto updatedWellProjectDto)
    {
        var existing = GetWellProject(updatedWellProjectDto.Id).Result;
        WellProjectAdapter.ConvertExisting(existing, updatedWellProjectDto);

        var updatedWellProject = _context.WellProjects!.Update(existing);
        _context.SaveChanges();
        return WellProjectDtoAdapter.Convert(updatedWellProject.Entity);
    }

    public async Task<WellProject> GetWellProject(Guid wellProjectId)
    {
        var wellProject = await _context.WellProjects!
            .Include(c => c.OilProducerCostProfile)
            .Include(c => c.GasProducerCostProfile)
            .Include(c => c.WaterInjectorCostProfile)
            .Include(c => c.GasInjectorCostProfile)
            .Include(c => c.WellProjectWells!).ThenInclude(wpw => wpw.DrillingSchedule)
            .Include(c => c.WellProjectWells!).ThenInclude(wpw => wpw.Well)
            .FirstOrDefaultAsync(o => o.Id == wellProjectId);
        if (wellProject == null)
        {
            throw new ArgumentException(string.Format("Well project {0} not found.", wellProjectId));
        }
        return wellProject;
    }
}
