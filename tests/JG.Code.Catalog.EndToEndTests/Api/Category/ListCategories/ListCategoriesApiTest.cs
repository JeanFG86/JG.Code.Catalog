﻿using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.Application.UseCases.Category.ListCategories;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.EndToEndTests.Extensions.DateTime;
using JG.Code.Catalog.EndToEndTests.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit.Abstractions;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.ListCategories;

//class CategoryListOutput
//{
//    public CategoryListOutput(Meta meta, IReadOnlyList<CategoryModelOutput> data)
//    {
//        Meta = meta;
//        Data = data;
//    }

//    public Meta Meta { get; set; }
//    public IReadOnlyList<CategoryModelOutput> Data { get; set; }    
//}

public class Meta
{
    public Meta(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
    }

    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
}

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }    

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);        

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Meta.CurrentPage.Should().Be(1);
        output!.Meta.PerPage.Should().Be(defaultPerPage);
        output!.Data.Should().HaveCount(defaultPerPage);
        foreach (CategoryModelOutput outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }        
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(0);
        output.Data!.Should().HaveCount(0);        
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);
        
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Data.Should().HaveCount(input.PerPage);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        foreach (CategoryModelOutput outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
       int quantityCategoriesToGenerate,
       int page,
       int perPage,
       int expectedQuantityItems
        )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: page, perPage: perPage);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Data.Should().HaveCount(expectedQuantityItems);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        foreach (CategoryModelOutput outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(SerarchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("Comedy", 2, 3, 0, 0)]
    public async Task SerarchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
        )
    {
        var categoriesNamesList = new List<string>{
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based onReal Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        };
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(categoriesNamesList);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: page, perPage: perPage, search: search);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output!.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        foreach (CategoryModelOutput outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
        )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, sort: orderBy, dir: searchOrder);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Data.Should().HaveCount(exampleCategoriesList.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(exampleCategoriesList, input.Sort, input.Dir);
        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputIem = output.Data![indice];
            var exampleItem = expectedOrderedList[indice];
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Id.Should().Be(exampleItem!.Id);
            outputIem.Description.Should().Be(exampleItem.Description);
            outputIem.IsActive.Should().Be(exampleItem.IsActive);
            outputIem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(ListOrderedDates))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]    
    [InlineData("createdat", "asc")]
    [InlineData("createdat", "desc")]
    public async Task ListOrderedDates(
        string orderBy,
        string order
        )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, sort: orderBy, dir: searchOrder);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Data.Should().HaveCount(exampleCategoriesList.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        DateTime? lastItemDate = null;
        foreach (CategoryModelOutput outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
            if (lastItemDate != null)
            {
                if (order == "asc")
                    Assert.True(outputItem.CreatedAt >= lastItemDate);
                else
                    Assert.True(outputItem.CreatedAt <= lastItemDate);

            }
            lastItemDate = outputItem.CreatedAt;
        }
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
