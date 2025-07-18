using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Enum;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public record CreateVideoInput(string Title,
    string Description,
    bool Opened,
    bool Published, 
    int YearLaunched, 
    int Duration,
    Rating Rating,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null) : IRequest<CreateVideoOutput>
{
   
}