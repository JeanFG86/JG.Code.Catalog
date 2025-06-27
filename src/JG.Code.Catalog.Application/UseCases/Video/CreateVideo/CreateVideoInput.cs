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
    IReadOnlyCollection<Guid>? CastMembersIds = null) : IRequest<CreateVideoOutput>
{
   
}