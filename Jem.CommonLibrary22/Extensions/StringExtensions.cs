using System.Globalization;
using System.Text.RegularExpressions;

namespace Jem.CommonLibrary22;

public static class StringExtensions
{
    #region Bracketed

    /// <summary>Given "value" returns "(value)". Given "value1, value2, ..." returns "(value1), (value2), ...". Calls Enclosed.</summary>
    public static string? BracketedRound(this string? value, char separator = ',') { return Bracketed(value, "(", ")", separator); }

    /// <summary>Given "value" returns "(value)". Given "value1, value2, ..." returns "(value1), (value2), ...". Calls Enclosed.</summary>
    public static string? BracketedSquare(this string? value, char separator = ',') { return Bracketed(value, "[", "]", separator); }

    /// <summary>Encloses one or more values, comma-separated by default, each with prefix and suffix.</summary>
    public static string? Bracketed(this string? value, string? prefix, string? suffix, char separator = ',')
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        if (value.Contains(separator))
        {
            var result = string.Empty;
            var parts = value.SplitTrim(separator);
            foreach (var part in parts)
            {
                result = result.Comma(prefix + part + suffix);
            }
            return result;
        }

        return string.Concat(prefix, value, suffix);
    }

    #endregion Bracketed

    #region Clean

    public static string? Clean(this string? s)
    {
        if (s == null) return s;

        var sb = new StringBuilder();
        var inWhitespace = true;
        foreach (var c in s)
        {
            if (char.IsWhiteSpace(c)) inWhitespace = true;
            else
            {
                if (sb.Length > 0 && inWhitespace) sb.Append(' ');
                sb.Append(c);
                inWhitespace = false;
            }
        }
        return sb.ToString();
    }

    #endregion Clean

    #region Comma/Dot/Line/Pipe/Space...

    /// <summary>Returns source and following with a comma between them if both are not null or whitespace,
    /// such that "a".Dot("") or "".Dot("a") returns "a" and "a".Dot("b") returns "a.b". Useful for building
    /// lists of names by writing names = names.Comma(name);(</summary>
    public static string? Comma(this string? source, string? following)
    {
        if (string.IsNullOrWhiteSpace(source)) return following;
        if (string.IsNullOrWhiteSpace(following)) return source;
        return source + ", " + following;
    }

    /// <summary>Returns source and following with a dot between them if both are not null and not
    /// whitespace, such that "a".Dot("") or "".Dot("a") returns "a" and "a".Dot("b") returns "a.b".
    /// Useful for building keys such as "TableName" or "ScopeName.TableName" by writing
    /// key = parentName.Dot(Name).</summary>
    public static string? Dot(this string? source, string? following, string separator = ".")
    {
        if (string.IsNullOrWhiteSpace(source)) return following;
        if (string.IsNullOrWhiteSpace(following)) return source;
        return source + separator + following;
    }

    /// <summary>If the suffix is not empty, returns the string and suffix, separated by a dot.</summary>
    public static string? Line(this string? source, string? suffix, string separator = "_")
    {
        if (string.IsNullOrWhiteSpace(source)) return suffix;
        if (!string.IsNullOrWhiteSpace(suffix)) return source + separator + suffix;
        return source;
    }

    /// <summary>If the suffix is not empty, returns the string and suffix, separated by a pipe.</summary>
    public static string? Pipe(this string? source, string? suffix, string separator = "|")
    {
        if (string.IsNullOrWhiteSpace(source)) return suffix;
        if (!string.IsNullOrWhiteSpace(suffix)) return source + separator + suffix;
        return source;
    }

    /// <summary>If the suffix is not empty, returns the string and suffix, separated by a space.</summary>
    public static string? Space(this string? source, string? suffix)
    {
        if (string.IsNullOrWhiteSpace(source)) return suffix;
        if (string.IsNullOrWhiteSpace(suffix)) return source;
        return source + " " + suffix;
    }

    #endregion Comma/Dot/Line/Pipe/Space...

    #region ContainsAnyOf

    public static bool ContainsAnyOf(this string? source, string validCharacters)
    {
        if (source == null) return false;
        foreach (var c in source)
        {
            if (validCharacters.Contains(c)) return true;
        }
        return false;
    }

    #endregion ContainsAnyOf

    #region ContainsAnyString

    public static bool ContainsAnyString(this string? source, params string[] validStrings)
    {
        if (source == null) return false;
        foreach (var s in validStrings)
        {
            if (source.Contains(s)) return true;
        }
        return false;
    }

    #endregion ContainsAnyString

    #region ContainsText

    public static bool ContainsText(this string? a, string? b, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        if (a == null) return (b == null);
        if (b == null) return true;
        var pos = a.IndexOf(b, comparisonType);
        return pos >= 0;
    }

    #endregion ContainsText

    #region // ContainsByName

    ///// <summary>Returns true if there is an item matching on name.</summary>
    //public static bool ContainsByName<T>(this IEnumerable<T> list, string name) where T : IName
    //{
    //    foreach (var item in list)
    //    {
    //        if (item.Name == name) return true;
    //    }
    //    return false;
    //}

    ///// <summary>Returns true if there is an item matching on any of names.</summary>
    //public static bool ContainsByName<T>(this IEnumerable<T> list, params string[] names) where T : IName
    //{
    //    foreach (var item in list)
    //    {
    //        if (item.Name.In(names)) return true;
    //    }
    //    return false;
    //}

    #endregion // ContainsByName

    #region Equals

    public static bool Equals(this string? a, string? b,
        StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        if (a == null) return (b == null);
        if (b == null) return false;
        return (string.Compare(a, b, comparisonType) == 0);
    }

    #endregion Equals

    #region GetKeyedValues

    public static IDictionary<string, string> GetKeyedValues(this string values, char separator = '|', char equals = '=')
    {
        var results = new Dictionary<string, string>();

        var kvs = values.Split(separator);
        if (kvs != null)
            foreach (var kv in kvs)
            {
                kv.SplitKey(out string? key, out string? value, seperator: equals);
                if (key != null && value != null)
                    results.Add(key, value);
            }

        return results;
    }

    #endregion GetKeyedValues

    #region In

    public static bool In(this string name, params string[] names)
    {
        return (names.Contains(name));
    }

    #endregion In

    #region IsEmail

    private const string validEmailPattern =
        @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

    private static Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

    public static bool IsEmail(this string emailAddress)
    {
        return emailAddress != null && ValidEmailRegex.IsMatch(emailAddress);
    }

    #endregion IsEmail

    #region IsIdentifier

    public static bool IsNotIdentifier(this string value,
        bool allowUnderscores = false, bool allowSpaces = false, bool allowDashes = false, int maxLength = 50)
    {
        return !IsIdentifier(value, allowUnderscores, allowSpaces, allowDashes, maxLength);
    }

    public static bool IsIdentifier(this string value,
        bool allowUnderscores = false, bool allowSpaces = false, bool allowDashes = false, int maxLength = 50)
    {
        if (value.IsNullOrWhiteSpace() || value.Length > maxLength) return false;

        bool firstChar = true;
        bool lastCharWasSpace = false;

        foreach (var c in value)
        {
            var isLetter = char.IsLetter(c);
            var isUnderscore = (allowUnderscores && c == '_');
            if (firstChar)
            {
                // Identifier may begin with letter or underscore
                if (!isLetter && !isUnderscore) return false;
            }
            else
            {
                var isDigit = char.IsDigit(c);
                var isSpace = (allowSpaces && c == ' ');
                var isDash = (allowDashes && c == '-');

                // Identifier may contain letters, digits, and if permitted, spaces, underscores and dashes
                if (!isLetter && !isDigit && !isUnderscore && !isSpace && !isDash)
                    return false;
            }

            firstChar = false;
            lastCharWasSpace = (c == ' ');
        }
        return !lastCharWasSpace;
    }

    #endregion IsIdentifier

    #region IsNullOrEmpty/Whitespace

    /// <summary>Returns true if s is null or "". Use IsNullOrWhitespace unless your need this functionality.</summary>
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    /// <summary>Returns false if s is null or "". Use IsNotNullOrWhitespace unless your need this functionality.</summary>
    public static bool IsNotNullOrEmpty(this string s)
    {
        return !string.IsNullOrEmpty(s);
    }

    /// <summary>Returns true if s is null or contains only whitespace.</summary>
    public static bool IsNullOrWhiteSpace(this string s)
    {
        return string.IsNullOrWhiteSpace(s);
    }

    /// <summary>Returns false if s is null or contains only whitespace.</summary>
    public static bool IsNotNullOrWhiteSpace(this string s)
    {
        return !string.IsNullOrWhiteSpace(s);
    }

    public static string NoSpaces(this string s)
    {
        return s.Replace(" ", "");
    }
    
    #endregion IsNullOrEmpty/Whitespace

    #region Join

    public static string Join(this IEnumerable<string> strings, string separator = "|")
    {
        var sb = new StringBuilder();
        Join(strings, separator, sb);
        return sb.ToString();
    }

    public static void Join(this IEnumerable<string> strings, string separator, StringBuilder sb)
    {
        foreach (var s in strings)
        {
            if (sb.Length > 0) sb.Append(separator);
            sb.Append(s);
        }
    }

    #endregion Join

    #region LengthExcludingWhitespace

    public static int LengthExcludingWhitespace(this string s)
    {
        var length = 0;
        if (s != null)
            foreach (var c in s)
            {
                if (char.IsWhiteSpace(c)) continue;
                length++;
            }
        return length;
    }

    #endregion LengthExcludingWhitespace

    #region MatchWildcardString

    /// <summary>Fast * and ? pattern matching for strings, up to 3K in length.</summary>
    /// <param name="input"></param>
    /// <param name="pattern"></param>
    /// <remarks>https://www.codeproject.com/Tips/57304/Use-wildcard-characters-and-to-compare-strings</remarks>
    /// <returns></returns>
    public static Boolean MatchWildcardString(this String input, String pattern)
    {
        if (String.Compare(pattern, input) == 0) return true;

        if (String.IsNullOrEmpty(input))
        {
            if (String.IsNullOrEmpty(pattern.Trim(new Char[1] { '*' })))
                return true;
            else
                return false;
        }

        if (input.Length > 3000)
            throw new InvalidOperationException("Length cannot exceed 3000 characters!");

        if (pattern.Length == 0) return false;

        if (pattern[0] == '?')
            return MatchWildcardString(pattern.Substring(1), input.Substring(1));

        if (pattern[pattern.Length - 1] == '?')
            return MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input.Substring(0, input.Length - 1));

        if (pattern[0] == '*')
        {
            if (MatchWildcardString(pattern.Substring(1), input))
                return true;
            else
                return MatchWildcardString(pattern, input.Substring(1));
        }

        if (pattern[pattern.Length - 1] == '*')
        {
            if (MatchWildcardString(pattern.Substring(0, pattern.Length - 1), input))
                return true;
            else
                return MatchWildcardString(pattern, input.Substring(0, input.Length - 1));
        }

        if (pattern[0] == input[0])
            return MatchWildcardString(pattern.Substring(1), input.Substring(1));

        return false;
    }

    public static Boolean MatchWildcardStringLong(String pattern, String input)
    {
        String regexPattern = "^";

        foreach (Char c in pattern)
        {
            switch (c)
            {
                case '*':
                    regexPattern += ".*";
                    break;

                case '?':
                    regexPattern += ".";
                    break;

                default:
                    regexPattern += "[" + c + "]";
                    break;
            }
        }
        return new Regex(regexPattern + "$").IsMatch(input);
    }

    #endregion MatchWildcardString

    #region MaxCharacters

    public static string? MaxCharacters(this string source, int max, string follow = "...")
    {
        if (follow == null) throw new ArgumentNullException(nameof(follow));
        var len = follow.Length;
        if (max < len) max = len;

        if (source == null || source.Length <= max - len) return source;

        return source.Substring(0, max - len) + follow;
    }

    #endregion MaxCharacters

    #region Quoted...

    /// <summary>Returns the string enclosed in double quotes, unless it contains double quotes,
    /// in which case it returns the string enclosed in single quotes. If it contains both single
    /// and double quotes, it changes double quote inside the string into two double quotes and
    /// encloses it in double quotes.</summary>
    public static string? Quoted(this string? source)
    {
        if (source != null)
        {
            if (source.Contains('\"'))
            {
                source = source.Replace("\'", "\"");
                return QuotedSingle(source);
            }
            if (source.Contains("'"))
            {
                source = source.Replace("'", "\"");
            }
        }
        return QuotedDouble(source);
    }

    /// <summary>Returns the string enclosed in single quotes. Changes each single quote inside the string into two single quotes.</summary>
    public static string? QuotedSingle(this string? source)
    {
        if (source != null) source = source.Replace("'", "''");
        return string.Concat("'", source, "'");
    }

    /// <summary>Returns the string enclosed in double quotes. Changes each double quote inside the string into two double quotes.</summary>
    public static string? QuotedDouble(this string? value)
    {
        if (value != null) value = value.Replace("\"", "\"\"");
        return string.Concat("\"", value, "\"");
    }

    /// <summary>Returns the string enclosed in double quotes. Changes each double quote inside the string into two double quotes.
    /// Also changes \ to \\ to support C# escape sequencing in code.</summary>
    public static string? QuotedDoubleCode(this string? value)
    {
        if (value != null) value = value.Replace("\\", "\\\\");
        return QuotedDouble(value);
    }

    /// <summary>Returns the string enclosed in single quotes. Changes each single quote inside the string into two single quotes.</summary>
    public static string QuotedSingleSql(this string source)
    {
        if (source != null) source = source.Replace("'", "''");
        return string.Concat("'", source, "'");
    }

    /// <summary>Returns the string enclosed in double quotes. Changes each double quote inside the string into two double quotes.</summary>
    public static string QuotedDoubleSql(this string value)
    {
        if (value != null) value = value.Replace("\"", "\"\"");
        return string.Concat("\"", value, "\"");
    }

    /// <summary>Returns the string enclosed between the prefix and suffix characters.
    /// Throws ArgumentOutOfRangeException if value contains either prefix or suffix.</summary>
    public static string Quoted(this string value, char prefix, char suffix)
    {
        if (value == null)
            return string.Concat(prefix, suffix);

        if (value.Contains(prefix)) throw new ArgumentOutOfRangeException($"value \"{value}\" contains prefix '{prefix}'!");
        if (value.Contains(suffix)) throw new ArgumentOutOfRangeException($"value \"{value}\" contains end '{suffix}'!");
        return string.Concat(prefix, value, suffix);
    }

    /// <summary>Returns the string enclosed between the prefix and suffix strings.
    /// Throws ArgumentOutOfRangeException if value contains either prefix or suffix.</summary>
    public static string Quoted(this string value, string prefix, string suffix)
    {
        if (value == null)
            return string.Concat(prefix, suffix);

        if (value.Contains(prefix)) throw new ArgumentOutOfRangeException($"value \"{value}\" contains prefix \"{prefix}\"!");
        if (value.Contains(suffix)) throw new ArgumentOutOfRangeException($"value \"{value}\" contains end \"{suffix}\"!");
        return string.Concat(prefix, value, suffix);
    }

    #endregion Quoted...

    #region SimiliarTo

    public static decimal SimiliarPercentage(this string source, string word)
    {
        if (word == null) return 0;
        if (source == null) return (word == null || word.Length == 0) ? 1 : 0;

        var original = source.Length > word.Length ? source.Substring(0, word.Length) : source;
        var diff = original.DamerauLevenshteinDistance(word);
        return (word.Length - diff) * 100 / word.Length;
    }

    public static bool SimiliarTo(this string source, string word)
        => SimiliarTo(source, word, 1) && SimiliarTo(word, source, 1);

    public static bool AlmostSimiliarTo(this string source, string word)
        => SimiliarTo(source, word, 0) && SimiliarTo(word, source, 0);

    public static bool SimiliarTo(this string source, string word, int extraErrors)
    {
        if (word == null) return true;
        if (source == null) return (word.Length == 0);

        var original = source.Length > word.Length ? source.Substring(0, word.Length) : source;
        var diff = original.DamerauLevenshteinDistance(word);

        // one error per eight characters + extra
        var maxErrors = (word.Length + 7) / 8 + extraErrors;

        return (diff <= maxErrors);
    }

    public static bool ContainsSimiliarTo(this string source, string word)
        => ContainsSimiliarTo(source, word, 1);

    public static bool ContainsAlmostSimiliarTo(this string source, string word)
        => ContainsSimiliarTo(source, word, 0);

    public static bool ContainsSimiliarTo(this string source, string word, int extraErrors)
    {
        if (word == null) return true;
        if (source == null) return (word.Length == 0);

        // one error per eight characters + extraErrors
        var maxErrors = (word.Length + 7) / 8 + extraErrors;

        var maxLength = word.Length;

        var sourceWords = source.SplitTrim(' ');
        var best = int.MaxValue;
        foreach (var sourceWord in sourceWords)
        {
            var diff = Math.Min(
                sourceWord.DamerauLevenshteinDistance(word),
                word.DamerauLevenshteinDistance(sourceWord));
            if (diff < best) { best = diff; if (best == 0) break; }
        }
        return best <= maxErrors;
    }

    #endregion SimiliarTo

    #region SingleSpaced

    public static string? SingleSpaced(this string? source)
    {
        if (source == null) return null;

        var sb = new StringBuilder(source.Length);
        var space = false;
        var word = false;
        foreach (var c in source)
        {
            if (c == ' ') space = true;
            else
            {
                if (word && space) sb.Append(' ');
                word = true;
                space = false;
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    #endregion SingleSpaced

    #region SplitTrim

    public static string[] SplitTrim(this string? source, char seperator = ',')
    {
        if (source == null) return new string[0];

        var values = source.Split(new char[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (value != null) values[i] = value.Trim();
        }
        return values;
    }

    #endregion SplitTrim

    #region SplitKey

    /// <summary>Splits a full name into parent and child names, such that "a.b" outputs parent "a" and child "b", and
    /// "b" outputs parent null and child "b". Returns true if there was a parent. Note that "a.b.c" will output
    /// parent "a" and child "b.c". Use SplitFullName(grandName, parentName, childName) for three levels.</summary>
    public static bool SplitKey(this string? key, out string? parentName, out string? childName, char seperator = '.')
    {
        if (key == null) { parentName = childName = null; return false; }

        var index = key.IndexOf(seperator);
        // "child"
        if (index == -1) { parentName = null; childName = key; return false; }
        // ".child"
        if (index == 0) { parentName = null; childName = key.Substring(1); return false; }

        // parent.child
        if (index > 0) parentName = key.Substring(0, index); else parentName = null;
        // everything after the first .
        if (index < key.Length - 1) childName = key.Substring(index + 1); else childName = null;
        // We have a parent dot child
        return true;
    }

    #endregion SplitKey

    #region SqlQuoted...

    /// <summary>Returns the string enclosed in single quotes. Changes each single quote inside the string into two single quotes.</summary>
    public static string? SqlQuotedSingle(this string? source)
    {
        if (source != null) source = source.Replace("'", "''");
        return string.Concat("'", source, "'");
    }

    /// <summary>Returns the string enclosed in double quotes. Changes each double quote inside the string into two double quotes.</summary>
    public static string? SqlQuotedDouble(this string? value)
    {
        if (value != null) value = value.Replace("\"", "\"\"");
        return string.Concat("\"", value, "\"");
    }

    #endregion SqlQuoted...

    #region TakeAnyOf

    /// <summary>
    ///     Returns a new string containing only letters, digits and validCharacters.
    ///     Thus if validCharacters is ":", then "Alpha-:-51" returns "Alpha:51"
    ///     and if validCharacters is null, then returns "Alpha51".
    /// </summary>
    /// <param name="source">The source string to extract characters from. If the source is null, the result is null.</param>
    /// <param name="validCharacters">The additional valid characters to consider, which may be null.</param>
    /// <returns>A new string containing only letters, digits and validCharacters from source.</returns>
    public static string? TakeAnyOfAndLettersDigits(this string? source, string? otherValidCharacters = null)
    {
        return source?.TakeAnyOf(CoreConstants.LettersAndDigits + otherValidCharacters);
    }

    /// <summary>
    ///     Returns a new string containing only digits and validCharacters.
    ///     Thus if validCharacters is "ABC123", then "Alpha51" returns "A51"
    ///     and if validCharacters is null, then returns "51".
    /// </summary>
    /// <param name="source">The source string to extract characters from. If the source is null, the result is null.</param>
    /// <param name="validCharacters">The additional valid characters to consider, which may be null.</param>
    /// <returns>A new string containing only digits and validCharacters from source.</returns>
    public static string? TakeAnyOfAndDigits(this string? source, string? otherValidCharacters = null)
    {
        return source?.TakeAnyOf(CoreConstants.Digits + otherValidCharacters);
    }

    public static string? TakeAnyOfDigitsOrDot(this string? source)
    {
        return source?.TakeAnyOf(CoreConstants.Digits + ".");
    }

    /// <summary>
    ///     Returns a new string containing only the original characters that fall into the set of
    ///     validCharacters. Thus if validCharacters is "ABC123", then "Alpha51" returns "A1".
    /// </summary>
    /// <param name="source">The source string to extract characters from. If the source is null, the result is null.</param>
    /// <param name="validCharacters">The characters to return, which cannot be null.</param>
    /// <returns>A new string containing only validCharacters from source.</returns>
    public static string? TakeAnyOf(this string? source, string validCharacters)
    {
        if (validCharacters == null) throw new ArgumentNullException(nameof(validCharacters));

        if (source == null) return null;

        var sb = new StringBuilder();
        foreach (var c in source)
        {
            if (validCharacters.Contains(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    #endregion TakeAnyOf

    #region TakeLeft

    public static string? TakeLeft(this string? source, int maxLength)
    {
        if (source == null) return null;
        if (maxLength <= 0) return "";
        if (source.Length > maxLength) return source.Substring(0, maxLength);
        else return source;
    }

    #endregion TakeLeft

    #region TakeRight

    public static string? TakeRight(this string? source, int maxLength)
    {
        if (source == null) return null;
        if (maxLength <= 0) return "";
        if (source.Length > maxLength) return source.Substring(source.Length - maxLength);
        else return source;
    }

    #endregion TakeRight

    #region ToCaption

    public static string? ToCaption(this string? name)
    {
        if (name == null) return null;

        var sb = new StringBuilder();
        char lastC = ' ';
        foreach (var c in name)
        {
            if (!char.IsUpper(lastC) && char.IsUpper(c)) sb.Append(' ');
            else if (c == '_') sb.Append(' ');
            else sb.Append(c);

            lastC = c;
        }
        return sb.ToString();
    }

    #endregion ToCaption

    #region ToChar

    public static char ToChar(this string text, char @default = ' ')
    {
        if (text.IsNullOrEmpty()) return @default;

        if (text.Length != 1)
            throw new InvalidCastException($"Cannot convert '{text}' to char!");

        return text[0];
    }

    #endregion ToChar

    #region ToDecimal

    public static decimal ToDecimal(this string source)
    {
        try
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            return decimal.Parse(source, culture);
        }
        catch (FormatException)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-ZA");
            return decimal.Parse(source, culture);
        }
    }

    #endregion ToDecimal

    #region ToDouble

    public static double ToDouble(this string source)
    {
        try
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            return double.Parse(source, culture);
        }
        catch (FormatException)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-ZA");
            return double.Parse(source, culture);
        }
    }

    #endregion ToDouble

    #region ToDigitsOnly

    public static string? ToDigitsOnly(this string? source) => source?.TakeAnyOf(CoreConstants.Digits);

    #endregion ToDigitsOnly

    #region ToEnum

    public static ENUM ToEnum<ENUM>(this string s) where ENUM : struct
    {
        if (!Enum.TryParse<ENUM>(s, out ENUM e))
            throw new InvalidCastException($"Could not cast \"{s}\" to {e.GetType()}!");
        return e;
    }

    #endregion ToEnum

    #region ToIdentifier

    /// <summary>
    ///     Returns an identifier by returning a new string containing only letters and digits, and starting with a letter.
    /// </summary>
    /// <param name="source">The source string to extract characters from. If the source is null, the result is null.</param>
    /// <param name="validCharacters">The additional valid characters to consider, which may be null.</param>
    /// <returns>A new string containing only digits and validCharacters from source.</returns>
    public static string? ToIdentifier(this string? source, int maxLength = 50)
    {
        if (source == null) return null;

        var sb = new StringBuilder();
        var first = true;
        foreach (var c in source)
        {
            if (first) { if (!char.IsLetter(c)) continue; }
            else { if (!char.IsLetter(c) && !char.IsDigit(c)) continue; }
            sb.Append(c);
            if (sb.Length == maxLength) break;
        }

        return sb.ToString();
    }

    #endregion ToIdentifier

    #region ToLowerFirstChar

    /// <summary>Changes the first character to lower, and ignores the casing of the rest of the string.</summary>
    public static string? ToLowerFirstChar(this string? source)
    {
        if (source == null) return null;
        if (source.Length == 1) return source.ToLower();
        return source.Substring(0, 1).ToLower() + source.Substring(1);
    }

    #endregion ToLowerFirstChar

    #region ToOnly

    public static string? ToOnly(this string? source, string validCharacters)
    {
        if (string.IsNullOrEmpty(validCharacters))
            throw new ArgumentNullException(nameof(validCharacters));

        if (string.IsNullOrEmpty(source)) return source;

        var sb = new StringBuilder(source.Length);
        foreach (var c in source)
        {
            if (validCharacters.Contains(c)) sb.Append(c);
        }
        return sb.ToString();
    }

    #endregion ToOnly

    #region ToSeparatedWords

    /// <summary>
    /// Changes the first character to lower, and ignores the rest.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? ToSeparatedWords(this string? source, char separator = ' ', bool allLowercase = true)
    {
        if (source == null) return null;
        if (source.Length == 1) return source.ToLower();

        var sb = new StringBuilder();
        var inWord = false;
        foreach (var c in source)
        {
            if (char.IsUpper(c))
            {
                if (inWord) sb.Append(separator);

                if (allLowercase)
                    sb.Append(char.ToLower(c));
                else
                    sb.Append(c);

                inWord = false;
            }
            else
            {
                inWord = true;
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    #endregion ToSeparatedWords

    #region ToXmlText

    public static string? ToXmlText(this string? source)
    {
        if (source == null) return null;

        return System.Security.SecurityElement.Escape(source);
    }

    #endregion ToXmlText
}