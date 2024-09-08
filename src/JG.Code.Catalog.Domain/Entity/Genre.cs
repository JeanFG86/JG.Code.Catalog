﻿using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Domain.Entity;
public class Genre
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<Guid> Categories { get; set; }

    public Genre(string name, bool isActive = true)
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        Categories = new List<Guid>();
        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void Update(string name)
    {
        Name = name;
        Validate();
    }

    public void AddCategory(Guid categoryId)
    {
        Categories.Add(categoryId);
        Validate();
    }

    private void Validate() => DomainValidation.NotNullOrEmpty(Name, nameof(Name));
}
