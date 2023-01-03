using Jem.CommonLibrary22;

public abstract class JemDbContext : DbContext
{
    protected const string defaultName = "(default)";

    public DbSet<SysMessage> RootMessages { get; set; } = default!;

    #region Roots

    public DbSet<DocRoot> Root { get; set; } = default!;

    private DocRoot? _defaultRoot;

    // The root object once fetched from the database
    public DocRoot DefaultRoot
    {
        get
        {
            if (_defaultRoot != null) return _defaultRoot;

            _defaultRoot = Root.Find(Guid.Empty);
            if (_defaultRoot != null) return _defaultRoot;

            _defaultRoot = AddRoot(new ID<DocRoot>(Guid.Empty), defaultName);
            return _defaultRoot;
        }
    }

    private DocRoot AddRoot(ID<DocRoot> id, string name)
    {
        var root = new DocRoot { Id = id, Name = name };
        Root.Add(root);
        return root;
    }

    #endregion

    #region Projects

    public DbSet<DocProject> DocProjects { get; set; } = default!;

    public DocProject AccessProject(string name)
        => TryGetProject(name) ?? AddProject(name);

    public DocProject AccessProject(ID<DocProject> id, string name)
        => TryGetProject(id) ?? TryGetProject(name) ?? AddProject(id, name);

    public DocProject AddProject(string name) =>
        AddProject(new ID<DocProject>(), name);

    public DocProject AddProject(ID<DocProject> id, string name)
    {
        var row = new DocProject
        {
            Id = id,
            Name = name,
            OwnerRoot = DefaultRoot
        };

        DocProjects.Add(row);
        return row;
    }

    public DocProject GetProject(ID<DocProject> id) => TryGetProject(id) ??
            throw new NotFoundException($"Project {id} not found!");

    public DocProject? TryGetProject(ID<DocProject> id) => DocProjects.Find(id.Guid);

    public DocProject? TryGetProject(string name) =>
        DocProjects.FirstOrDefault(p => p.OwnerRoot == DefaultRoot && p.Name == name);

    #endregion
        
    #region Folders

    public DbSet<DocFolder> DocFolders { get; set; } = default!;

    public DocFolder AccessFolder(ID<DocProject> ownerProjectId, string name) =>
        TryGetFolder(ownerProjectId, name) ??
        AddFolder(ownerProjectId, name);

    public DocFolder AccessFolder(ID<DocProject> ownerProjectId, ID<DocFolder> id, string name) =>
        TryGetFolder(id) ??
        AddFolder(ownerProjectId, id, name);

    public DocFolder AddFolder(ID<DocProject> ownerProjectId, string folderName)
        => AddFolder(ownerProjectId, new ID<DocFolder>(), folderName);

    public DocFolder AddFolder(ID<DocProject> ownerProjectId, ID<DocFolder> folderId, string folderName)
    {
        var row = new DocFolder
        {
            OwnerProjectId = ownerProjectId,
            Id = folderId,
            Name = folderName,
        };
        DocFolders.Add(row);
        return row;
    }

    public DocFolder GetFolder(ID<DocFolder> id) =>
        TryGetFolder(id) ?? throw new NotFoundException($"Folder {id} not found!");

    public DocFolder? TryGetFolder(ID<DocFolder> id) => DocFolders.Find(id);

    public DocFolder? TryGetFolder(ID<DocProject> projectId, string name) =>
        DocFolders.FirstOrDefault(p => p.Name == name && p.OwnerProjectId == projectId);

    public void DropFolders(params DocFolder[] rows) => DocFolders.RemoveRange(rows);

    #endregion

    #region Files

    public DbSet<DocFile> DocFiles { get; set; } = default!;

    public DocFile AccessFile(ID<DocFolder> ownerFolderId, string name) =>
        TryGetFile(ownerFolderId, name) ??
        AddFile(ownerFolderId, new(), name);

