using Bdo.DatabaseLibrary1;

using Omega.WpfModels1;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Omega.WpfControllers1;

public class UserSettingsController : IdNamedModel
{
    [SetsRequiredMembers]
    public UserSettingsController(MainController main)
    {
        this.Main = main;

        var userName = Environment.UserName;

        using var db = new BdoDbContext();
        var user = db.SysUserSettings.FirstOrDefault(u => u.Name == userName);
        if (user == null)
        {
            this.Id = Guid.NewGuid();
            this.Name = userName;
            user = new SysUserSettings { Id = this.Id, Name = userName };
            db.SaveChanges();
        }
        else
        {
            this.Id = user.Id;
            this.Name = user.Name ?? userName;
            this.ResetPanZoomOnFileSelect = user.ResetPanZoomOnFileSelect;
            this.SnapTop = user.SnapTop;
            this.SnapBottom = user.SnapBottom;
            this.SnapLeft = user.SnapLeft;
            this.SnapRight = user.SnapRight;
            this.SelectedDocFileId = user.SelectedDocFileId;
            this.SelectedDocFolderId = user.SelectedDocFolderId;
            this.SelectedDocPageId = user.SelectedDocPageId;
            this.SelectedDocProjectId = user.SelectedDocProjectId;
            this.SelectedDocSolutionId = user.SelectedDocSolutionId;
            this.SelectedProProfileId = user.SelectedProProfileId;
            this.SelectedProTemplateId = user.SelectedProTemplateId;
        }
    }

    public required MainController Main { get; init; }

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
