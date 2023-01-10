using Jem.CommonLibrary22;

namespace Jem.DatabaseLibrary1;

public abstract class JemDbContext : DbContext
{
    protected const string defaultName = "(default)";

    #region UserSettings

    public DbSet<SysUserSettings> SysUserSettings { get; set; } = default!;
    //public DbSet<SysUserSelection> SysUserSelections { get; set; } = default!;

    public async Task<SysUserSettings> AccessSysUserSettings(string userName)
    {
        var user = await SysUserSettings.FirstOrDefaultAsync(u => u.Name == userName);
        if (user == null)
        {
            user = new SysUserSettings { Id = Guid.NewGuid(), Name = userName };
            SysUserSettings.Add(user);
        }
        return user;
    }

    #endregion

    #region Messages

    public DbSet<SysMessage> SysMessages { get; set; } = default!;

    #endregion

    #region Solutions

    public DbSet<DocSolution> DocSolutions { get; set; } = default!;

    public async Task<DocSolution> AccessSolutionAsync(string name) =>
        await TryGetSolutionAsync(name) ??
        await AddSolutionAsync(name);

    public async Task<DocSolution> AccessSolutionAsync(ID<DocSolution> id, string name) =>
        await TryGetSolutionAsync(id) ??
        await TryGetSolutionAsync(name) ??
        await AddSolutionAsync(id, name);

    public async Task<DocSolution> AddSolutionAsync(string name) =>
        await AddSolutionAsync(new ID<DocSolution>(), name);

    public async Task<DocSolution> AddSolutionAsync(ID<DocSolution> id, string name)
    {
        var row = new DocSolution
        {
            Id = id,
            Name = name,
        };

        DocSolutions.Add(row);
        await SaveChangesAsync();

        return row;
    }

    public async Task<DocSolution> GetSolution(ID<DocSolution> id) =>
        await TryGetSolutionAsync(id) ??
        throw new NotFoundException($"Solution {id} not found!");

    public async Task<DocSolution?> TryGetSolutionAsync(ID<DocSolution> id)
        => await DocSolutions.FindAsync(id.Guid);

    public async Task<DocSolution?> TryGetSolutionAsync(string name) =>
        await DocSolutions.FirstOrDefaultAsync(p => p.Name == name);

    #endregion

    #region Projects

    public DbSet<DocProject> DocProjects { get; set; } = default!;

    public async Task<DocProject> AccessProjectAsync(ID<DocSolution> solutionId, string name)=> 
        await TryGetProjectAsync(solutionId, name) ?? 
        await AddProjectAsync(solutionId, name);

    public async Task<DocProject> AccessProject(ID<DocSolution> solutionId, ID<DocProject> id, string name)=> 
        await TryGetProjectAsync(id) ?? 
        await TryGetProjectAsync(solutionId, name) ?? 
        await AddProjectAsync(solutionId, id, name);

    public async Task<DocProject> AddProjectAsync(ID<DocSolution> solutionId, string name) =>
        await AddProjectAsync(solutionId, new ID<DocProject>(), name);

