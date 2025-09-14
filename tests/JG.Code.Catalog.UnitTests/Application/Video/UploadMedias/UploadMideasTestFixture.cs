using JG.Code.Catalog.UnitTests.Common.Fixtures;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UploadMideas;

namespace JG.Code.Catalog.UnitTests.Application.Video.UploadMideas;

//[CollectionDefinition(nameof(UploadMideasTestFixture))]
//public class UploadMideasTestFixtureCollection : ICollectionFixture<UploadMideasTestFixture>{}

public class UploadMideasTestFixture : VideoTestFixtureBase
{
    public UseCase.UploadMediasInput GetValidInput(Guid? videoId = null, bool withVideoFile = true, bool withTrailerFile = true) =>
        new (videoId ?? Guid.NewGuid(), withVideoFile ? GetValidMediaFileInput() : null, withTrailerFile ? GetValidMediaFileInput() : null);
}