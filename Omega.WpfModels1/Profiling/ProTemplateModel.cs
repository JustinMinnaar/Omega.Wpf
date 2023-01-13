using Jem.CommonLibrary22;
using Jem.Profiling22.Data;

namespace Omega.WpfModels1.Profiling;

public class ProTemplateModel : IdNamedModel
{
    public required ProProfileModel OwnerProfile { get; set; }

    public ProfileTemplateType Type { get; set; }
    public CRect? Rect { get; set; }
    public string? RectAsText
    {
        get => Rect.ToString();
        set { Rect = value == null ? CRect.Empty : CRect.Parse(value); }
    }
    public string? RectText { get; set; }
}
