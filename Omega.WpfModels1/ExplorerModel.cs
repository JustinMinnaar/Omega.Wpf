using Jem.OcrLibrary22;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Omega.WpfModels1;

// Projects, Folders, Files, Pages, Profiles: are loaded on demand
public class ExplorerModel : IdNamedModel
{

    public ObservableCollection<RootModel>? Roots { get; set; } = new();
    public RootModel? SelectedRoot { get; set; }

    public event EventHandler? SelectedRootChanged;
    protected virtual void OnSelectedRootChanged() => SelectedRootChanged?.Invoke(this, EventArgs.Empty);

    public ObservableCollection<ProjectModel>? Projects { get; set; } = new();
    public ProjectModel? SelectedProject { get; set; }

    public event EventHandler? SelectedProjectChanged;
    protected virtual void OnSelectedProjectChanged() => SelectedProjectChanged?.Invoke(this, EventArgs.Empty);

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

    public OcrDocument? oDocument { get; set; }
    public OcrPage? oPage { get; set; }

    public string? LastMessage { get; set; }

    public async Task LoadRootsAsync()
    {
        SelectedRoot = null;
        Roots = new ObservableCollection<RootModel>();

        using var db = new BdoDbContext();
        var dbRoots = await db.Root.OrderBy(r => r.Name).ToListAsync();

        foreach (var dbRoot in dbRoots)
        {
            var mRoot = new RootModel { Id = dbRoot.Id, Name = dbRoot.Name };
            Roots.Add(mRoot);
            SelectedRoot ??= mRoot;
        }
    }

    public async Task LoadProjectsAsync()
    {
        SelectedProject = null;
        Projects = new ObservableCollection<ProjectModel>();

        var mRoot = SelectedRoot;
        if (mRoot == null) return;

        using var db = new BdoDbContext();
        var dbProjects = await db.DocProjects.Where(p => p.OwnerRootId == mRoot.Id).OrderBy(p => p.Name).ToListAsync();

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
        Folders = new ObservableCollection<FolderModel>();

        var mProject = SelectedProject;
        if (mProject == null) return;

        using var db = new BdoDbContext();
        var dbFolders = await db.DocFolders.Where(f => f.OwnerProjectId == mProject.Id).OrderBy(f => f.Name).ToListAsync();

        foreach (var dbFolder in dbFolders)
        {
            var mFolder = new FolderModel { Id = dbFolder.Id, Name = dbFolder.Name };
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
        var dbFiles = await db.DocFiles.Where(f => f.OwnerFolderId == mFolder.Id).OrderBy(f => f.Name).ToListAsync();

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

        async Task<string> DoLoadOcr()
        {
            oDocument = null;

            var mProject = SelectedProject; if (mProject == null) return "No project selected.";
            var mFolder = SelectedFolder; if (mFolder == null) return "No folder selected.";
            var mFile = SelectedFile; if (mFile == null) return "No file selected.";

            var rootPath = @"F:\Bdo";
            var projectName = mProject.Name;
            var folderName = mFolder.Name;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(mFile.Name);

            var binOcrFilePath = $@"{rootPath}\{projectName}\{folderName}\{fileName}.bocr";
            oDocument = await OcrDocument.TryLoadFromBinaryFileAsync(binOcrFilePath);
            if (oDocument == null) return "No Ocr available.";

            oDocument.ResizeTo2100();

            return "Loaded ocr.";
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

        if (oDocument != null)
        {
            foreach (var oPage in oDocument.Pages)
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
        oPage = null;

        if (SelectedPage == null) return;

        if (oDocument == null) return;

        oPage = oDocument.Pages.FirstOrDefault(p => p.PageIndex == SelectedPage.PageIndex);
        if (oPage == null) return;

        LastMessage = $"Page {(oPage.PageIndex + 1)} contains {oPage.SymbolsCount} symbols.";
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
