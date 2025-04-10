﻿using FluentAssertions;
using JG.Code.Catalog.Domain.Exceptions;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregates")]
    public void Instantiate()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        
        var video = new DomainEntity.Video(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration);

        video.Should().NotBeNull();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
    }
    
    [Fact(DisplayName = nameof(InstantiateThrowsExceptionWhenNotValid))]
    [Trait("Domain", "Video - Aggregates")]
    public void InstantiateThrowsExceptionWhenNotValid()
    {
        var expectedTitle = "";
        var expectedDescription = _fixture.GetTooLongDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        
        var action = () => new DomainEntity.Video(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration);

        action.Should().Throw<EntityValidationException>().WithMessage("Validation errors");

    }
}