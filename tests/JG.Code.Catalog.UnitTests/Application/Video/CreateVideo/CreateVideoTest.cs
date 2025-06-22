using JG.Code.Catalog.Application.Interfaces;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
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
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>());
        var input = _fixture.CreateValidVideoInput();
        
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
    
    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(UseCase.CreateVideoInput input, string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>());
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should()
            .ThrowAsync<EntityValidationException>().WithMessage("There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should().Be(expectedValidationError);
        repositoryMock.Verify(
            x => x.Insert(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCategoriesIds()
    {
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategoriesIds);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, categoryRepositoryMock.Object, Mock.Of<IGenreRepository>());
        
        var input = _fixture.CreateValidVideoInput(exampleCategoriesIds);
        
        var output =await useCase.Handle(input, CancellationToken.None);
        
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
        output.CategoriesIds.Should().BeEquivalentTo(exampleCategoriesIds);
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(video 
            => video.Id != Guid.Empty &&
               video.Title == input.Title &&
               video.Published == input.Published &&
               video.Description == input.Description &&
               video.YearLaunched == input.YearLaunched &&
               video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
        ), It.IsAny<CancellationToken>()));
        categoryRepositoryMock.VerifyAll();
    }
    
    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedCategoryId = exampleCategoriesIds[2];
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, videoRepositoryMock.Object, categoryRepositoryMock.Object, Mock.Of<IGenreRepository>());
        
        var input = _fixture.CreateValidVideoInput(exampleCategoriesIds);
        
        var action =  () => useCase.Handle(input, CancellationToken.None);
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage(
            $"Related category id (or ids) not found: {removedCategoryId}");
    }
    
     [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithGenresIds()
    {
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        //genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenresIds);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object);
        
        var input = _fixture.CreateValidVideoInput(genresIds: exampleGenresIds);
        
        var output =await useCase.Handle(input, CancellationToken.None);
        
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
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEquivalentTo(exampleGenresIds);
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(video 
            => video.Id != Guid.Empty &&
               video.Title == input.Title &&
               video.Published == input.Published &&
               video.Description == input.Description &&
               video.YearLaunched == input.YearLaunched &&
               video.Genres.All(id => exampleGenresIds.Contains(id))
        ), It.IsAny<CancellationToken>()));
        genreRepositoryMock.VerifyAll();
    }
}