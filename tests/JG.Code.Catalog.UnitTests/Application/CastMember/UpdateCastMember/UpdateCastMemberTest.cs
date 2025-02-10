using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory(DisplayName = nameof(UpdateCastMember))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    [MemberData(
        nameof(UpdateCastMemberDataGenerator.GetCastMemberToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCastMemberDataGenerator)
    )]
    public async Task UpdateCastMember(DomainEntity.CastMember exampleCastMember, UseCase.UpdateCastMemberInput input)
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        CastMemberModelOutput output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(exampleCastMember, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }
    
    [Fact(DisplayName = nameof(ThrowWhenCastMemberNotFound))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenCastMemberNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var input = _fixture.GetValidInput();
        repositoryMock.Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"castmember '{input.Id}' not found"));
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);


        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(repository => repository.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<DomainEntity.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact(DisplayName = nameof(ThrowWhenInvalidName))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenInvalidName()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(castMemberExample);
        var input = new UseCase.UpdateCastMemberInput(Guid.NewGuid(), null!, _fixture.GetRandomCastMemberType());
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
        
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be empty or null");
        repositoryMock.Verify(repository => repository.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<DomainEntity.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}