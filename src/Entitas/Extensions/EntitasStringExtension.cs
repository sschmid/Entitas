namespace Entitas
{
    public static class EntitasStringExtension
    {
        public static string RemoveSuffix(this string str, string suffix)
        {
            return str.EndsWith(suffix, System.StringComparison.Ordinal)
                ? str.Substring(0, str.Length - suffix.Length)
                : str;
        }
    }
}
