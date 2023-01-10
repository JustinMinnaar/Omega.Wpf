namespace Jem.ProDatabaseLibrary1.Entities;

public sealed class ProProfile : IdNamed
{
    public Guid? OwnerGroupId { get; set; }
    public ProGroup? OwnerGroup { get; set; }

    public ICollection<ProTemplate> Templates { get; set; } = default!;
    public Guid? SelectedTemplateId { get; set; }
    public ProTemplate? SelectedTemplate { get; set; }

}
