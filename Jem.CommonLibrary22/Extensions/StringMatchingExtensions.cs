using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jem.CommonLibrary22;

public static class StringMatchingExtensions
{
    // todo: optimise
    public static bool ContainsTextWithoutSpacing(this string source, string contains)
    {
        source = source.Replace(" ", "");
        // contains = contains.Replace(" ", "");
        return source.Contains(contains, StringComparison.OrdinalIgnoreCase);

        //if (contains.Contains(' '))
        //    throw new ArgumentOutOfRangeException(nameof(contains));

        //var countSpaces = 0;
        //foreach (var c in source)
        //    if (c == ' ') countSpaces++;

        //for (int i = 0; i < source.Length - contains.Length - countSpaces; i++)
        //{

        //}
    }
}
