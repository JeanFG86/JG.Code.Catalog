using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.GetCategory;
public class GetCategoryInput : IRequest<GetCategoryOutput>
{
    public Guid Id { get; set; }

    public GetCategoryInput(Guid id) => Id = id;    
}
