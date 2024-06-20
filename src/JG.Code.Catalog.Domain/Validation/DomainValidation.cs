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
            throw new EntityValidationException($"{fieldName} should not be empty or null");
    }

    public static void MinLength(string target, int minLength, string fieldName)
    {
        if(target.Length < minLength)
        {
            throw new EntityValidationException($"{fieldName} should be at least {minLength} characters long");
        }
    }

    public static void MaxLength(string target, int maxLength, string fieldName)
    {
        if (target.Length > maxLength)
        {
            throw new EntityValidationException($"{fieldName} should be less or equal {maxLength} characters");
        }
    }
}
