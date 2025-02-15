using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

public class ListCastMembersOutput: PaginatedListOutput<CastMemberModelOutput>
{
    public ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items) 
        : base(page, perPage, total, items)
    {
    }

    public static ListCastMembersOutput FromSearchOutput(SearchOutput<DomainEntity.CastMember> searchOutput)
    {
        return new(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(CastMemberModelOutput.FromCastMember).ToList()
        );
    }
}
