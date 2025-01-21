using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Enum;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;

public class CreateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public CreateCastMemberInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
}