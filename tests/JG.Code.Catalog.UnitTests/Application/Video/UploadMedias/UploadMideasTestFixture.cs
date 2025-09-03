using JG.Code.Catalog.UnitTests.Common.Fixtures;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UploadMideas;

namespace JG.Code.Catalog.UnitTests.Application.Video.UploadMideas;

[CollectionDefinition(nameof(UploadMideasTestFixture))]
public class UploadMideasTestFixtureFixtureCollection : ICollectionFixture<UploadMideasTestFixture>{}

public class UploadMideasTestFixture : VideoTestFixtureBase
{
    public UseCase.UploadMediasInput GetValidInput(Guid? videoId = null) =>
        new (videoId ?? Guid.NewGuid(), GetValidMediaFileInput(), GetValidMediaFileInput());
}