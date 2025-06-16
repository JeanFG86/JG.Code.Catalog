using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideo(IUnitOfWork unitOfWork,IVideoRepository videoRepository, ICategoryRepository categoryRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateVideoOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
    {

        var video = new Domain.Entity.Video(input.Title, input.Description, input.YearLaunched, input.Opened, input.Published, input.Duration, input.Rating);
        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);
        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await _categoryRepository.GetIdsListByIds(input.CategoriesIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < input.CategoriesIds!.Count)
            {
                var notFoundIds = input.CategoriesIds!.ToList().FindAll(categoryId => !persistenceIds.Contains(categoryId));
                throw new RelatedAggregateException($"Related category id (or ids) not found: {string.Join(',', notFoundIds)}");
            }
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);  
        }
        
        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return CreateVideoOutput.FromVideo(video);
    }
}