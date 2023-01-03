namespace Jem.CommonLibrary22;

public static class ICollectionExtensions
{
    public static int IndexOf<T>(this ICollection<T> list, T value)
    {
        int i = 0;
        foreach (var item in list)
        {
            if (value == null) { if (item == null) return i; }
            else { if (value.Equals(item)) return i; }
            i++;
        }
        return -1;
        // throw new NotFoundException($"'{value}' not found!");
    }


    public static void RemoveAllNewItemsFromOldList<T>(this IEnumerable<T> newList, ICollection<T> oldList) where T : class, IId
    {
        foreach (var item in newList)
        {
            if (oldList.TryGetById(item.Id) == null) oldList.Remove(item);
        }
    }

    /// <summary>This returns the count followed by the singular or plural form of the name, i.e. 0 companies, 1 company or 2 companies.</summary>
    public static string ToCount(this ICollection<object> list, int count, string nameSingle, string namePlural)
    {
        return count + " " + (count == 1 ? nameSingle : namePlural);
    }

    public static string CountText<T>(this ICollection<T> list, int count, string single, string plural)
        => count + " " + (count == 1 ? single : plural);

    public static string CountText<T>(this ICollection<T> list, string single, string plural)
        => list.Count + " " + (list.Count == 1 ? single : plural);

    public static bool ContainsById<T>(this ICollection<T> list, Guid id) where T : class, IId
    {
        foreach (var item in list)
        {
            if (item.Id == id) return true;
        }
        return false;
    }

    public static bool ContainsByName<T>(this ICollection<T> list, string name) where T : class, IName
    {
        foreach (var item in list)
        {
            if (item.Name == name) return true;
        }
        return false;
    }

    public static T? TryGetById<T>(this ICollection<T> list, Guid id) where T : class, IId
    {
        // slower: return list.FirstOrDefault(item => item.Id == guid);
        foreach (var item in list)
        {
            if (item.Id == id) return item;
        }
        return default(T);
    }

    public static T? TryGetByName<T>(this ICollection<T> list, string name) where T : class, IName
    {
        foreach (var item in list)
        {
            if (item.Name == name) return item;
        }
        return default(T);
    }

    public static T? TryTakeById<T>(this List<T> newList, List<T> oldList) where T : class, IId
    {
        foreach (var item in newList)
        {
            if (oldList.TryGetById(item.Id) == null)
            {
                oldList.Remove(item);
                return item;
            }
        }
        return null;
    }

    public static T GetById<T>(this ICollection<T> list, Guid id) where T : class, IId
    {
        foreach (var item in list)
        {
            if (item.Id == id) return item;
        }
        throw new InvalidItemException(id + " not found.");
    }

    public static T GetByName<T>(this ICollection<T> list, string name) where T : class, IName
    {
        foreach (var item in list)
        {
            if (item.Name == name) return item;
        }
        throw new NotFoundException($"'{name}' not found!");
    }

    /// <summary>This function is called Matches because we cannot add an extension method to
    /// an interface (ie the ICollection).</summary>
    public static bool Matches<T>(this ICollection<T> list, ICollection<T> fromList) where T : class, IItem
    {
        if (list.Count != fromList.Count) return false;

        // All target items must be in the original list, and match
        foreach (T item in list)
        {
            var fromItem = fromList.TryGetById(item.Id);
            if (fromItem == null) return false;

            if (!item.Equals(fromItem)) return false;
        }

        // All original items must be in the target
        foreach (var fromItem in fromList)
        {
            var item = list.TryGetById(fromItem.Id);
            if (fromItem == null)
            {
                return false;
            }
        }

        return true;
    }

    public static string? AsStringSeparated<T>
        (this List<T> list, string separator = ", ", string? prefix = null, string? suffix = null, int maxLength = int.MaxValue)
    {
        if (list == null) return null;

        var result = new StringBuilder();

        var first = true;
        foreach (var s in list)
        {
            if (!first) result.Append(separator);
            first = false;

            if (prefix != null) result.Append(prefix);
            var text = s?.ToString();
            if (text != null) result.Append(text);
            if (suffix != null) result.Append(suffix);

            if (result.Length >= maxLength) break;
        }

        return result.ToString();
    }
}