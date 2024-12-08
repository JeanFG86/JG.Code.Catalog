using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTest
{
    private readonly DeleteGenreApiTestFixture _fixture;

    public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task DeleteGenre()
    {
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        await _fixture.Persistence.InsertList(exampleGenresList);
        var exampleGenre = exampleGenresList[5];

        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{exampleGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var pessistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        pessistenceGenre.Should().BeNull();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task WhenNotFound404()
    {
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenresList);

        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        output.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre {randomGuid} not found.");
    }
    
    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task DeleteGenreWithRelations()
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

        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var pessistenceGenre = await _fixture.Persistence.GetById(targetGenre.Id);
        pessistenceGenre.Should().BeNull();
        var relations = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relations.Should().HaveCount(0);
    }
}