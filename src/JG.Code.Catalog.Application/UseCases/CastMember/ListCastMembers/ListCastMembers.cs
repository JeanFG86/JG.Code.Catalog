using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;

public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _castMemberRepository;

    public ListCastMembers(ICastMemberRepository castMemberRepository)
    {
        _castMemberRepository = castMemberRepository;
    }
    
    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput input, CancellationToken cancellationToken)
    {
        var searchOutput = await _castMemberRepository.Search(input.ToSearchInput(), cancellationToken);
        return ListCastMembersOutput.FromSearchOutput(searchOutput);
    }
}