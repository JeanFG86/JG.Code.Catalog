namespace JG.Code.Catalog.UnitTests.Domain.Entity.Category;

using JG.Code.Catalog.Domain.Exceptions;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggegates")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description);

        var datetimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > datetimeBefore);
        Assert.True(category.CreatedAt < datetimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggegates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);

        var datetimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > datetimeBefore);
        Assert.True(category.CreatedAt < datetimeAfter);
        Assert.Equal(isActive, category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggegates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "Category Description");

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsnull))]
    [Trait("Domain", "Category - Aggegates")]
    public void InstantiateErrorWhenDescriptionIsnull()
    {
        Action action = () => new DomainEntity.Category("Category Name", null!);

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should not be null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLess3Characters))]
    [Trait("Domain", "Category - Aggegates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void InstantiateErrorWhenNameIsLess3Characters(string invalidName)
    {
        Action action = () => new DomainEntity.Category(invalidName, "Category Ok Description");

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be at least 3 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggegates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidName, "Category Ok Description");

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be less or equal 255 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggegates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category("Category Name", invalidDescription);

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should be less or equal 10_000 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggegates")]
    public void Activate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggegates")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        category.Deactivate();

        Assert.False(category.IsActive);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggegates")]
    public void Update()
    {
        var category = new DomainEntity.Category("Category name", "Category description");
        var newValues = new { Name = "New Name", Description = "New Description" };

        category.Update(newValues.Name, newValues.Description);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(newValues.Description, category.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggegates")]
    public void UpdateOnlyName()
    {
        var category = new DomainEntity.Category("Category name", "Category description");
        var newValues = new { Name = "New Name"};
        var currentDescription = category.Description;

        category.Update(newValues.Name);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(currentDescription, category.Description);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggegates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = new DomainEntity.Category("Category name", "Category description");
        Action action = () => category.Update(name!);

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }
}
