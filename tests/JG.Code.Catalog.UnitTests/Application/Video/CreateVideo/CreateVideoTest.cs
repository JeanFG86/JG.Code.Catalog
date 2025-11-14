using JG.Code.Catalog.Application.Interfaces;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.CreateVideo;
using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Domain.Extensions;

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
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput();
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
    }
    
    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumb()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        
        var expectedThumbName = $"thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInput(thumb: _fixture.GetValidImageFileInput());
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.ThumbFileUrl.Should().Be(expectedThumbName);
    }
    
    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithBanner()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        
        var expectedBannerName = $"banner.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedBannerName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInput(banner: _fixture.GetValidImageFileInput());
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }
    
    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumbHalf()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        
        var expectedThumbHalfName = $"thumbhalf.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInput(thumbHalf: _fixture.GetValidImageFileInput());
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithMedia()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedMediaName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInput(media: _fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.VideoFileUrl.Should().Be(expectedMediaName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithTrailer()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedTrailerName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedTrailerName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInput(trailer: _fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithAllImages()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        
        var expectedBannerName = $"banner.jpg";
        var expectedThumbName = $"thumb.jpg";
        var expectedThumbHalfName = $"thumbhalf.jpg";
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedBannerName);
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedThumbName);
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInputWithAllImages();
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(video 
            => video.Id != Guid.Empty &&
               video.Title == input.Title &&
               video.Published == input.Published &&
               video.Description == input.Description &&
               video.YearLaunched == input.YearLaunched
        ), It.IsAny<CancellationToken>()));
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.BannerFileUrl.Should().Be(expectedBannerName);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }
    
    [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionInUploadErrorCases()
    {
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new UseCase.CreateVideo(Mock.Of<IUnitOfWork>(), Mock.Of<IVideoRepository>(), Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInputWithAllImages();
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);
        
        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong in upload");
    }
    
    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackInImagesUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionAndRollbackInImagesUploadErrorCases()
    {
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync("123-banner.jpg");
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync("123-thumb.jpg");
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new UseCase.CreateVideo(Mock.Of<IUnitOfWork>(), Mock.Of<IVideoRepository>(), Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);
        var input = _fixture.CreateValidInputWithAllImages();
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);
        
        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong in upload");
        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => (x =="123-banner.jpg") || (x =="123-thumb.jpg")), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackInMediaUploadCommitErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionAndRollbackInMediaUploadCommitErrorCases()
    {
        var input = _fixture.CreateValidInputWithAllMedias();
        var storageServiceMock = new Mock<IStorageService>();
        var storageMediaPath = _fixture.GetValidMediaPath();
        var storageTrailerPath = _fixture.GetValidMediaPath();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storagePathList = new List<string>() { storageMediaPath, storageTrailerPath };
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith($"media.{input.Media!.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(storageMediaPath);
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith($"trailer.{input.Trailer!.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(storageTrailerPath);
        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Something went wrong in the commit"));
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, Mock.Of<IVideoRepository>(), Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), storageServiceMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong in the commit");
        storageServiceMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => storagePathList.Contains(x)), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(UseCase.CreateVideoInput input, string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        
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
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, categoryRepositoryMock.Object, Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput(exampleCategoriesIds);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
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
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, videoRepositoryMock.Object, categoryRepositoryMock.Object, Mock.Of<IGenreRepository>(), Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        
        var input = _fixture.CreateValidInput(exampleCategoriesIds);
        
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
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenresIds);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput(genresIds: exampleGenresIds);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
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
    
    [Fact(DisplayName = nameof(ThrowsWhenInvalidGenreId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenInvalidGenreId()
    {
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleGenresIds[2];
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenresIds.FindAll(id => id != removedId));;
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, Mock.Of<ICastMemberRepository>(), Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput(genresIds: exampleGenresIds);
        
        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedId}");
    }
    
    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCastMembersIds()
    {
        var exampleCastMembersIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var repositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCastMembersIds);
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), castMemberRepositoryMock.Object, Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput(castMembersIds: exampleCastMembersIds);
        
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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEquivalentTo(exampleCastMembersIds);
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(video 
            => video.Id != Guid.Empty &&
               video.Title == input.Title &&
               video.Published == input.Published &&
               video.Description == input.Description &&
               video.YearLaunched == input.YearLaunched &&
               video.CastMembers.All(id => exampleCastMembersIds.Contains(id))
        ), It.IsAny<CancellationToken>()));
        castMemberRepositoryMock.VerifyAll();
    }
    
    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenInvalidCastMemberId()
    {
        var exampleCastMembersIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleCastMembersIds[2];
        var repositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCastMembersIds.FindAll(id => id != removedId));
        var useCase = new UseCase.CreateVideo(unitOfWorkMock.Object, repositoryMock.Object, Mock.Of<ICategoryRepository>(), Mock.Of<IGenreRepository>(), castMemberRepositoryMock.Object, Mock.Of<IStorageService>());
        var input = _fixture.CreateValidInput(castMembersIds: exampleCastMembersIds);;
        
        var action = () => useCase.Handle(input, CancellationToken.None);
       
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {removedId}");
    }
}