    public async Task<DocProject> AddProjectAsync(ID<DocSolution> solutionId, ID<DocProject> id, string name)
    {
        var row = new DocProject
        {
            OwnerSolutionId = solutionId,
            Id = id,
            Name = name,
        };

        DocProjects.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<DocProject> GetProjectAsync(ID<DocProject> id) =>
        await TryGetProjectAsync(id) ??
        throw new NotFoundException($"Project {id} not found!");

    public async Task<DocProject?> TryGetProjectAsync(ID<DocProject> id) =>
        await DocProjects.FindAsync(id.Guid);

    public async Task<DocProject?> TryGetProjectAsync(ID<DocSolution> solutionId, string name) =>
        await DocProjects.FirstOrDefaultAsync(p => p.OwnerSolutionId == solutionId && p.Name == name);

    public void DropProjects(params DocProject[] rows) =>
        DocProjects.RemoveRange(rows);

    #endregion

    #region Folders

    public DbSet<DocFolder> DocFolders { get; set; } = default!;

    public async Task<DocFolder> AccessFolderAsync(ID<DocProject> ownerProjectId, string name) =>
        await TryGetFolderAsync(ownerProjectId, name) ??
        await AddFolderAsync(ownerProjectId, name);

    public async Task<DocFolder> AccessFolderAsync(ID<DocProject> ownerProjectId, ID<DocFolder> id, string name) =>
        await TryGetFolderAsync(id) ??
        await AddFolderAsync(ownerProjectId, id, name);

    public async Task<DocFolder> AddFolderAsync(ID<DocProject> ownerProjectId, string folderName)
        => await AddFolderAsync(ownerProjectId, new ID<DocFolder>(), folderName);

    public async Task<DocFolder> AddFolderAsync(ID<DocProject> ownerProjectId, ID<DocFolder> folderId, string folderName)
    {
        var row = new DocFolder
        {
            OwnerProjectId = ownerProjectId,
            Id = folderId,
            Name = folderName,
        };

        DocFolders.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<DocFolder> GetFolder(ID<DocFolder> id) =>
        await TryGetFolderAsync(id)
        ?? throw new NotFoundException($"Folder {id} not found!");

    public async Task<DocFolder?> TryGetFolderAsync(ID<DocFolder> id) =>
        await DocFolders.FindAsync(id);

    public async Task<DocFolder?> TryGetFolderAsync(ID<DocProject> projectId, string name) =>
        await DocFolders.FirstOrDefaultAsync(p => p.Name == name && p.OwnerProjectId == projectId);

    public void DropFolders(params DocFolder[] rows) =>
        DocFolders.RemoveRange(rows);

    #endregion

    #region Files

    public DbSet<DocFile> DocFiles { get; set; } = default!;

    public async Task<DocFile> AccessFileAsync(ID<DocFolder> ownerFolderId, string name) =>
        await TryGetFileAsync(ownerFolderId, name) ??
        await AddFileAsync(ownerFolderId, new(), name);

    public async Task<DocFile> AccessFileAsync(ID<DocFolder> ownerFolderId, ID<DocFile> id, string name) =>
        await TryGetFileAsync(id) ??
        await TryGetFileAsync(ownerFolderId, name) ??
        await AddFileAsync(ownerFolderId, id, name);

    public async Task<DocFile> AddFileAsync(ID<DocFolder> ownerFolderId, string fileName)
        => await AddFileAsync(ownerFolderId, new(), fileName);

    public async Task<DocFile> AddFileAsync(ID<DocFolder> ownerFolderId, ID<DocFile> fileId, string fileName)
    {
        var row = new DocFile
        {
            OwnerFolderId = ownerFolderId,
            Id = fileId,
            Name = fileName,
        };

        DocFiles.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<DocFile> GetFileAsync(ID<DocFile> id) =>
        await TryGetFileAsync(id) ??
        throw new NotFoundException($"File {id} not found!");

    public async Task<DocFile?> TryGetFileAsync(ID<DocFile> id) =>
        await DocFiles.FindAsync(id);

    public async Task<DocFile?> TryGetFileAsync(ID<DocFolder> folderId, string name) =>
        await DocFiles.FirstOrDefaultAsync(p => p.Name == name && p.OwnerFolderId == folderId);

    public void DropFiles(params DocFile[] rows) =>
        DocFiles.RemoveRange(rows);

    #endregion

    //#region Documents

    //public DbSet<DocDocument> DocDocuments { get; set; } = default!;

    //public DocDocument AddDocument(ID<DocFolder> ownerFolderId, string name, DocDocumentType? type = null) =>
    //    AddDocument(ownerFolderId, new ID<DocDocument>(), name, type);

    //public DocDocument AddDocument(ID<DocFolder> ownerFolderId, ID<DocDocument> id, string name, DocDocumentType? type = null)
    //{
    //    var row = new DocDocument
    //    {
    //        OwnerFolderId = ownerFolderId,
    //        Id = id,
    //        Name = name,
    //        Type = type ?? DefaultDocumentType,
    //    };

    //    DocDocuments.Add(row);
    //    return row;
    //}

    //public DocDocument GetDocument(ID<DocDocument> id) => TryGetDocument(id) ??
    //        throw new NotFoundException($"Document {id} not found!");

    //public DocDocument? TryGetDocument(ID<DocDocument> id)
    //{
    //    var document = DocDocuments.Find(id.Guid);
    //    return document;
    //}

    //public DocDocument? TryGetDocument(ID<DocFolder> ownerFolderId, string name)
    //{
    //    var document = DocDocuments.FirstOrDefault(d => d.Name == name && d.OwnerFolderId == ownerFolderId.Guid);
    //    return document;
    //}

    //public DocDocument AccessDocument(ID<DocFolder> ownerFolderId, string name, DocDocumentType? type = null)
    //{
    //    var document =
    //        TryGetDocument(ownerFolderId, name) ??
    //        AddDocument(ownerFolderId, name, type);
    //    return document;
    //}

    //public DocDocument AccessDocument(ID<DocFolder> ownerFolderId, ID<DocDocument> id, string name, DocDocumentType? type = null)
    //{
    //    var document =
    //        TryGetDocument(id) ??
    //        TryGetDocument(ownerFolderId, name) ??
    //        AddDocument(ownerFolderId, id, name, type);
    //    return document;
    //}

    //#endregion

    //#region DocumentsTypes

    //public DbSet<DocDocumentType> DocDocumentsTypes { get; set; } = default!;

    //public DocDocumentType AddDocumentType(string name) => AddDocumentType(new(), name);

    //public DocDocumentType AddDocumentType(ID<DocDocumentType> id, string name)
    //{
    //    var row = new DocDocumentType { Id = id, Name = name };
    //    DocDocumentsTypes.Add(row);
    //    return row;
    //}

    //private DocDocumentType? cachedDefaultDocumentType;

    //public ID<DocDocumentType> defaultDocDocumentTypeId = new(Guid.Empty);

    //public DocDocumentType DefaultDocumentType => cachedDefaultDocumentType ??=
    //    (TryGetDocumentType(defaultDocDocumentTypeId) ?? AddDocumentType(defaultDocDocumentTypeId, defaultName));

    //public DocDocumentType GetDocumentType(ID<DocDocumentType> id) => TryGetDocumentType(id) ??
    //        throw new NotFoundException($"DocumentType {id} not found!");

    //public DocDocumentType? TryGetDocumentType(ID<DocDocumentType> id) => DocDocumentsTypes.FirstOrDefault(p => p.Id == id);

    //public DocDocumentType? TryGetDocumentType(string name) => DocDocumentsTypes.FirstOrDefault(p => p.Name == name);

    //#endregion

    //#region DocumentsValues

    //public DbSet<DocValue> DocDocumentsValues { get; set; } = default!;

    //public DocValue AddDocumentValue(string name, string? value) => AddDocumentValue(new(), name, value);

    //public DocValue AddDocumentValue(ID<DocValue> id, string name, string? value)
    //{
    //    var row = new DocValue { Id = id, Name = name, Value = value };
    //    DocDocumentsValues.Add(row);
    //    return row;
    //}

    //public DocValue GetDocumentValue(ID<DocValue> id) => TryGetDocumentValue(id) ??
    //        throw new NotFoundException($"DocumentValue {id} not found!");

    //public DocValue? TryGetDocumentValue(ID<DocValue> id) => DocDocumentsValues.FirstOrDefault(p => p.Id == id);

    //public DocValue? TryGetDocumentValue(ID<DocDocument> docId, string name) =>
    //    DocDocumentsValues.FirstOrDefault(p => p.OwnerDocumentId == docId.Guid && p.Name == name);

    //#endregion

    #region Pages

    public DbSet<DocPage> DocPages { get; set; } = default!;

    public async Task<DocPage> AddPageAsync(ID<DocFile> ownerFileId, string name) =>
        await AddPageAsync(ownerFileId, new(), name);

    public async Task<DocPage> AddPageAsync(ID<DocFile> ownerFileId, ID<DocPage> id, string name)
    {
        var row = new DocPage
        {
            OwnerFileId = ownerFileId,
            Id = id,
            Name = name,
        };

        DocPages.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<DocPage> GetPageAsync(ID<DocPage> id) =>
        await TryGetPageAsync(id) ??
        throw new NotFoundException($"Page {id} not found!");

    public async Task<DocPage?> TryGetPageAsync(ID<DocPage> id) =>
        await DocPages.FindAsync(id.Guid);

    public async Task<DocPage?> TryGetPage(ID<DocFile> ownerFileId, string name) =>
        await DocPages.FirstOrDefaultAsync(d => d.Name == name && d.OwnerFileId == ownerFileId);

    public async Task<DocPage> AccessPageAsync(ID<DocFile> ownerFileId, ID<DocPage> id, string name) =>
        await TryGetPageAsync(id) ??
        await TryGetPage(ownerFileId, name) ??
        await AddPageAsync(ownerFileId, id, name);

    #endregion

    #region Images

    public DbSet<DocImage> DocImages { get; set; } = default!;

    public async Task<DocImage> AddImageAsync(DocPage ownerPage, string name) =>
        await AddImageAsync(ownerPage, new(), name);

    public async Task<DocImage> AddImageAsync(DocPage ownerPage, ID<DocImage> id, string name,
        int width = 0, int height = 0, string? checksum = null)
    {
        var row = new DocImage
        {
            OwnerPage = ownerPage,
            Id = id,
            Name = name,
            Width = width,
            Height = height,
            Checksum = checksum,
        };

        DocImages.Add(row);
        await SaveChangesAsync();
        return row;
    }

    public async Task<DocImage> GetImageAsync(ID<DocImage> id) =>
        await TryGetImageAsync(id) ?? 
        throw new NotFoundException($"Image {id} not found!");

    public async Task<DocImage?> TryGetImageAsync(ID<DocImage> id) =>
        await DocImages.FindAsync(id);

    public async Task<DocImage?>  TryGetImageAsync(DocPage ownerPage, string name) =>
        await DocImages.FirstOrDefaultAsync(d => d.Name == name && d.OwnerPage == ownerPage);

    public async Task<DocImage> AccessImageAsync(DocPage page, ID<DocImage> id, string name) =>
        await TryGetImageAsync(id) ??
        await TryGetImageAsync(page, name) ??
        await AddImageAsync(page, id, name);

    #endregion

    #region ctor

    private readonly string? _connectionString;

    public JemDbContext(string connectionString) => _connectionString = connectionString;

    public JemDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString != null)
            optionsBuilder.UseSqlServer(_connectionString);
    }

    #endregion

    #region model

    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);

        ConfigRoots(m);
        ConfigProjects(m);
        ConfigFolders(m);
        ConfigFiles(m);
        ConfigPages(m);
        //ConfigDocuments(m);

        ConfigSettings(m);
    }

