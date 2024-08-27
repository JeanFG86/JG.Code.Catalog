using Newtonsoft.Json.Serialization;

namespace JG.Code.Catalog.Api.Extensions.String;

public static class StringSnakeCaseExtension
{
    private readonly static NamingStrategy _snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return _snakeCaseNamingStrategy.GetPropertyName(value, false);
    }
}
