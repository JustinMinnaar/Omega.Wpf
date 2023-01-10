using System;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1.Profiling;

public class ProProfileModel : IdNamedModel
{
    public required ProGroupModel OwnerGroup { get; set; }
    
    public ObservableCollection<ProTemplateModel> Templates { get; set; } = new();
    public Guid? SelectedTemplateId { get; set; }
    public ProTemplateModel? SelectedTemplate { get; set; }
    
    public Guid? GroupId { get; set; }
}
