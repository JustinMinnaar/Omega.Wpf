﻿using Bdo.DatabaseLibrary1;

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

        this.DarkMode = user.DarkMode;

        this.SelectedDocSolutionId = user.SelectedDocSolutionId;
        this.SelectedProBagId = user.SelectedProBagId;

        this.WorkingFolderPath = user.WorkingFolderPath;
    }

    public async Task SaveAsync()
    {
        using var db = new BdoDocDbContext();

        var user = await db.AccessSysUserSettings(userName: this.Name);

        user.ResetPanZoomOnFileSelect = this.ResetPanZoomOnFileSelect;
        user.SnapTop = this.SnapTop;
        user.SnapBottom = this.SnapBottom;
        user.SnapLeft = this.SnapLeft;
        user.SnapRight = this.SnapRight;

        user.DarkMode = this.DarkMode;
        user.WorkingFolderPath = this.WorkingFolderPath;

        user.SelectedDocSolutionId = this.SelectedDocSolutionId;
        user.SelectedProBagId = this.SelectedProBagId;

        await db.SaveChangesAsync();
    }

    public required MainController Main { get; init; }

    public Guid? SelectedDocSolutionId { get; set; }
    public Guid? SelectedProBagId { get; set; }
    public bool ResetPanZoomOnFileSelect { get; set; } = true;
    public bool SnapTop { get; set; } = true;
    public bool SnapBottom { get; set; } = true;
    public bool SnapLeft { get; set; } = false;
    public bool SnapRight { get; set; } = false;
    public bool DarkMode { get; set; }
    public string? WorkingFolderPath { get; set; }

    public bool HasChanged { get; set; }

    public override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(HasChanged))
            this.HasChanged = true;
    }

}
