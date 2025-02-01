using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public class UpdateCastMember: IUpdateCastMember
{
    private readonly ICastMemberRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCastMember(ICastMemberRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _categoryRepository.Get(request.Id, cancellationToken);
        castMember.Update(request.Name, request.Type);
        await _categoryRepository.Update(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}