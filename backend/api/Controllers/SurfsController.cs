using api.Dtos;
using api.Services;

using Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[RequiresApplicationRoles(
    ApplicationRole.Admin,
    ApplicationRole.ReadOnly,
    ApplicationRole.User
)]
public class SurfsController : ControllerBase
{
    private readonly SurfService _surfService;


    public SurfsController(SurfService surfService)
    {
        _surfService = surfService;
    }

    [HttpPut(Name = "UpdateSurf")]
    public ProjectDto UpdateSurf([FromBody] SurfDto surfDto)
    {
        return _surfService.UpdateSurf(surfDto);
    }

    [HttpPut("new", Name = "NewUpdateSurf")]
    public SurfDto NewUpdateSurf([FromBody] SurfDto surfDto)
    {
        return _surfService.NewUpdateSurf(surfDto);
    }

    [HttpPost(Name = "CreateSurf")]
    public async Task<ProjectDto> CreateSurf([FromQuery] Guid sourceCaseId, [FromBody] SurfDto surfDto)
    {
        return await _surfService.CreateSurf(surfDto, sourceCaseId);
    }

    [HttpPost("{surfId}/copy", Name = "CopySurf")]
    public SurfDto CopySurf([FromQuery] Guid caseId, Guid surfId)
    {
        return _surfService.CopySurf(surfId, caseId);
    }

    [HttpDelete("{surfId}", Name = "DeleteSurf")]
    public async Task<ProjectDto> DeleteSurf(Guid surfId)
    {
        return await _surfService.DeleteSurf(surfId);
    }
}
