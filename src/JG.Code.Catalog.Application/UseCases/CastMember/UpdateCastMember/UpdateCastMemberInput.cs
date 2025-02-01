using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Enum;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public class UpdateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CastMemberType Type { get; private set; }

    public UpdateCastMemberInput(Guid id, string name, CastMemberType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}