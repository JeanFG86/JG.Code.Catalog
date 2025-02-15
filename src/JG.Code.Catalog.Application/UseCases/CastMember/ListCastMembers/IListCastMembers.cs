using MediatR;

namespace JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;

public interface IListCastMembers: IRequestHandler<ListCastMembersInput, ListCastMembersOutput>
{
    
}