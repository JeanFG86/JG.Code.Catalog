using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Domain.Validator;

public class VideoValidator : Validation.Validator
{
    private readonly Video _video;
    
    public VideoValidator(Video video, ValidationHandler handler) : base(handler)
    {
        _video = video;
    }

    public override void Validate()
    {
       
    }
}