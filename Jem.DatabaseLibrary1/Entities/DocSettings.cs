
/// <summary>Contains configuration for the current user.</summary>
public class SysUserSettings : IdNamed
{
    public Guid? SelectedDocSolutionId { get; set; }
    public Guid? SelectedDocProjectId { get; set; }
    public Guid? SelectedDocFolderId { get; set; }
    public Guid? SelectedDocFileId { get; set; }
    public Guid? SelectedDocPageId { get; set; }
    public Guid? SelectedProProfileId { get; set; }
    public Guid? SelectedProTemplateId { get; set; }
    public bool ResetPanZoomOnFileSelect { get; set; } = true;
    public bool SnapTop { get; set; } = true;
    public bool SnapBottom { get; set; } = true;
    public bool SnapLeft { get; set; } = false;
    public bool SnapRight { get; set; } = false;
}