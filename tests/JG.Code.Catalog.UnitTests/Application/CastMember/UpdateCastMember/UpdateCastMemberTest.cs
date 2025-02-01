using FluentAssertions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
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
}