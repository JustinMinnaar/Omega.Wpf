namespace Jem.CommonLibrary22;

public static class IEnumerableExtensions
{
    public static string ToStringOfNamesQuoted<T>(this IEnumerable<T> items) where T : class, IName
        => ToStringOfNames(items, "'", "'");

    public static string ToStringOfNames<T>(this IEnumerable<T> items, string? prefix = null, string? suffix = null) where T : class, IName
    {
        var first = true;
        var names = new StringBuilder();
        foreach (var item in items)
        {
            if (first) first = false; else names.Append(", ");

            names.Append(prefix + item.Name + suffix);
        }
        return names.ToString();
    }
}
