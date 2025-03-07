﻿using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Genre;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
namespace JG.Code.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest
{
    private readonly UpdateGenreApiTestFixture _fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var genreFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
    }
    
    [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task ProblemDetailsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/genres/{randomGuid}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Detail.Should().Be($"Genre {randomGuid} not found.");
        output!.Title.Should().Be("Not Found");
        output!.Type.Should().Be($"NotFound");
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
    }
    
    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task UpdateGenreWithRelations()
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
        int newRelationsCount = randon.Next(2, exampleCategories.Count -1);
        var newRelationsCategoriesIds = new List<Guid>();
        for (int i = 0; i < newRelationsCount; i++)
        {
            int selectedCategoryIndex = randon.Next(0, exampleCategories.Count - 1);
            DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
            if (newRelationsCategoriesIds.Contains(selected.Id))
                newRelationsCategoriesIds.Add(selected.Id);
        }
        await _fixture.Persistence.InsertList(exampleGenres);
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        await _fixture.Persistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(), newRelationsCategoriesIds);

        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoriesIdsFromOutput = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(newRelationsCategoriesIds);
        var genreFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var genresCategoriesFromDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelationsCategoriesIds);
    }
    
    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(), new List<Guid>{randomGuid});

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {randomGuid}");
    }
    
     [Fact(DisplayName = nameof(UpdateGenrePersistsRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task UpdateGenrePersistsRelationsWhenNotPresentInInput()
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
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoriesIdsFromOutput = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);
        var genreFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var genresCategoriesFromDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
    }
    
}