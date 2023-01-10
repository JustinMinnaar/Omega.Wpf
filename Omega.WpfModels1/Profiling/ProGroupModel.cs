using System;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1.Profiling;

public class ProGroupModel : IdNamedModel
{
    public required ProBagModel OwnerBag { get; set; }
    public ObservableCollection<ProProfileModel> Profiles { get; set; } = new();
    public Guid? SelectedProfileId { get; set; }
    public ProProfileModel? SelectedProfile { get; set; }
}
