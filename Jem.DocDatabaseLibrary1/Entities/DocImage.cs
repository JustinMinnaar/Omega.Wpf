namespace Jem.DocDatabaseLibrary1;

public sealed class DocImage : IdNamed
{
    public ID<DocImage> GetId() => new(this.Id);

    public Guid? OwnerPageId { get; set; }
    public DocPage? OwnerPage { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    [MaxLength(200)]
    public string? Checksum { get; set; }
}
