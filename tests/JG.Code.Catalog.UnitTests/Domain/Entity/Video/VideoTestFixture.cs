﻿using JG.Code.Catalog.UnitTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>{}

public class VideoTestFixture : BaseFixture
{
    public DomainEntity.Video GetValidVideo()
    {
        return new DomainEntity.Video(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), GetRandomBoolean(), GetRandomBoolean(), GetValidDuration());
    }

    public string GetValidTitle() => Faker.Lorem.Letter(100);

    public string GetValidDescription() => Faker.Commerce.ProductDescription();

    public int GetValidYearLaunched() => Faker.Random.Int(1950, 2100);

    public int GetValidDuration() => Faker.Random.Int(100, 300);

    public string GetTooLongTitle() => Faker.Lorem.Letter(400);
    
    public string GetTooLongDescription() => Faker.Lorem.Letter(4001);
}