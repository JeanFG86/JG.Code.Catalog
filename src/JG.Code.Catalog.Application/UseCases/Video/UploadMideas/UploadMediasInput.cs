using JG.Code.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.UploadMideas;

public record UploadMediasInput(Guid VideoId, FileInput? VideoFile, FileInput? TrailerFile) : IRequest;