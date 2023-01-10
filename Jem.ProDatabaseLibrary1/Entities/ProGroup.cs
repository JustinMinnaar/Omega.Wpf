namespace Jem.ProDatabaseLibrary1.Entities;

public sealed class ProGroup : IdNamed
{
    public Guid? OwnerBagId { get; set; }
    public ProBag? OwnerBag{ get; set; }

    public ICollection<ProProfile> Profiles { get; set; } = default!;
    public Guid? SelectedProfileId { get; set; }
    public ProProfile? SelectedProfile { get; set; }

}
