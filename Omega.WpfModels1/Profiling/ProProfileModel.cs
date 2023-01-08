using System.Collections.ObjectModel;

namespace Omega.WpfModels1.Profiling;

public class ProProfileModel : IdNamedModel
{
    public ObservableCollection<ProTemplateModel> Templates { get; set; } = new();

    public ProTemplateModel? SelectedTemplate { get; set; }
}
