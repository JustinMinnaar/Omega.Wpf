public sealed class DocFolder : BaseEntity
{
    public Guid? OwnerProjectId { get; set; }
    public DocProject? OwnerProject { get; set; }

    public ICollection<DocFile> Files { get; set; } = default!;

    public ICollection<DocDocument> Documents { get; set; } = default!;
    public bool AreFilesLoaded { get; set; }

    /// <summary>Are any of the files in error?</summary>
    public bool AreAnyFileError { get; set; }

    /// <summary>Were all files identified?</summary>
    public bool AreAllFilesIdentified { get; set; }
}
