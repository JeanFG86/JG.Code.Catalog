using JG.Code.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
public interface IUpdateCategory : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
{
}
