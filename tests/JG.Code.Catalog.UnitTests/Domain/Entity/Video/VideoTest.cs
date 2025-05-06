using FluentAssertions;
using FluentValidation.Results;
using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Validation;
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
        var expectedRating = Rating.ER;
        
        var video = new DomainEntity.Video(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration, expectedRating);

        video.Should().NotBeNull();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }
    
    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWhenValidState()
    {
        var validVideo = _fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();

        validVideo.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeFalse();
    }
    
    [Fact(DisplayName = nameof(ValidateWithErrorWhenInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWithErrorWhenInvalidState()
    {
        var invalidVideo = new DomainEntity.Video(_fixture.GetTooLongTitle(), _fixture.GetTooLongDescription(), _fixture.GetValidYearLaunched(), 
            _fixture.GetRandomBoolean(), _fixture.GetRandomBoolean(), _fixture.GetValidDuration(), _fixture.GetRandomRating());
        var notificationValidationHandler = new NotificationValidationHandler();

        invalidVideo.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new ValidationError("'Title' should be less or equal 255 characters long."),
            new ValidationError("'Description' should be less or equal 4000 characters long."),
        });
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Video - Aggregates")]
    public void Update()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var video = _fixture.GetValidVideo();
        
        video.Update(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration);

        video.Should().NotBeNull();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
    }
    
    [Fact(DisplayName = nameof(ValidateStillValidatingAfterUpdateToValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateStillValidatingAfterUpdateToValidState()
    {
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var video = _fixture.GetValidVideo();
        
        video.Update(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration);
        var notificationValidationHandler = new NotificationValidationHandler();
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeFalse();
    }
    
    [Fact(DisplayName = nameof(ValidateGenerateErrorsAfterUpdateToInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateGenerateErrorsAfterUpdateToInvalidState()
    {
        var expectedTitle = _fixture.GetTooLongTitle();
        var expectedDescription = _fixture.GetTooLongDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedDuration = _fixture.GetValidDuration();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var video = _fixture.GetValidVideo();
        
        video.Update(expectedTitle, expectedDescription, expectedYearLaunched, expectedOpened, expectedPublished, expectedDuration);
        var notificationValidationHandler = new NotificationValidationHandler();
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.Errors.Should().HaveCount(2);
        notificationValidationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
        {
            new ("'Title' should be less or equal 255 characters long."),
            new ("'Description' should be less or equal 4000 characters long."),
        });
    }
    
    [Fact(DisplayName = nameof(UpdateThumb))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumb()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();
        
        validVideo.UpdateThumb(validImagePath);
        
        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb!.Path.Should().Be(validImagePath);
    }
    
    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumbHalf()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();
        
        validVideo.UpdateThumbHalf(validImagePath);
        
        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf!.Path.Should().Be(validImagePath);
    }
    
    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateBanner()
    {
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();
        
        validVideo.UpdateBanner(validImagePath);
        
        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner!.Path.Should().Be(validImagePath);
    }
    
    [Fact(DisplayName = nameof(UpdateMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateMedia()
    {
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();
        
        validVideo.UpdateMedia(validPath);
        
        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.FilePath.Should().Be(validPath);
    }
    
    [Fact(DisplayName = nameof(UpdateTrailer))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateTrailer()
    {
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();
        
        validVideo.UpdateTrailer(validPath);
        
        validVideo.Trailer.Should().NotBeNull();
        validVideo.Trailer!.FilePath.Should().Be(validPath);
    }
}