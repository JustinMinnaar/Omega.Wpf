public sealed class DocFolder : BaseEntity
{
    public Guid? OwnerProjectId { get; set; }
    public DocProject? OwnerProject { get; set; }

    public ICollection<DocFile> Files { get; set; } = default!;

    public ICollection<DocDocument> Documents { get; set; } = default!;
    public bool AreFilesLoaded { get; set; }
}
