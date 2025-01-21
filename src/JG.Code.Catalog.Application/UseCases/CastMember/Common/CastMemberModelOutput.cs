using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.Application.UseCases.CastMember.Common;

public class CastMemberModelOutput
{
    public CastMemberModelOutput(Guid id, string name, CastMemberType type, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Type = type;
        CreatedAt = createdAt;
    }
    
    public Guid Id { get; private set; }
    public String Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
}