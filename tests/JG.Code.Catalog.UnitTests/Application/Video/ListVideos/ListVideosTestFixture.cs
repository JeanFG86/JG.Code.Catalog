using JG.Code.Catalog.UnitTests.Common.Fixtures;
using Entities = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixtureCollection))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{
}

public class ListVideosTestFixture : VideoTestFixtureBase
{
    public List<Entities.Video> CreateExampleVideosList() => Enumerable.Range(1, Random.Shared.Next(10, 20)).Select(_ => GetValidVideoWithAllProperties()).ToList();

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = "";
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public Entities.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Entities.Video> CreateExampleVideosListWithoutRelations() =>
        Enumerable.Range(1, Random.Shared.Next(10, 20)).Select(_ => GetValidVideo()).ToList();


    public (List<Entities.Video> Videos, List<Entities.Category> Categories, List<Entities.Genre> Genres) CreateExampleVideosListWithRelations()
    {
        var quantityToBeCreated = Random.Shared.Next(2, 10);
        List<Entities.Category> categories = new List<Entities.Category>();
        List<Entities.Genre> genres = new List<Entities.Genre>();
        var videos = Enumerable.Range(1, quantityToBeCreated)
            .Select(_ => GetValidVideoWithAllProperties()).ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var categoriesqtd = Random.Shared.Next(2, 5);
            for (int i = 0; i < categoriesqtd; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var genresqtd = Random.Shared.Next(2, 5);
            for (int i = 0; i < genresqtd; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }
        });

        return (videos, categories, genres);
    }

    private string GetValidGenreName() =>
       Faker.Commerce.Categories(1)[0];

    private Entities.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new Entities.Genre(name ?? GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }
}
