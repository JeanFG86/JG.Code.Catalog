using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;

public interface IGetCastMember : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{
    
}