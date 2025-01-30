using FluentAssertions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UsesCases = JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Create))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Create()
    {
        var input = new CreateCastMemberInput(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UsesCases.CreateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        repositoryMock.Verify(repository => repository.Insert(It.Is<DomainEntity.CastMember>(x => x.Name == input.Name), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ThrowsWhenInvalidName(string? name)
    {
        var input = new CreateCastMemberInput(name, _fixture.GetRandomCastMemberType());
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UsesCases.CreateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be empty or null");
    }
}