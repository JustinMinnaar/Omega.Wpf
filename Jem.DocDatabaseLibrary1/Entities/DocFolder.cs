namespace Jem.DocDatabaseLibrary1;

public sealed class DocFolder : BaseEntity
{
    public Guid? OwnerProjectId { get; set; }
    public DocProject? OwnerProject { get; set; }

    public ICollection<DocFile> Files { get; set; } = default!;
    public Guid? SelectedDocFileId { get; set; }
    public DocFile? SelectedDocFile { get; set; }

    //public ICollection<DocDocument> Documents { get; set; } = default!;
    //public Guid? SelectedDocDocumentId { get; set; }
    //public DocDocument? SelectedDocDocument { get; set; }

    /// <summary>Have we loaed the files (from the client)? If not, we will scan the project folder and load them.</summary>
    public bool AreFilesLoaded { get; set; }

    /// <summary>Are any of the files in error?</summary>
    public bool AreAnyFileError { get; set; }

    /// <summary>Were all files identified?</summary>
    public bool AreAllFilesIdentified { get; set; }
}
