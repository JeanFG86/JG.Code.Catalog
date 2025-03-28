using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JG.Code.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CastMembersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public CastMembersController(IMediator mediator)
        => _mediator = mediator;
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new GetCastMemberInput(id), cancellationToken);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(CastMemberModelOutput), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCastMemberInput(id), cancellationToken);
        return NoContent();
    }
}