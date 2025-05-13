using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.Exceptions;
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
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private List<Guid> _categories;
    public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();
    
    private List<Guid> _genres;
    public IReadOnlyList<Guid> Genres => _genres.AsReadOnly();

    public Video(string title, string description, int yearLaunched, bool opened, bool published, int duration, Rating rating)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        Rating = rating;
        
        _categories = [];
        _genres = [];
        
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
    public void UpdateBanner(string path) => Banner = new Image(path);
    public void UpdateMedia(string validPath) => Media = new Media(validPath);
    public void UpdateTrailer(string validPath) => Trailer = new Media(validPath);

    public void UpdateAsSendToEncode()
    {
        if(Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string validEncodedPath)
    {
        if(Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateAsEncoded(validEncodedPath);
    }

    public void AddCategory(Guid categoryId) => _categories.Add(categoryId);

    public void RemoveCategory(Guid categoryIdExample) => _categories.Remove(categoryIdExample);

    public void RemoveAllCategories() => _categories = new();
    
    public void AddGenre(Guid genreId) => _genres.Add(genreId);
    
    public void RemoveGenre(Guid genreId) => _genres.Remove(genreId);
    
    public void RemoveAllGenres() => _genres = new();
}