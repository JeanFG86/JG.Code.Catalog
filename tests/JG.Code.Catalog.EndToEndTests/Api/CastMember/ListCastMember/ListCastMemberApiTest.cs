using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.EndToEndTests.Extensions.DateTime;
using JG.Code.Catalog.EndToEndTests.Models;
using Microsoft.AspNetCore.Http;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.ListCastMember;

[Collection(nameof(ListCastMemberApiTestFixture))]
public class ListCastMemberApiTest: IDisposable
{
    private readonly ListCastMemberApiTestFixture _fixture;

    public ListCastMemberApiTest(ListCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/API", "CastMember/ListGenres - Endpoints")]
    public async Task List()
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        var targetGenre = examples[5];
        await _fixture.Persistence.InsertList(examples);

        var input = new ListCastMembersInput();
        input.Page = 1;
        input.PerPage = examples.Count;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(examples.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(examples.Count);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
    
    [Fact(DisplayName = nameof(EmptyWhenThereAreNoItems))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task EmptyWhenThereAreNoItems()
    {
        var input = new ListCastMembersInput();
        input.Page = 1;
        input.PerPage = 15;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(0);
        output.Data!.Count.Should().Be(0);
    }
    
    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "CastMember/ListGenres - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated( int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems)
    {
        var examples = _fixture.GetExampleCastMembersList(quantityToGenerate);
        await _fixture.Persistence.InsertList(examples);

        var input = new ListCastMembersInput();
        input.Page = page;
        input.PerPage = perPage;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(quantityToGenerate);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(expectedQuantityItems);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
    
    
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "CastMember/ListGenres - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("Comedy", 2, 3, 0, 0)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var examples = _fixture.GetExampleCastMembersListWithNames(new List<string>
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based onReal Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });
        await _fixture.Persistence.InsertList(examples);
        var input = new ListCastMembersInput();
        input.Page = page;
        input.PerPage = perPage;
        input.Search = search;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(expectedQuantityItemsReturned);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
    
    [Theory(DisplayName = nameof(Ordered))]
    [Trait("EndToEnd/API", "CastMember/ListGenres - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdat", "asc")]
    [InlineData("createdat", "desc")]
    [InlineData("", "asc")]
    public async Task Ordered(
        string orderBy,
        string order
    )
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        await _fixture.Persistence.InsertList(examples);
        var input = new ListCastMembersInput();
        input.Page = 1;
        input.PerPage = 10;
        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        input.Dir = orderEnum;
        input.Sort = orderBy;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(10);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(10);
        var expectedOrderList = _fixture.CloneCastMembersListOrdered(examples, orderBy, orderEnum);
        for (int i = 0; i < expectedOrderList.Count; i++)
        {
            var outputItem = output.Data[i];
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        };
    }
    
    public void Dispose() => _fixture.CleanPersistence();
}