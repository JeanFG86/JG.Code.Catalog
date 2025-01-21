using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Repository;


namespace JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;

public class CreateCastMember : ICreateCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    {
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new Domain.Entity.CastMember(request.Name, request.Type);
        await _castMemberRepository.Insert(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return new CastMemberModelOutput(castMember.Id, castMember.Name, castMember.Type, castMember.CreatedAt);
    }
}