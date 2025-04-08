﻿using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Domain.Validator;

public class VideoValidator : Validation.Validator
{
    private readonly Video _video;
    private const int TitleMaxLength = 255;
    
    public VideoValidator(Video video, ValidationHandler handler) : base(handler)
    {
        _video = video;
    }

    public override void Validate()
    {
        if(string.IsNullOrWhiteSpace(_video.Title))
            _handler.HandleError($"'{nameof(_video.Title)}' is required.");
        if(_video.Title.Length > 255)
            _handler.HandleError($"'{nameof(_video.Title)}' should be less or equal {TitleMaxLength} characters long.");
    }
}