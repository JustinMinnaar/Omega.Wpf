using System.Collections.ObjectModel;

namespace Omega.WpfModels1;

public class ProjectModel : IdNamedModel
{
    public string? Path { get; set; }

    public static ProjectModel From(DocProject dbProject)
    {
        return new ProjectModel { Id = dbProject.Id, Name = dbProject.Name, Path = dbProject.Path };
    }
}
