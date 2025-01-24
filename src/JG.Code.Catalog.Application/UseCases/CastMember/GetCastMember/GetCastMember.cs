using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;

public class GetCastMember : IGetCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;

    public GetCastMember(ICastMemberRepository castMemberRepository)
    {
        _castMemberRepository = castMemberRepository;
    }

    public async Task<CastMemberModelOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _castMemberRepository.Get(request.Id, cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}