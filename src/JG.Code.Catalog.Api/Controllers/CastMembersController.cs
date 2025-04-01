using JG.Code.Catalog.Api.ApiModels.CastMember;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;
using JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;
using JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;
using JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
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
    
    
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCastMemberInput input, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(input, cancellationToken);
        return CreatedAtAction(nameof(Create), new { output.Id }, new ApiResponse<CastMemberModelOutput>(output));
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromBody] UpdateCastMemberApiInput apiInput, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new UpdateCastMemberInput(id, apiInput.Name, apiInput.Type);
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }
    
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
    
    [HttpGet]
    [ProducesResponseType(typeof(CastMemberModelOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null)
    {
        var input = new ListCastMembersInput();
        if (page != null)
            input.Page = page.Value;

        if (perPage != null)
            input.PerPage = perPage.Value;

        if (!string.IsNullOrWhiteSpace(search))
            input.Search = search;

        if (!string.IsNullOrWhiteSpace(sort))
            input.Sort = sort;

        if (dir != null)
            input.Dir = dir.Value;
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponseList<CastMemberModelOutput>(output));
    }
}