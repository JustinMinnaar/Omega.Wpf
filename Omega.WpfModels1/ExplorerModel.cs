using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Omega.WpfModels1;

public enum IdentifiedFilter { All, Unknown, Known }

public class UserOptionsModel : IdNamedModel
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

// Projects, Folders, Files, Pages, Profiles: are loaded on demand
public class ExplorerModel : IdNamedModel
{
    public UserOptionsModel UserOptions { get; set; } = new UserOptionsModel { Id = Guid.NewGuid(), Name = Environment.UserName };

    public ObservableCollection<SolutionModel>? Solutions { get; set; } = new();
    public SolutionModel? SelectedSolution { get; set; }

    public event EventHandler? SelectedSolutionChanged;
    protected virtual void OnSelectedSolutionChanged() => SelectedSolutionChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<ProjectModel>? Projects { get; set; } = new();
    public ProjectModel? SelectedProject { get; set; }

    public event EventHandler? SelectedProjectChanged;
    protected virtual void OnSelectedProjectChanged() => SelectedProjectChanged?.Invoke(this, EventArgs.Empty);

    public static IdentifiedFilter[] IdentifiedFilters => (IdentifiedFilter[])Enum.GetValues(typeof(IdentifiedFilter));
    public IdentifiedFilter? SelectedIdentifiedFilter { get; set; }

    public event EventHandler? SelectedIdentifiedFilterChanged;
    protected virtual void OnSelectedIdentifiedFilterChanged() => SelectedIdentifiedFilterChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<FolderModel>? Folders { get; set; } = new();
    public FolderModel? SelectedFolder { get; set; }

    public event EventHandler? SelectedFolderChanged;
    protected virtual void OnSelectedFolderChanged() => SelectedFolderChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<FileModel>? Files { get; set; } = new();
    public FileModel? SelectedFile { get; set; }

    public event EventHandler? SelectedFileChanged;
    protected virtual void OnSelectedFileChanged() => SelectedFileChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<PageModel>? Pages { get; set; } = new();
    public PageModel? SelectedPage { get; set; }

    public event EventHandler? SelectedPageChanged;
    protected virtual void OnSelectedPageChanged() => SelectedPageChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<ProfileModel>? Profiles { get; set; } = new();
    public ProfileModel? SelectedProfile { get; set; }

    public event EventHandler? SelectedProfileChanged;
    protected virtual void OnSelectedProfileChanged() => SelectedProfileChanged?.Invoke(this, EventArgs.Empty);

    public OcrDocument? ODocument { get; set; }
    public OcrPage? OPage { get; set; }

    //public string? LastError { get; set; }
    //public bool HasLastError => LastError != null;
    public string? LastMessage { get; set; }

    public async Task LoadAsync(string userName)
    {
        try
        {
            await LoadUserAsync(userName);
            await LoadSolutionsAsync();
        }
        catch (Exception ex)
        {
            LastMessage = ex.Message;
        }
    }

    public async Task<SolutionModel> AccessSolutionAsync(string solutionName)
    {
        using var db = new BdoDbContext();

        var dbSolution = await db.TryGetSolutionAsync(solutionName);
        if (dbSolution == null)
        {
            dbSolution = new DocSolution { Id = Guid.NewGuid(), Name = solutionName };
            db.DocSolutions.Add(dbSolution);
            await db.SaveChangesAsync();
        }

        var mSolution = new SolutionModel { Id = dbSolution.Id, Name = dbSolution.Name };
        return mSolution;
    }

    private async Task LoadUserAsync(string userName)
    {
        var db = new BdoDbContext();
        var dbUser = await db.SysUserSettings.AsNoTracking().FirstOrDefaultAsync(u => u.Name == userName);
        if (dbUser == null)
        {
            dbUser = new SysUserSettings { Id = new(), Name = userName };
            await db.SaveChangesAsync();
        }

        this.UserOptions = new UserOptionsModel
        {
            Id = dbUser.Id,
            Name = dbUser.Name,
            SnapBottom = dbUser.SnapBottom,
            SnapLeft = dbUser.SnapLeft,
            SnapRight = dbUser.SnapRight,
            SnapTop = dbUser.SnapTop,
            ResetPanZoomOnFileSelect = dbUser.ResetPanZoomOnFileSelect,
            SelectedDocFileId = dbUser.SelectedDocFileId,
            SelectedDocFolderId = dbUser.SelectedDocFolderId,
            SelectedDocPageId = dbUser.SelectedDocPageId,
            SelectedDocProjectId = dbUser.SelectedDocProjectId,
            SelectedDocSolutionId = dbUser.SelectedDocSolutionId,
            SelectedProProfileId = dbUser.SelectedProProfileId,
            SelectedProTemplateId = dbUser.SelectedProTemplateId,
        };
    }

