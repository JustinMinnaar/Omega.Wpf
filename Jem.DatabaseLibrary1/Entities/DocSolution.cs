
/// <summary>Contains options and configuration for the solution.</summary>
public sealed class DocSolution : IdNamed
{

    public ICollection<DocProject> Projects { get; set; } = default!;
}
