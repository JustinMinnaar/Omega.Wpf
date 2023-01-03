namespace Jem.Profiling22.Data;

using System;
using System.Collections.ObjectModel;
using System.Linq;

public class ProGroups : ObservableCollection<ProGroup>
{
    public int LastGroupNumberAdded { get; set; }

    public ProGroup AddGroup()
    {
        LastGroupNumberAdded++;

        var group = AddGroup(new ProGroup { Name = "Group #" + LastGroupNumberAdded });

        SelectedGroupId = group.Id;
        return group;
    }

    public ProGroup AddGroup(Guid id, string name)
    {
        return AddGroup(new ProGroup { Id = id, Name = name });
    }

    public ProGroup AddGroup(ProGroup proGroup)
    {
        base.Add(proGroup);
        return proGroup;
    }

    public void DelSelectedGroup() => DelGroup(SelectedGroup);

    public void DelGroup(ProGroup? group)
    {
        if (group == null) return;

        if (SelectedGroupId == group.Id)
            SelectedGroupId = null;

        Remove(group);
    }

    public Guid? SelectedGroupId { get; set; }

    public ProGroup? SelectedGroup
    {
        get => this.FirstOrDefault(a => a.Id == SelectedGroupId);
        set => SelectedGroupId = value?.Id;
    }
}
