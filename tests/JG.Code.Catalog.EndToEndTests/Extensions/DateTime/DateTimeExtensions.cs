namespace JG.Code.Catalog.EndToEndTests.Extensions.DateTime;
internal static class DateTimeExtensions
{
    public static System.DateTime TrimMillisseconds(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
    }
}
