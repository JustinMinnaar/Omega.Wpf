
/// <summary>Contains configuration for the current user.</summary>
public class SysUserSettings : IdNamed
{
    public Guid? SelectedDocSolutionId { get ; set; }
    public DocSolution ?SelectedDocSolution { get; set; }
    
    public Guid? SelectedProBagId { get; set; }
    
    public bool ResetPanZoomOnFileSelect { get; set; } = true;
    public bool SnapTop { get; set; } = true;
    public bool SnapBottom { get; set; } = true;
    public bool SnapLeft { get; set; } = false;
    public bool SnapRight { get; set; } = false;
    public bool DarkMode { get; set; }

    public string? WorkingFolderPath { get; set; }

    //#region Selections

    ///// <summary>A collection of selections for this user.</summary>
    //public ICollection<SysUserSelection> Selections { get; set; } = default!;

    //public Guid? GetSelection(Guid sourceId)
    //{
    //    var entry = Selections?.FirstOrDefault(s => s.SettingsId == this.Id && s.SourceId == sourceId);
    //    return entry?.TargetId;
    //}

    //public void SetSelection(Guid sourceId, Guid targetId)
    //{
    //    var entry = Selections.FirstOrDefault(s => s.SettingsId == this.Id && s.SourceId == sourceId );
    //    if (entry != null)
    //    {
    //        entry.TargetId = targetId;
    //    }
    //    else
    //    {
    //        Selections.Add(new SysUserSelection { SettingsId = this.Id, SourceId = sourceId, TargetId = targetId });
    //    }
    //}

    //#endregion
}

///// <summary>Contains selections for the current user.</summary>
//public class SysUserSelection
//{
//    public required Guid SettingsId { get; set; }
//    public SysUserSettings Settings { get; set; } = default!;
//    public required Guid SourceId { get; set; }
//    public Guid? TargetId { get; set; }
//}