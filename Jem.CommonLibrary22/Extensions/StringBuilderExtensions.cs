namespace Jem.CommonLibrary22;

public static class StringBuilderExtensions
{
    public static void AppendCommaSeparated(this StringBuilder sb, string[] names, string prefix, string suffix)
    {
        var first = true;
        foreach (var name in names)
        {
            if (name.IsNullOrWhiteSpace()) continue;

            if (first) first = false; else sb.Append(", ");
            if (prefix != null) sb.Append(prefix);
            sb.Append(name);
            if (suffix != null) sb.Append(suffix);
        }
    }
}