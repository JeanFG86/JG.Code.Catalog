using JG.Code.Catalog.UnitTests.Common;

namespace JG.Code.Catalog.UnitTests.Infra.Storage;

[CollectionDefinition(nameof(StorageServiceTestFixtureCollection))]
public class StorageServiceTestFixtureCollection : ICollectionFixture<StorageServiceTestFixture>
{
}

public class StorageServiceTestFixture : BaseFixture
{
    public string GetBucketName() => "fg-catalog-medias";
    public string GetFileName() => Faker.System.CommonFileName();
    public string GetContentFile() => Faker.Lorem.Paragraph();
    public string GetContentType() => Faker.System.MimeType();
}
