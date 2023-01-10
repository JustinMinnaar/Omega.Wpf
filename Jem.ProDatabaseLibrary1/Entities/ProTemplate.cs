namespace Jem.ProDatabaseLibrary1.Entities;

public sealed class ProTemplate : IdNamed
{
    public Guid? OwnerProfileId { get; set; }
    public ProProfile? OwnerProfile { get; set; }
}


