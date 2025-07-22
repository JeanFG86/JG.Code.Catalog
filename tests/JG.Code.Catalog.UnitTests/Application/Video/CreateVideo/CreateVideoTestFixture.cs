using System.Text;
using JG.Code.Catalog.Application.UseCases.Video.CreateVideo;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>{}

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput CreateValidVideoInput(List<Guid>? categoriesIds = null, List<Guid>? genresIds = null, List<Guid>? castMembersIds = null, FileInput? thumb = null, FileInput? banner = null, FileInput? thumbHalf = null) =>
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
            thumbHalf
        );
}