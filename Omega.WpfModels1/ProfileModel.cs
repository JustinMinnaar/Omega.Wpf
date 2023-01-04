using System.Collections.ObjectModel;

namespace Omega.WpfModels1;

public class ProfileModel : IdNamedModel
{
    public ObservableCollection<TemplateModel> Templates { get; set; } = new();

    public TemplateModel? SelectedTemplate { get; set; }
}