    public async Task LoadSolutionsAsync()
    {
        SelectedSolution = null;
        SelectedProject = null;
        SelectedIdentifiedFilter = IdentifiedFilter.All;
        SelectedFolder = null;
        SelectedFile = null;
        SelectedPage = null;

        Solutions = new ObservableCollection<SolutionModel>();

        using var db = new BdoDbContext();
        var dbSolutions = await db.DocSolutions.OrderBy(r => r.Name).ToListAsync();

        // If no previous user selection matches, we'll select the first one
        var dbSelected = dbSolutions.FirstOrDefault(s => s.Id == UserOptions?.SelectedDocSolutionId) ?? dbSolutions.FirstOrDefault();

        // create models for each solution, and select the selected's model
        foreach (var dbSolution in dbSolutions)
        {
            var mSolution = new SolutionModel { Id = dbSolution.Id, Name = dbSolution.Name };
            Solutions.Add(mSolution);
            if (dbSelected?.Id == mSolution.Id) SelectedSolution = mSolution;
        }
    }

    public async Task LoadProjectsAsync()
    {
        SelectedProject = null;
        SelectedIdentifiedFilter = IdentifiedFilter.All;
        SelectedFolder = null;
        SelectedFile = null;
        SelectedPage = null;

        Projects = new ObservableCollection<ProjectModel>();

        var mRoot = SelectedSolution;
        if (mRoot == null) return;

        using var db = new BdoDbContext();
        var dbProjects = await db.DocProjects.Where(p => p.OwnerSolutionId == mRoot.Id).OrderBy(p => p.Name).ToListAsync();

        foreach (var dbProject in dbProjects)
        {
            var mProject = new ProjectModel { Id = dbProject.Id, Name = dbProject.Name };
            Projects.Add(mProject);
            SelectedProject ??= mProject;
        }
    }

    public async Task LoadFolders()
    {
        SelectedFolder = null;
        SelectedFile = null;
        SelectedPage = null;

        Folders = new ObservableCollection<FolderModel>();

        var mProject = SelectedProject;
        if (mProject == null) return;

        using var db = new BdoDbContext();

        var q = db.DocFolders.Where(f => f.OwnerProjectId == mProject.Id);
        if (SelectedIdentifiedFilter == IdentifiedFilter.Unknown)
            q = q.Where(f => f.AreAllFilesIdentified == false);
        if (SelectedIdentifiedFilter == IdentifiedFilter.Known)
            q = q.Where(f => f.AreAllFilesIdentified == true);
        var dbFolders = await q.OrderBy(f => f.Name).ToListAsync();

        foreach (var dbFolder in dbFolders)
        {
            var mFolder = new FolderModel
            {
                Id = dbFolder.Id,
                Name = dbFolder.Name,
                AreAnyFileError = dbFolder.AreAnyFileError,
                AreAllFilesIdentified = dbFolder.AreAllFilesIdentified
            };
            Folders.Add(mFolder);
            SelectedFolder ??= mFolder;
        }
    }

    public async Task LoadFiles()
    {
        SelectedFile = null;
        Files = new ObservableCollection<FileModel>();

        var mFolder = SelectedFolder;
        if (mFolder == null) return;

        using var db = new BdoDbContext();

        var q = db.DocFiles.Where(f => f.OwnerFolderId == mFolder.Id);
        if (SelectedIdentifiedFilter == IdentifiedFilter.Unknown)
            q = q.Where(f => f.IsIdentified == false);
        if (SelectedIdentifiedFilter == IdentifiedFilter.Known)
            q = q.Where(f => f.IsIdentified == true);
        var dbFiles = await q.OrderBy(f => f.Name).ToListAsync();

        foreach (var dbFile in dbFiles)
        {
            var mFile = new FileModel { Id = dbFile.Id, Name = dbFile.Name };
            Files.Add(mFile);
            SelectedFile ??= mFile;
        }
    }

    public async Task LoadOcr()
    {
        LastMessage = await DoLoadOcr();

        async Task<string?> DoLoadOcr()
        {
            ODocument = null;

            var mProject = SelectedProject; if (mProject == null) return "No project selected.";
            var mFolder = SelectedFolder; if (mFolder == null) return "No folder selected.";
            var mFile = SelectedFile; if (mFile == null) return "No file selected.";

            var rootPath = @"F:\Bdo";
            var projectName = mProject.Name;
            var folderName = mFolder.Name;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(mFile.Name);

            var binOcrFilePath = $@"{rootPath}\{projectName}\{folderName}\{fileName}.bocr";
            ODocument = await OcrDocument.TryLoadFromBinaryFileAsync(binOcrFilePath);
            if (ODocument == null) return "No Ocr available.";

            ODocument.ResizeTo2100();

            return null;
        }
    }

