namespace Jem.ProfilingLibrary23;

// note: using an array is significantly faster than a list

public class JProGroup : IdNamed
{
    public JProGroup(ProGroup group)
    {
        Id = group.Id;
        Name = group.Name;
        Profiles = CompileProfiles(group.Profiles).ToArray();
    }

    private IEnumerable<JProProfile> CompileProfiles(IEnumerable<ProProfile> profiles)
    {
        foreach (var profile in profiles)
            yield return new JProProfile(profile);
    }

    public JProProfile[] Profiles { get; }
}
