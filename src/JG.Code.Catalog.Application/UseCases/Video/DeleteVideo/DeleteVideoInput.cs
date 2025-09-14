using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.DeleteVideo;

public record DeleteVideoInput(Guid Id) : IRequest; 
