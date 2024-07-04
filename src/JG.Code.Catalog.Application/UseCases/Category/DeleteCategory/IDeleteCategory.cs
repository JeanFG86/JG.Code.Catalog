using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.DeleteCategory;
public interface IDeleteCategory : IRequestHandler<DeleteCategoryInput>
{
}
