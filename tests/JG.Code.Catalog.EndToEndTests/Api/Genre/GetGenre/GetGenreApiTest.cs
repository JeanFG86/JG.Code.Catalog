﻿using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.GetGenre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest
{
    private readonly GetGenreApiTestFixture _fixture;

    public GetGenreApiTest(GetGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task GetGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
    }
    
    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task NotFound()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre {randomGuid} not found.");
    }
    
    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task GetGenreWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5]; 
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
        
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        List<Guid> relatedCategoriesIds = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
    }
}