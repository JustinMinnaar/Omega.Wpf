using System;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1.Profiling;

public class ProBagModel : IdNamedModel
{
    public ObservableCollection<ProGroupModel> Groups { get; set; } = new();

    public Guid? SelectedGroupId { get; set; }
    public ProGroupModel? SelectedGroup { get; set; }
}
