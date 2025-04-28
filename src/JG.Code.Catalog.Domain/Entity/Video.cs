using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.SeedWork;
using JG.Code.Catalog.Domain.Validation;
using JG.Code.Catalog.Domain.Validator;
using JG.Code.Catalog.Domain.ValueObject;

namespace JG.Code.Catalog.Domain.Entity;

public class Video: AggregateRoot
{
   
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public Rating Rating { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Video(string title, string description, int yearLaunched, bool opened, bool published, int duration, Rating rating)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        Rating = rating;
        CreatedAt = DateTime.Now;
    }

    public void Validate(ValidationHandler handler)
        => (new VideoValidator(this, handler)).Validate();

    public void Update(string title, string description, int yearLaunched, bool opened, bool published, int duration)
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Opened = opened;
        Published = published;
        Duration = duration;
    }

    public void UpdateThumb(string path) => Thumb = new Image(path);
    public void UpdateThumbHalf(string path) => ThumbHalf = new Image(path);
    
}