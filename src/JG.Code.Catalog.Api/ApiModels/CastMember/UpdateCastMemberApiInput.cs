using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.Api.ApiModels.CastMember;

public class UpdateCastMemberApiInput
{
    public string Name { get; set; }
    public CastMemberType Type { get; private set; }

    public UpdateCastMemberApiInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }
}