using JG.Code.Catalog.Application.UseCases.Video.DeleteVideo;
using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Application.Video.DeleteVideo;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture> { }

public class DeleteVideoTestFixture : VideoTestFixtureBase
{
    public DeleteVideoInput GetValidInput(Guid? id = null)
    {
        return new(id ?? Guid.NewGuid());
    }
}
