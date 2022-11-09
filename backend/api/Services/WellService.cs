using api.Adapters;
using api.Context;
using api.Dtos;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class WellService
{
    private readonly DcdDbContext _context;
    private readonly ProjectService _projectService;
    private readonly IServiceProvider _serviceProvider;
    private readonly WellProjectService _wellProjectService;
    private readonly ExplorationService _explorationService;
    private readonly ILogger<CaseService> _logger;

    public WellService(DcdDbContext context, ProjectService projectService, WellProjectService wellProjectService, ExplorationService explorationService, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _context = context;
        _projectService = projectService;
        _logger = loggerFactory.CreateLogger<CaseService>();
        _serviceProvider = serviceProvider;
        _wellProjectService = wellProjectService;
        _explorationService = explorationService;
    }

    public async Task<ProjectDto> CreateWell(WellDto wellDto)
    {
        var well = WellAdapter.Convert(wellDto);
        _context.Wells!.Add(well);
        await _context.SaveChangesAsync();
        return _projectService.GetProjectDto(wellDto.ProjectId);
    }

    public ProjectDto UpdateWell(WellDto updatedWellDto)
    {
        var existing = GetWell(updatedWellDto.Id).Result;
        WellAdapter.ConvertExisting(existing, updatedWellDto);

        _context.Wells!.Update(existing);
        _context.SaveChanges();
        return _projectService.GetProjectDto(existing.ProjectId);
    }

    public WellDto UpdateExistingWell(WellDto updatedWellDto)
    {
        var existing = GetWell(updatedWellDto.Id).Result;
        WellAdapter.ConvertExisting(existing, updatedWellDto);

        var well = _context.Wells!.Update(existing);
        return WellDtoAdapter.Convert(well.Entity);
    }

    public WellDto[] UpdateMultipleWells(WellDto[] updatedWellDtos)
    {
        var updatedWellDtoList = new List<WellDto>();
        foreach (var wellDto in updatedWellDtos)
        {
            var updatedWellDto = UpdateExistingWell(wellDto);
            updatedWellDtoList.Add(updatedWellDto);
        }

        _context.SaveChanges();

        var costProfileHelper = _serviceProvider.GetRequiredService<CostProfileFromDrillingScheduleHelper>();
        costProfileHelper.UpdateCostProfilesForWells(updatedWellDtos.Select(w => w.Id).ToList());

        return updatedWellDtoList.ToArray();
    }

    public WellDto[]? CreateMultipleWells(WellDto[] wellDtos)
    {
        ProjectDto? projectDto = null;
        foreach (var wellDto in wellDtos)
        {
            projectDto = CreateWell(wellDto).Result;
        }

        return projectDto?.Wells?.ToArray();
    }

    public async Task<Well> GetWell(Guid wellId)
    {
        var well = await _context.Wells!
            .Include(e => e.WellProjectWells)
            .Include(e => e.ExplorationWells)
            .FirstOrDefaultAsync(w => w.Id == wellId);
        if (well == null)
        {
            throw new ArgumentException($"Well {wellId} not found");
        }

        return well;
    }

    public WellDto GetWellDto(Guid wellId)
    {
        var well = GetWell(wellId).Result;
        var wellDto = WellDtoAdapter.Convert(well);

        return wellDto;
    }

    public IEnumerable<Well> GetAll()
    {
        if (_context.Wells != null)
        {
            return _context.Wells;
        }

        _logger.LogInformation("No Wells existing");
        return new List<Well>();
    }

    public IEnumerable<WellDto> GetDtosForProject(Guid projectId)
    {
        var wells = GetWells(projectId).Result;
        var wellsDtos = new List<WellDto>();
        foreach (var well in wells)
        {
            wellsDtos.Add(WellDtoAdapter.Convert(well));
        }

        return wellsDtos;
    }

    public async Task<IEnumerable<Well>> GetWells(Guid projectId)
    {
        if (_context.Wells != null)
        {
            return await _context.Wells
                .Where(d => d.ProjectId.Equals(projectId)).ToListAsync();
        }

        return new List<Well>();
    }

    public IEnumerable<WellDto> GetAllDtos()
    {
        if (GetAll().Any())
        {
            var wells = GetAll();
            var wellDtos = new List<WellDto>();
            foreach (var well in wells)
            {
                var wellDto = WellDtoAdapter.Convert(well);
                wellDtos.Add(wellDto);
            }

            return wellDtos;
        }

        return new List<WellDto>();
    }
}
