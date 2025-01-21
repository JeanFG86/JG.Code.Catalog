using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;

public interface ICreateCastMember : IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
{
    
}