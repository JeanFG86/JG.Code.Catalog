using System.Text;
using JG.Code.Catalog.Application.UseCases.Video.CreateVideo;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>{}

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput CreateValidInput(List<Guid>? categoriesIds = null, List<Guid>? genresIds = null, List<Guid>? castMembersIds = null, FileInput? thumb = null, FileInput? banner = null, FileInput? thumbHalf = null, FileInput? media = null, FileInput? trailer = null) =>
        new(
            GetValidTitle(), 
            GetValidDescription(), 
            GetRandomBoolean(), 
            GetRandomBoolean(),
            GetValidYearLaunched(), 
            GetValidDuration(), 
            GetRandomRating(),
            categoriesIds,
            genresIds,
            castMembersIds,
            thumb,
            banner,
            thumbHalf,
            media,
            trailer
        );
    
    public CreateVideoInput CreateValidInputWithAllImages() =>
        new(
            GetValidTitle(), 
            GetValidDescription(), 
            GetRandomBoolean(), 
            GetRandomBoolean(),
            GetValidYearLaunched(), 
            GetValidDuration(), 
            GetRandomRating(),
            null,
            null,
            null,
            GetValidImageFileInput(),
            GetValidImageFileInput(),
            GetValidImageFileInput(),
            GetValidImageFileInput()
        );

    internal CreateVideoInput CreateValidInputWithAllMedias() =>
        new(
            GetValidTitle(),
            GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating(),
            null,
            null,
            null,
            null,
            null,
            null,
            GetValidMediaFileInput(),
            GetValidMediaFileInput()
        );
}