    public async Task AfterFileChanged()
    {
        await LoadOcr();
        await LoadPages();
    }

    private async Task LoadPages()
    {
        SelectedPage = null;
        Pages = new ObservableCollection<PageModel>();

        var mFile = SelectedFile; if (mFile == null) return;

        using var db = new BdoDbContext();
        var dbPages = await db.DocPages.Where(f => f.OwnerFileId == mFile.Id).OrderBy(p => p.Name).ToListAsync();

        if (ODocument != null)
        {
            foreach (var oPage in ODocument.Pages)
            {
                var dbPage = dbPages.FirstOrDefault(p => p.PageIndex == oPage.PageIndex);

                var pageName = $"{(oPage.PageIndex + 1)} ({oPage.SymbolsCount})";
                var isBlank = (oPage.SymbolsCount == 0);

                if (dbPage == null)
                {
                    dbPage = new DocPage
                    {
                        OwnerFileId = mFile.Id,
                        PageIndex = oPage.PageIndex,
                        Name = pageName,
                        SymbolCount = oPage.SymbolsCount,
                        IsBlank = isBlank,
                    };
                    dbPages.Add(dbPage);
                }
                else
                {
                    if (dbPage.Name != pageName)
                        dbPage.Name = pageName;

                    if (dbPage.SymbolCount != oPage.SymbolsCount)
                        dbPage.SymbolCount = oPage.SymbolsCount;

                    if (dbPage.IsBlank != isBlank)
                        dbPage.IsBlank = isBlank;
                }
            }

            await db.SaveChangesAsync();
        }

        foreach (var dbPage in dbPages)
        {
            var mPage = new PageModel
            {
                Id = dbPage.Id,
                Name = dbPage.Name,
                PageIndex = dbPage.PageIndex,
                IsBlank = dbPage.IsBlank,
                IsError = dbPage.IsError,
                ProfileId = dbPage.ProfileId,
                ProfileName = dbPage.ProfileName,
                ProfileVersion = dbPage.ProfileVersion,
            };
            Pages.Add(mPage);
            SelectedPage ??= mPage;
        }
    }

    public void LoadPage()
    {
        OPage = null;

        if (SelectedPage == null) return;

        if (ODocument == null) return;

        OPage = ODocument.Pages.FirstOrDefault(p => p.PageIndex == SelectedPage.PageIndex);
        if (OPage == null) return;

        LastMessage = $"Page {(OPage.PageIndex + 1)} contains {OPage.SymbolsCount} symbols for {SelectedFile?.Name}.";
    }

    public CRect? LastRectangleDrawn { get; set; }

    public string? LastRectangleText { get; set; }

    public CRect? RectangleDrawn(Rect mouseRect)
    {
        ExtractRectangleText(new CRect(mouseRect.X, mouseRect.Y, mouseRect.Width, mouseRect.Height));
        return LastRectangleDrawn;
    }

    public void ExtractRectangleText(CRect rect)
    {
        LastRectangleDrawn = rect;

        if (OPage == null) return;
        var result = OPage?.ExtractSymbolsAndText(LastRectangleDrawn.Value, maxSymbolHeight: 999f);

        LastRectangleText = result?.Text;
        if (result == null)
        {
            LastRectangleDrawn = null;
        }
        else
        {
            if (UserOptions.SnapTop) rect.Top = result.Bounds.Top; else rect.Top = Math.Min(rect.Top, result.Bounds.Top);
            if (UserOptions.SnapBottom) rect.Bottom = result.Bounds.Bottom; else rect.Bottom = Math.Max(rect.Bottom, result.Bounds.Bottom);
            if (UserOptions.SnapLeft) rect.Left = result.Bounds.Left; else rect.Left = Math.Min(rect.Left, result.Bounds.Left);
            if (UserOptions.SnapRight) rect.Right = result.Bounds.Right; else rect.Right = Math.Max(rect.Right, result.Bounds.Right);
            LastRectangleDrawn = rect;
        }
    }

    //public async Task LoadProfiles()
    //{
    //    SelectedProfile = null;
    //    Profiles = new ObservableCollection<ProfileModel>();

    //    using var db = new BdoDbContext();
    //    var dbProfiles = await db.ProProfiles.ToListAsync();

    //    foreach (var dbProfile in dbProfiles)
    //    {
    //        var mProfile = new ProfileModel { Id = dbProfile.Id, Name = dbProfile.Name };
    //        Profiles.Add(mProfile);
    //        SelectedProfile ??= mProfile;
    //    }
    //}

    //public async Task LoadProfiles()
    //{

    //        var profile = new ProfileModel { Name = "Standard Statement" };
    //        profile.Templates.Add(new TemplateModel { Name = "Page" });
    //        mRoot.Profiles.Add(profile);

    //}
}
