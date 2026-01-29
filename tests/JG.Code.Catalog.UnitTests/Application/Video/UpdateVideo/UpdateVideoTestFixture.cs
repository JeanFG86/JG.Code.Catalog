using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.UnitTests.Common.Fixtures;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{
}

public class UpdateVideoTestFixture : VideoTestFixtureBase
{
    public UseCase.UpdateVideoInput CreateValidInput(
        Guid videoId,
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null,
        FileInput? media = null,
        FileInput? trailer = null) => new(
        VideoId: videoId,
        Title: GetValidTitle(),
        Description: GetValidDescription(),
        YearLaunched: GetValidYearLaunched(),
        Duration: GetValidDuration(),
        Opened: GetRandomBoolean(),
        Published: GetRandomBoolean(),
        Rating: GetRandomRating(),
        CategoriesIds: categoriesIds,
        GenresIds: genresIds,
        CastMembersIds: castMembersIds,
        Thumb: thumb,
        Banner: banner,
        ThumbHalf: thumbHalf,
        Media: media,
        Trailer: trailer
    );

    public UseCase.UpdateVideoInput CreateInvalidInput(Guid videoId) => new(
        VideoId: videoId,
        Title: GetTooLongTitle(),
        Description: GetTooLongDescription(),
        YearLaunched: GetValidYearLaunched(),
        Duration: GetValidDuration(),
        Opened: GetRandomBoolean(),
        Published: GetRandomBoolean(),
        Rating: GetRandomRating(),
        CategoriesIds: null,
        GenresIds: null,
        CastMembersIds: null
    );

    public UseCase.UpdateVideoInput CreateValidInputWithAllImages(Guid videoId) => new(
        VideoId: videoId,
        Title: GetValidTitle(),
        Description: GetValidDescription(),
        YearLaunched: GetValidYearLaunched(),
        Duration: GetValidDuration(),
        Opened: GetRandomBoolean(),
        Published: GetRandomBoolean(),
        Rating: GetRandomRating(),
        CategoriesIds: null,
        GenresIds: null,
        CastMembersIds: null,
        Thumb: GetValidImageFileInput(),
        Banner: GetValidImageFileInput(),
        ThumbHalf: GetValidImageFileInput()
    );

    public UseCase.UpdateVideoInput CreateValidInputWithAllMedias(Guid videoId) => new(
        VideoId: videoId,
        Title: GetValidTitle(),
        Description: GetValidDescription(),
        YearLaunched: GetValidYearLaunched(),
        Duration: GetValidDuration(),
        Opened: GetRandomBoolean(),
        Published: GetRandomBoolean(),
        Rating: GetRandomRating(),
        CategoriesIds: null,
        GenresIds: null,
        CastMembersIds: null,
        Media: GetValidMediaFileInput(),
        Trailer: GetValidMediaFileInput()
    );

    public List<Guid> GetListOfGuids(int? count = null)
    {
        var random = new Random();
        var quantity = count ?? random.Next(2, 5);
        return Enumerable.Range(1, quantity)
            .Select(_ => Guid.NewGuid())
            .ToList();
    }
}