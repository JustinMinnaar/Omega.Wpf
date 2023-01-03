namespace Jem.Profiling22.Data;

using System;
using System.Collections.ObjectModel;
using System.Linq;
public class ProGroup : IdNamed
{
    public ObservableCollection<ProProfile> Profiles { get; set; } = new();

    public int LastProfileNumberAdded { get; set; }
    
    public ProProfile AddProfile(Guid id, string name)
    {
        return AddProfile(new ProProfile { Id = id, Name = name });
    }

    public void AddProfile()
    {
        LastProfileNumberAdded++;

        var profile = new ProProfile("Profile #" + LastProfileNumberAdded);
        AddProfile(profile);

        SelectedProfileId = profile.Id;
    }

    private ProProfile AddProfile(ProProfile profile)
    {
        Profiles.Add(profile);
        return profile;
    }

    public void DelSelectedProfile() => DelProfile(SelectedProfile);

    public void DelProfile(ProProfile? profile)
    {
        if (profile == null) return;

        if (SelectedProfileId == profile.Id)
            SelectedProfileId = null;

        Profiles.Remove(profile);
    }

    public Guid? SelectedProfileId { get; set; }
    
    public ProProfile? SelectedProfile
    {
        get => Profiles.FirstOrDefault(a => a.Id == SelectedProfileId);
        set => SelectedProfileId = value?.Id;
    }


    // public Dictionary<string, string> Values { get; set; } = new();
}
