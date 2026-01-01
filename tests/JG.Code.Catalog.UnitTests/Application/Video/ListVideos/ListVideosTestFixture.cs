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

    public (List<Entities.Video> Videos, List<Entities.Category> Categories) CreateExampleVideosListWithRelations()
    {
        var quantityToBeCreated = Random.Shared.Next(2, 10);
        List<Entities.Category> categories = new List<Entities.Category>();
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
        });

        return (videos, categories);
    }
}
