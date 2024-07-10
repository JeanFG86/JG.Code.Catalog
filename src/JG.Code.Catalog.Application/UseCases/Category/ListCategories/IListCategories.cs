using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.ListCategories;
public interface IListCategories : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{
}
