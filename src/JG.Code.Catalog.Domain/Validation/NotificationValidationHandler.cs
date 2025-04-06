namespace JG.Code.Catalog.Domain.Validation;

public class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> _errors;
    
    public NotificationValidationHandler()
    {
        _errors = new();
    }
    
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();
    
    public bool HasErrors() => _errors.Any();

    public override void HandleError(ValidationError error)
        => _errors.Add(error);
}