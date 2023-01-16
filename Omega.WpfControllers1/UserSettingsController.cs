using Bdo.DatabaseLibrary1;

using Microsoft.EntityFrameworkCore;

using Omega.WpfModels1;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Omega.WpfControllers1;

public class UserSettingsController : IdNamedModel
{
    [SetsRequiredMembers]
    public UserSettingsController(MainController main)
    {
        this.Main = main;

        var userName = Environment.UserName;
        this.Id = Guid.NewGuid();
        this.Name = userName;
    }

    public async Task LoadAsync()
    {
        using var db = new BdoDocDbContext();

        var user = await db.AccessSysUserSettings(userName: this.Name);

        this.Id = user.Id;
        this.Name = user.Name;

        this.ResetPanZoomOnFileSelect = user.ResetPanZoomOnFileSelect;
        this.SnapTop = user.SnapTop;
        this.SnapBottom = user.SnapBottom;
        this.SnapLeft = user.SnapLeft;
        this.SnapRight = user.SnapRight;

        this.SelectedDocSolutionId = user.SelectedDocSolutionId;
        this.SelectedDocProjectId = user.SelectedDocProjectId;

        this.SelectedProBagId = user.SelectedProBagId;

        this.DarkMode = user.DarkMode;
        this.WorkingFolderPath = user.WorkingFolderPath;
        this.ImportFolderPath = user.ImportFolderPath;
        this.ImportExcludeWildcards = user.ImportExcludeExtensions;
        this.ImportFoldersInParallel = user.ImportFoldersInParallel;
        this.ImportFilesInSubFolders = user.ImportFilesInSubFolders;

        this.HasChanged = false;
    }

    public async Task SaveAsync()
    {
        if (!this.HasChanged) return;

        using var db = new BdoDocDbContext();

        var user = await db.AccessSysUserSettings(userName: this.Name);

        user.ResetPanZoomOnFileSelect = this.ResetPanZoomOnFileSelect;
        user.SnapTop = this.SnapTop;
        user.SnapBottom = this.SnapBottom;
        user.SnapLeft = this.SnapLeft;
        user.SnapRight = this.SnapRight;

        user.SelectedDocSolutionId = this.SelectedDocSolutionId;
        user.SelectedDocProjectId = this.SelectedDocProjectId;

        user.SelectedProBagId = this.SelectedProBagId;

        user.DarkMode = this.DarkMode;
        user.WorkingFolderPath = this.WorkingFolderPath;
        user.ImportFolderPath = this.ImportFolderPath;
        user.ImportExcludeExtensions = this.ImportExcludeWildcards;
        user.ImportFoldersInParallel = this.ImportFoldersInParallel;
        user.ImportFilesInSubFolders = this.ImportFilesInSubFolders;

        await db.SaveChangesAsync();

        this.HasChanged = false;
    }

    public required MainController Main { get; init; }

    public bool ResetPanZoomOnFileSelect { get; set; } = true;
    public bool SnapTop { get; set; } = true;
    public bool SnapBottom { get; set; } = true;
    public bool SnapLeft { get; set; } = false;
    public bool SnapRight { get; set; } = false;

    public Guid? SelectedDocSolutionId { get; set; }
    public Guid? SelectedDocProjectId { get; set; }

    public Guid? SelectedProBagId { get; set; }

    public bool DarkMode { get; set; }
    /// <summary>the path used to store ocr, csv, txt, etc. temporary files.</summary>
    public string? WorkingFolderPath { get; set; }

    /// <summary>The path to import on a run.</summary>
    public string? ImportFolderPath { get; set; }

    /// <summary>A list of extensions, separated by ;, to exclude from import.</summary>
    public string? ImportExcludeWildcards { get; set; }

    /// <summary>True to optimise and perform import of folders in parallel.</summary>
    /// <remarks>Turn off during debugging.</remarks>
    public bool ImportFoldersInParallel { get; set; }

    public bool ImportFilesInSubFolders { get; set; }

    public bool HasChanged { get; set; }

    public override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(HasChanged) && propertyName != nameof(Main))
            this.HasChanged = true;
    }

}
