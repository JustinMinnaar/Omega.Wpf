using System;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1.Profiling;

public class ProGroupModel : IdNamedModel
{
    public ObservableCollection<ProProfileModel> Profiles { get; set; } = new();

    public Guid? SelectedProfileId { get; set; }
    public ProProfileModel? SelectedProfile { get; set; }
}
