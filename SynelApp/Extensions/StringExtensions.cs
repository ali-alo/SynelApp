using System.Globalization;

namespace SynelApp.Extensions
{
    public static class StringExtensions
    {
        // custom extension to convert string form csv to a DateTime type
        public static DateTime ToDateTime(this string value)
        {
            DateTime.TryParseExact(value, "d/M/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime result);
            return result;  // will return DateOnly.MinValue if conversion is impossible
        }
    }
}
