using JG.Code.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.GetCategory;
public interface IGetCategory : IRequestHandler<GetCategoryInput, CategoryModelOutput>
{
}
