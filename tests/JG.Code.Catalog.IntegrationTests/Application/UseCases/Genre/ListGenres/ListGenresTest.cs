using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
using JG.Code.Catalog.Infra.Data.EF.Models;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.ListGenres;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenres()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new UseCase.ListGenres(new GenreRepository(_fixture.CreateDbContext(true)), new CategoryRepository(actDbContext));
        ListGenresInput input = new ListGenresInput(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        });
    }

    [Fact(DisplayName = nameof(ListGenresReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenresReturnsEmptyWhenPersistenceIsEmpty()
    {
        var actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new UseCase.ListGenres(new GenreRepository(_fixture.CreateDbContext()), new CategoryRepository(actDbContext));
        ListGenresInput input = new ListGenresInput(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListGenresVerifyRelations))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async Task ListGenresVerifyRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random randon = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = randon.Next(0, 3);
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
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new UseCase.ListGenres(new GenreRepository(_fixture.CreateDbContext(true)), new CategoryRepository(actDbContext));
        ListGenresInput input = new ListGenresInput(1, 20);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(ListGenresPaginated))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListGenresPaginated(
       int quantityToGenerate,
       int page,
       int perPage,
       int expectedQuantityItems
   )
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(quantityToGenerate);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random randon = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = randon.Next(0, 3);
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
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new UseCase.ListGenres(new GenreRepository(_fixture.CreateDbContext(true)), new CategoryRepository(actDbContext));
        ListGenresInput input = new ListGenresInput(page, perPage);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
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
        var exampleGenres = _fixture.GetExampleListGenresByNames(new List<string>{
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
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        Random randon = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = randon.Next(0, 3);
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
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new UseCase.ListGenres(new GenreRepository(_fixture.CreateDbContext(true)), new CategoryRepository(actDbContext));
        ListGenresInput input = new ListGenresInput(page, perPage, search);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id);
            outputItem.Name.Should().Contain(search);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            List<Guid> outputItemCategoryIds = outputItem.Categories.Select(x => x.Id).ToList();
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
}
