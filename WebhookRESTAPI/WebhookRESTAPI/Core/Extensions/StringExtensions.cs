namespace WebhookRESTAPI.Core.Extensions
{
    public static class StringExtensions
    {
        public static string StringJoin(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }
    }
}
