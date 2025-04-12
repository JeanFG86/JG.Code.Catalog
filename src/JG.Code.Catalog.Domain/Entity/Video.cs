using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.SeedWork;
using JG.Code.Catalog.Domain.Validation;
using JG.Code.Catalog.Domain.Validator;

namespace JG.Code.Catalog.Domain.Entity;

public class Video: AggregateRoot
{
   
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Video(string title, string description, int yearLaunched, bool opened, bool published, int duration)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        CreatedAt = DateTime.Now;
    }
}