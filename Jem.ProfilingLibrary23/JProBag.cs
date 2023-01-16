using System.Collections.ObjectModel;

namespace Jem.ProfilingLibrary23;

public sealed class JProBag : IdNamed
{
    public JProGroup[] Groups { get; }

    public List<SkipKeyValue> Skipables { get; }

    public JProBag(ProBag root)
    {
        Id = root.Id;
        Name = root.Name;

        Groups = CreateGroups(root).ToArray();
        Skipables = CreateSkipables(root);
    }

    private static IEnumerable<JProGroup> CreateGroups(ProBag root)
    {
        foreach (var group in root.Groups)
        {
            yield return new JProGroup(group);
        }
    }

    private static List<SkipKeyValue> CreateSkipables(ProBag root)
    {
        var skipables = new List<SkipKeyValue>();
        foreach (var kv in root.Skipables)
        {
            var name = kv.Key;
            foreach (var text in kv.Value)
            {
                skipables.Add(new SkipKeyValue(name, text.NoSpaces()));
            }
        }
        return skipables;
    }

    public JProProfile? TryGetProfile(Guid? id)
    {
        foreach (var group in Groups)
            foreach (var profile in group.Profiles)
                if (profile.Id == id) return profile;

        return null;
    }
}