    public DocFile AccessFile(ID<DocFolder> ownerFolderId, ID<DocFile> id, string name) =>
        TryGetFile(id) ??
        TryGetFile(ownerFolderId, name) ??
        AddFile(ownerFolderId, id, name);

    public DocFile AddFile(ID<DocFolder> ownerFolderId, string fileName)
        => AddFile(ownerFolderId, new(), fileName);

    public DocFile AddFile(ID<DocFolder> ownerFolderId, ID<DocFile> fileId, string fileName)
    {
        var row = new DocFile
        {
            OwnerFolderId = ownerFolderId,
            Id = fileId,
            Name = fileName,
        };
        DocFiles.Add(row);
        return row;
    }

    public DocFile GetFile(ID<DocFile> id) => 
        TryGetFile(id) ?? throw new NotFoundException($"File {id} not found!");

    public DocFile? TryGetFile(ID<DocFile> id) =>
        DocFiles.Find(id);

    public DocFile? TryGetFile(ID<DocFolder> folderId, string name) =>
        DocFiles.FirstOrDefault(p => p.Name == name && p.OwnerFolderId == folderId);

    public void DropFiles(params DocFile[] rows) =>
        DocFiles.RemoveRange(rows);

    #endregion

    #region //Documents

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

    #endregion

    #region //DocumentsTypes

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

    #endregion

    #region //DocumentsValues

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

    #endregion

    #region Pages

    public DbSet<DocPage> DocPages { get; set; } = default!;

    public DocPage AddPage(ID<DocFile> ownerFileId, string name) =>
        AddPage(ownerFileId, new(), name);

    public DocPage AddPage(ID<DocFile> ownerFileId, ID<DocPage> id, string name)
    {
        var row = new DocPage
        {
            OwnerFileId = ownerFileId,
            Id = id,
            Name = name,
        };

        DocPages.Add(row);
        return row;
    }

    public DocPage GetPage(ID<DocPage> id) =>
        TryGetPage(id) ?? throw new NotFoundException($"Page {id} not found!");

    public DocPage? TryGetPage(ID<DocPage> id) =>
        DocPages.Find(id.Guid);

    public DocPage? TryGetPage(ID<DocFile> ownerFileId, string name) =>
        DocPages.FirstOrDefault(d => d.Name == name && d.OwnerFileId == ownerFileId);

    public DocPage AccessPage(ID<DocFile> ownerFileId, ID<DocPage> id, string name) =>
        TryGetPage(id) ??
        TryGetPage(ownerFileId, name) ??
        AddPage(ownerFileId, id, name);

    #endregion

    #region Images

    public DbSet<DocImage> DocImages { get; set; } = default!;

    public DocImage AddImage(DocPage ownerPage, string name) =>
        AddImage(ownerPage, new(), name);

    public DocImage AddImage(DocPage ownerPage, ID<DocImage> id, string name,
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
        return row;
    }

    public DocImage GetImage(ID<DocImage> id) => 
        TryGetImage(id) ?? throw new NotFoundException($"Image {id} not found!");

    public DocImage? TryGetImage(ID<DocImage> id) =>
        DocImages.Find(id);

    public DocImage? TryGetImage(DocPage ownerPage, string name) =>
        DocImages.FirstOrDefault(d => d.Name == name && d.OwnerPage == ownerPage);

    public DocImage AccessImage(DocPage page, ID<DocImage> id, string name) => 
        TryGetImage(id) ??
        TryGetImage(page, name) ??
        AddImage(page, id, name);

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
    }

    private static void ConfigRoots(ModelBuilder m)
    {
        m.Entity<DocRoot>()
            .HasMany<DocProject>(type => type.Projects)
            .WithOne(project => project.OwnerRoot)
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
    //                    .HasMany<DocDocument>(type => type.Documents)
    //                    .WithOne(page => page.Type)
    //                    .IsRequired()
    //                    .OnDelete(DeleteBehavior.Cascade);

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

    #endregion

}