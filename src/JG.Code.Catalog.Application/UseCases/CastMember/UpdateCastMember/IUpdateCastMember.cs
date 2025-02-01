using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public interface IUpdateCastMember: IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
    
}