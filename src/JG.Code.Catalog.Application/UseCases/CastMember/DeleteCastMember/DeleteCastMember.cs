using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;

public class DeleteCastMember : IDeleteCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
        => (_castMemberRepository, _unitOfWork) = (castMemberRepository, unitOfWork);
    
    public async Task Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _castMemberRepository.Get(request.Id, cancellationToken);
        await _castMemberRepository.Delete(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}