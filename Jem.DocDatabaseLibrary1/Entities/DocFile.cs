namespace Jem.DocDatabaseLibrary1;

public sealed class DocFile : BaseEntity
{
    public Guid? OwnerFolderId { get; set; }
    public DocFolder? OwnerFolder { get; set; }

    public long? Size { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public string? OcrEngine { get; set; }
    public DateTime? OcrDate { get; set; }

    public ICollection<DocPage> Pages { get; set; } = default!;
    public Guid? SelectedDocPageId { get; set; }
    public DocPage? SelectedDocPage { get; set; }

    /// <summary>The profile that identified this page.</summary>
    public Guid? ProfileId { get; set; }
    public string? ProfileName { get; set; }
    public bool IsIdentified { get; set; }

    //public ICollection<DocValue> Values { get; set; } = default!;
}
