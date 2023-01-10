namespace Jem.ProDatabaseLibrary1.Entities;

public sealed class ProBag : IdNamed
{
    public ICollection<ProGroup> Groups { get; set; } = default!;
    public Guid? SelectedGroupId { get; set; }
    public ProGroup? SelectedGroup { get; set; }
}
