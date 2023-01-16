namespace Jem.DocDatabaseLibrary1;

public sealed class DocPage : IdNamed
{
    public ID<DocPage> GetId() => new(this.Id);

    public int PageIndex { get; set; }

    public Guid? OwnerFileId { get; set; }
    public DocFile? OwnerFile { get; set; }

    //public Guid? DocumentId { get; set; }
    //public DocDocument? Document { get; set; }

    //public Guid? TypeId { get; set; }
    //public DocPageType? Type { get; set; }

    /// <summary>A count of the symbols on this page. The symbols might not be stored but the last count of symbols can be useful.</summary>
    public int? SymbolCount { get; set; }
    /// <summary>Binary encoded symbol data.</summary>
    public byte[]? SymbolData { get; set; }

    /// <summary>A count of the images on this page. The images might not be stored but the last count of images can be useful.</summary>
    public int? ImageCount { get; set; }
    public ICollection<DocImage> Images { get; set; } = default!;
    
    public bool? IsBlank { get; set; }
    public bool IsError { get; set; }
    public Guid? ProfileId { get; set; }
    public string? ProfileName { get; set; }
    public float ProfileVersion { get; set; }
}
