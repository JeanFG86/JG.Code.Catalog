using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Extensions;
using JG.Code.Catalog.Domain.Repository;
using Moq;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UseCase.UpdateVideo _useCase;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new UseCase.UpdateVideo(
            _videoRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosBasicInfo()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Title == input.Title &&
            video.Description == input.Description &&
            video.YearLaunched == input.YearLaunched &&
            video.Duration == input.Duration &&
            video.Opened == input.Opened &&
            video.Published == input.Published &&
            video.Rating == input.Rating
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenVideoNotFound()
    {
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        _videoRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithCategoriesIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCategoriesIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var categoriesIds = _fixture.GetListOfGuids();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: categoriesIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoriesIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Categories.Count == categoriesIds.Count &&
            video.Categories.All(categoryId => categoriesIds.Contains(categoryId))
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Categories.Should().HaveCount(categoriesIds.Count);
        output.Categories.Select(c => c.Id).Should().BeEquivalentTo(categoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenCategoryIdNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenCategoryIdNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var categoriesIds = _fixture.GetListOfGuids();
        var removedCategoryId = categoriesIds[1];
        var persistenceIds = categoriesIds.FindAll(x => x != removedCategoryId);
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: categoriesIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {removedCategoryId}");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithGenresIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithGenresIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var genresIds = _fixture.GetListOfGuids();
        var input = _fixture.CreateValidInput(exampleVideo.Id, genresIds: genresIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(genresIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Genres.Count == genresIds.Count &&
            video.Genres.All(genreId => genresIds.Contains(genreId))
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Genres.Should().HaveCount(genresIds.Count);
        output.Genres.Select(g => g.Id).Should().BeEquivalentTo(genresIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenGenreIdNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenGenreIdNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var genresIds = _fixture.GetListOfGuids();
        var removedGenreId = genresIds[1];
        var persistenceIds = genresIds.FindAll(x => x != removedGenreId);
        var input = _fixture.CreateValidInput(exampleVideo.Id, genresIds: genresIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedGenreId}");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithCastMembersIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCastMembersIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var castMembersIds = _fixture.GetListOfGuids();
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: castMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMembersIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.CastMembers.Count == castMembersIds.Count &&
            video.CastMembers.All(castMemberId => castMembersIds.Contains(castMemberId))
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.CastMembers.Should().HaveCount(castMembersIds.Count);
        output.CastMembers.Select(c => c.Id).Should().BeEquivalentTo(castMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenCastMemberIdNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenCastMemberIdNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var castMembersIds = _fixture.GetListOfGuids();
        var removedCastMemberId = castMembersIds[1];
        var persistenceIds = castMembersIds.FindAll(x => x != removedCastMemberId);
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: castMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {removedCastMemberId}");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithAllRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithAllRelations()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var categoriesIds = _fixture.GetListOfGuids();
        var genresIds = _fixture.GetListOfGuids();
        var castMembersIds = _fixture.GetListOfGuids();
        var input = _fixture.CreateValidInput(
            exampleVideo.Id,
            categoriesIds: categoriesIds,
            genresIds: genresIds,
            castMembersIds: castMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoriesIds);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(genresIds);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMembersIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Categories.Count == categoriesIds.Count &&
            video.Genres.Count == genresIds.Count &&
            video.CastMembers.Count == castMembersIds.Count
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Categories.Should().HaveCount(categoriesIds.Count);
        output.Genres.Should().HaveCount(genresIds.Count);
        output.CastMembers.Should().HaveCount(castMembersIds.Count);
    }

    [Fact(DisplayName = nameof(UpdateVideoRemovingAllCategories))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoRemovingAllCategories()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: new List<Guid>());
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Categories.Count == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Categories.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateVideoRemovingAllGenres))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoRemovingAllGenres()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id, genresIds: new List<Guid>());
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Genres.Count == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Genres.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateVideoRemovingAllCastMembers))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoRemovingAllCastMembers()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: new List<Guid>());
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.CastMembers.Count == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateVideoRemovingAllRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoRemovingAllRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(
            exampleVideo.Id,
            categoriesIds: new List<Guid>(),
            genresIds: new List<Guid>(),
            castMembersIds: new List<Guid>());
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Categories.Count == 0 &&
            video.Genres.Count == 0 &&
            video.CastMembers.Count == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Categories.Should().BeEmpty();
        output.Genres.Should().BeEmpty();
        output.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateVideoKeepRelationsWhenNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoKeepRelationsWhenNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var originalCategoriesCount = exampleVideo.Categories.Count;
        var originalGenresCount = exampleVideo.Genres.Count;
        var originalCastMembersCount = exampleVideo.CastMembers.Count;
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Categories.Count == originalCategoriesCount &&
            video.Genres.Count == originalGenresCount &&
            video.CastMembers.Count == originalCastMembersCount
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenValidationFails))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenValidationFails()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateInvalidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("Could not update video");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenMultipleCategoryIdsNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenMultipleCategoryIdsNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var categoriesIds = _fixture.GetListOfGuids(5);
        var notFoundIds = new List<Guid> { categoriesIds[1], categoriesIds[3] };
        var persistenceIds = categoriesIds.FindAll(x => !notFoundIds.Contains(x));
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoriesIds: categoriesIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<RelatedAggregateException>();
        exception.Which.Message.Should().Contain(notFoundIds[0].ToString());
        exception.Which.Message.Should().Contain(notFoundIds[1].ToString());
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenMultipleGenreIdsNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenMultipleGenreIdsNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var genresIds = _fixture.GetListOfGuids(5);
        var notFoundIds = new List<Guid> { genresIds[0], genresIds[4] };
        var persistenceIds = genresIds.FindAll(x => !notFoundIds.Contains(x));
        var input = _fixture.CreateValidInput(exampleVideo.Id, genresIds: genresIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<RelatedAggregateException>();
        exception.Which.Message.Should().Contain(notFoundIds[0].ToString());
        exception.Which.Message.Should().Contain(notFoundIds[1].ToString());
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenMultipleCastMemberIdsNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenMultipleCastMemberIdsNotFound()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var castMembersIds = _fixture.GetListOfGuids(5);
        var notFoundIds = new List<Guid> { castMembersIds[2], castMembersIds[3] };
        var persistenceIds = castMembersIds.FindAll(x => !notFoundIds.Contains(x));
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMembersIds: castMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<RelatedAggregateException>();
        exception.Which.Message.Should().Contain(notFoundIds[0].ToString());
        exception.Which.Message.Should().Contain(notFoundIds[1].ToString());
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}