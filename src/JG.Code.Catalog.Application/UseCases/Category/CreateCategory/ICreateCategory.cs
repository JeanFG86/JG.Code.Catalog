using JG.Code.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
public interface ICreateCategory : IRequestHandler<CreateCategoryInput, CategoryModelOutput>
{
}
