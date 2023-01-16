using Bdo.DatabaseLibrary1;

using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

using Microsoft.EntityFrameworkCore;

using Omega.WpfModels1;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Omega.WpfControllers1;

public class DocumentsController : CNotifyPropertyChanged
{
    #region class

    [SetsRequiredMembers]
    public DocumentsController(MainController main)
    {
        this.Main = main;
    }

    public required MainController Main { get; init; }

    #endregion

    public string? LastMessage { get; set; }

    // Projects, Folders, Files, Pages, Profiles: are loaded on demand

    #region Solutions

    public ObservableCollection<SolutionModel>? Solutions { get; set; } = new();
    public SolutionModel? SelectedSolution { get; set; }

    public event EventHandler? SelectedSolutionChanged;
    protected virtual void OnSelectedSolutionChanged() => SelectedSolutionChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region Projects

    public ObservableCollection<ProjectModel>? Projects { get; set; } = new();
    public ProjectModel? SelectedProject { get; set; }

    public event EventHandler? SelectedProjectChanged;
    protected virtual void OnSelectedProjectChanged() => SelectedProjectChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region IdentifiedFilters 

    public static IdentifiedFilter[] IdentifiedFilters => (IdentifiedFilter[])Enum.GetValues(typeof(IdentifiedFilter));
    public IdentifiedFilter? SelectedIdentifiedFilter { get; set; }

    public event EventHandler? SelectedIdentifiedFilterChanged;
    protected virtual void OnSelectedIdentifiedFilterChanged() => SelectedIdentifiedFilterChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region Folders

    public ObservableCollection<FolderModel>? Folders { get; set; } = new();
    public FolderModel? SelectedFolder { get; set; }

    public event EventHandler? SelectedFolderChanged;
    protected virtual void OnSelectedFolderChanged() => SelectedFolderChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region Files

    public ObservableCollection<FileModel>? Files { get; set; } = new();
    public FileModel? SelectedFile { get; set; }

    public event EventHandler? SelectedFileChanged;
    protected virtual void OnSelectedFileChanged() => SelectedFileChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region Pages

    public ObservableCollection<PageModel>? Pages { get; set; } = new();
    public PageModel? SelectedPage { get; set; }

    public event EventHandler? SelectedPageChanged;
    protected virtual void OnSelectedPageChanged() => SelectedPageChanged?.Invoke(this, EventArgs.Empty);

    #endregion
    #region Ocr

    public OcrDocument? ODocument { get; set; }
    public OcrPage? OPage { get; set; }

    #endregion

    public async Task<SolutionModel> AccessSolutionAsync(string solutionName)
    {
        using var db = new BdoDocDbContext();

        var dbSolution = await db.AccessSolutionAsync(solutionName);

        var mSolution = new SolutionModel { Id = dbSolution.Id, Name = dbSolution.Name };
        SelectedSolution = mSolution;
        return mSolution;
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

        using var db = new BdoDocDbContext();
        var dbSolutions = await db.DocSolutions.OrderBy(r => r.Name).ToListAsync();

        // If no previous user selection matches, we'll select the first one
        var dbSelected = dbSolutions.FirstOrDefault(s => s.Id == Main.Settings?.SelectedDocSolutionId) ?? dbSolutions.FirstOrDefault();

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

        using var db = new BdoDocDbContext();
        var dbProjects = await db.DocProjects.Where(p => p.OwnerSolutionId == mRoot.Id).OrderBy(p => p.Name).ToListAsync();

        foreach (var dbProject in dbProjects)
        {
            var mProject = ProjectModel.From(dbProject);
            Projects.Add(mProject);
            SelectedProject ??= mProject;
        }
    }

    public async Task LoadFoldersAsync()
    {
        SelectedFolder = null;
        SelectedFile = null;
        SelectedPage = null;

        Folders = new ObservableCollection<FolderModel>();

        var mProject = SelectedProject;
        if (mProject == null) return;

        using var db = new BdoDocDbContext();

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

    public async Task LoadFilesAsync()
    {
        SelectedFile = null;
        Files = new ObservableCollection<FileModel>();

        var mFolder = SelectedFolder;
        if (mFolder == null) return;

        using var db = new BdoDocDbContext();

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

    public async Task AfterFileChangedAsync()
    {
        await LoadOcrAsync();
        await LoadPagesAsync();
    }

    public async Task LoadOcrAsync()
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

    private async Task LoadPagesAsync()
    {
        SelectedPage = null;
        Pages = new ObservableCollection<PageModel>();

        var mFile = SelectedFile; if (mFile == null) return;

        using var db = new BdoDocDbContext();
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
            if (Main.Settings.SnapTop) rect.Top = result.Bounds.Top; else rect.Top = Math.Min(rect.Top, result.Bounds.Top);
            if (Main.Settings.SnapBottom) rect.Bottom = result.Bounds.Bottom; else rect.Bottom = Math.Max(rect.Bottom, result.Bounds.Bottom);
            if (Main.Settings.SnapLeft) { var w = result.Bounds.Left - rect.Left; rect.Width -= w; rect.Left += w; } else rect.Left = Math.Min(rect.Left, result.Bounds.Left);
            if (Main.Settings.SnapRight) rect.Right = result.Bounds.Right; else rect.Right = Math.Max(rect.Right, result.Bounds.Right);
            LastRectangleDrawn = rect;
        }
    }

}
