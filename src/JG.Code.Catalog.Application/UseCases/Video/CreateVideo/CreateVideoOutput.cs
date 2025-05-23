using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public record CreateVideoOutput(Guid Id, DateTime CreatedAt, string Title, bool Published, string Description, Rating Rating, int YearLaunched, int Duration, bool Opened);