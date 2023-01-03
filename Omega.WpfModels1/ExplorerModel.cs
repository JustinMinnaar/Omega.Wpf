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

    public async Task LoadPages()
    {
        SelectedPage = null;
        Pages = new ObservableCollection<PageModel>();

        var mFile = SelectedFile;
        if (mFile == null) return;

        using var db = new BdoDbContext();
        var dbPages = await db.DocPages.Where(f => f.OwnerFileId == mFile.Id).OrderBy(p => p.Name).ToListAsync();

        foreach (var dbPage in dbPages)
        {
            var mPage = new PageModel { Id = dbPage.Id, Name = dbPage.Name };
            Pages.Add(mPage);
            SelectedPage ??= mPage;
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