    private static void ConfigRoots(ModelBuilder m)
    {
        m.Entity<DocSolution>()
            .HasMany<DocProject>(type => type.Projects)
            .WithOne(project => project.OwnerSolution)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigProjects(ModelBuilder m)
    {
        m.Entity<DocProject>()
            .HasMany<DocFolder>(project => project.Folders)
            .WithOne(folder => folder.OwnerProject)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigFolders(ModelBuilder m)
    {
        m.Entity<DocFolder>()
            .HasMany<DocFile>(folder => folder.Files)
            .WithOne(file => file.OwnerFolder)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //m.Entity<DocFolder>()
        //    .HasMany<DocDocument>(folder => folder.Documents)
        //    .WithOne(document => document.OwnerFolder)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigFiles(ModelBuilder m)
    {
        m.Entity<DocFile>()
            .HasMany<DocPage>(file => file.Pages)
            .WithOne(page => page.OwnerFile)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //m.Entity<DocFile>()
        //    .HasMany<DocDocument>(file => file.Values)
        //    .WithOne(document => document.OwnerFolder)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigPages(ModelBuilder m)
    {
        m.Entity<DocPage>()
            .HasMany<DocImage>(page => page.Images)
            .WithOne(image => image.OwnerPage)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    //private static void ConfigDocuments(ModelBuilder m)
    //{
    //    m.Entity<DocDocumentType>()
    //        .HasMany<DocDocument>(type => type.Documents)
    //        .WithOne(page => page.Type)
    //        .IsRequired()
    //        .OnDelete(DeleteBehavior.Cascade);

    //    m.Entity<DocDocument>()
    //        .HasMany<DocPage>(file => file.Pages)
    //        .WithOne(page => page.Document)
    //        .IsRequired()
    //        .OnDelete(DeleteBehavior.Cascade);

    //    m.Entity<DocDocument>()
    //        .HasMany<DocValue>(value => value.Values)
    //        .WithOne(value => value.OwnerDocument)
    //        .IsRequired()
    //        .OnDelete(DeleteBehavior.Cascade);
    //}

    private void ConfigSettings(ModelBuilder m)
    {
        m.Entity<SysUserSettings>()
            .HasOne<DocSolution>(settings => settings.SelectedDocSolution);

        //m.Entity<SysUserSettings>()
        //    .HasMany<SysUserSelection>(user => user.Selections)
        //    .WithOne(a => a.Settings)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion
}