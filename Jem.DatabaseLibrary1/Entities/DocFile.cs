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
    public Guid? ProfileId { get; set; }
    public string? ProfileName { get; set; }
    public bool IsIdentified { get; set; }
}
