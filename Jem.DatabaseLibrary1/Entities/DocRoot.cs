/// <summary>Contains options and configuration for the entire system.</summary>
public sealed class DocRoot : IdNamed
{
    public Guid? SelectedProjectId { get; set; }

    public DocProject? SelectedProject { get; set; }

    public ICollection<DocProject> Projects { get; set; } = default!;
}