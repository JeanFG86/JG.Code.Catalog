using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.SeedWork;

namespace JG.Code.Catalog.Domain.Entity;

public class CastMember : AggregateRoot
{
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CastMember(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
        CreatedAt = DateTime.Now;
    }
}