namespace Jem.DocDatabaseLibrary1;

public enum SysMessageLevel { Message, Warning, Error }

public sealed class SysMessage
{
    [Key]
    public int Id { get; set; }

    public SysMessageLevel Level { get; set; }

    public string? Message { get; set; }
}