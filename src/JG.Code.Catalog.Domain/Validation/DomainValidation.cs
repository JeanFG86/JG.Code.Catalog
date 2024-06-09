using JG.Code.Catalog.Domain.Exceptions;

namespace JG.Code.Catalog.Domain.Validation;
public class DomainValidation
{
    public static void NotNull(object? target, string fieldName)
    {
        if (target is null)
            throw new EntityValidationException($"{fieldName} should not be null");
    }
    public static void NotNullOrEmpty(object? target, string fieldName)
    {
        if (String.IsNullOrWhiteSpace(target as string))
            throw new EntityValidationException($"{fieldName} should not be null or empty");
    }
}
