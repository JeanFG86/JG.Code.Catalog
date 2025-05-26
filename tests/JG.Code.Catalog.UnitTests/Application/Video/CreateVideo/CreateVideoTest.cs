using JG.Code.Catalog.Application.Interfaces;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;
    
    public CreateVideoTest(CreateVideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object);
        var input = new UseCase.CreateVideoInput(
            _fixture.GetValidTitle(), 
            _fixture.GetValidDescription(), 
            _fixture.GetRandomBoolean(), 
            _fixture.GetRandomBoolean(),
            _fixture.GetValidYearLaunched(), 
            _fixture.GetValidDuration(), 
            _fixture.GetRandomRating()
            );
        
        var output =await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(video 
            => video.Id != Guid.Empty &&
               video.Title == input.Title &&
               video.Published == input.Published &&
               video.Description == input.Description &&
               video.YearLaunched == input.YearLaunched
               ), It.IsAny<CancellationToken>()));
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
    }
    
    [Fact(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoThrowsWithInvalidInput()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object);
        var input = new UseCase.CreateVideoInput(
            "", 
            _fixture.GetValidDescription(), 
            _fixture.GetRandomBoolean(), 
            _fixture.GetRandomBoolean(),
            _fixture.GetValidYearLaunched(), 
            _fixture.GetValidDuration(), 
            _fixture.GetRandomRating()
        );
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>().WithMessage($"There are validation errors");
        repositoryMock.Verify(
            x => x.Insert(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        //return Task.CompletedTask;
    }
}