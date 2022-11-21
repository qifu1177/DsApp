namespace DS.Api.Extensions
{
    public static class StringExtension
    {
        public static string[] ToArray(this string str, string delimiter)
        {
            return str.Split(delimiter);
        }
    }
}
