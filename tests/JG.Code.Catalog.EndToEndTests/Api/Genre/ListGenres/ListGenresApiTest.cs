﻿using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.EndToEndTests.Extensions.DateTime;
using JG.Code.Catalog.EndToEndTests.Models;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.ListGenres;

[Collection(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTest : IDisposable
{
    private readonly ListGenresApiTestFixture _fixture;

    public ListGenresApiTest(ListGenresApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task List()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleGenres.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(exampleGenres.Count);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
    
    [Fact(DisplayName = nameof(ListWithRelations))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(15);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random randon = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = randon.Next(2, exampleCategories.Count -1);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = randon.Next(0, exampleCategories.Count - 1);
                DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
                if (genre.Categories.Contains(selected.Id))
                    genre.AddCategory(selected.Id);
            }
        });
        List<GenresCategories> genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(genre => genre.Categories.ToList().ForEach(categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))));
        await _fixture.Persistence.InsertList(exampleGenres);
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        await _fixture.Persistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleGenres.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(exampleGenres.Count);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            var relatedCategoriesIds = outputItem.Categories.Select(x => x.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outPutRelatedCategory =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == outPutRelatedCategory.Id);
                exampleCategory.Should().NotBeNull();
                outPutRelatedCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
    
    [Fact(DisplayName = nameof(EmptyWhenThereAreNoItems))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task EmptyWhenThereAreNoItems()
    {
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = 15;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(0);
        output.Data!.Count.Should().Be(0);
    }
    
    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated( int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems)
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(quantityToGenerate);
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput();
        input.Page = page;
        input.PerPage = perPage;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

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
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
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
        var exampleGenres = _fixture.GetExampleListGenresByNames(new List<string>
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
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput();
        input.Page = page;
        input.PerPage = perPage;
        input.Search = search;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

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
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
    
    [Theory(DisplayName = nameof(Ordered))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
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
        var exampleGenres = _fixture.GetExampleListGenres(10);
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = 10;
        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        input.Dir = orderEnum;
        input.Sort = orderBy;
        
        var (reponse, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(10);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(10);
        var expectedOrderList = _fixture.CloneGenresListOrdered(exampleGenres, orderBy, orderEnum);
        for (int i = 0; i < expectedOrderList.Count; i++)
        {
            var outputItem = output.Data[i];
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        };
    }

    public void Dispose() => _fixture.CleanPersistence();
}