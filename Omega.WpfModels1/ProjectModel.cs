using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1;

public class ProjectModel : IdNamedModel
{
    public static ProjectModel From(DocProject dbProject)
    {
        return new ProjectModel { Id = dbProject.Id, Name = dbProject.Name };
    }
}

public class FolderModel : IdNamedModel
{
    public bool AreAnyFileError { get; set; }
    public bool AreAllFilesIdentified { get; set; }
}

public class FileModel : IdNamedModel
{
    public bool IsError { get; set; }
    public bool IsIdentified { get; set; }
}

public class PageModel : IdNamedModel
{
    public int PageIndex { get; set; }
    public bool? IsBlank { get; set; }
    public bool IsError { get; set; }
    public bool IsIdentified { get; set; }
    public Guid? ProfileId { get; set; }
    public string? ProfileName { get; set; }
    public float ProfileVersion { get; set; }

    public Dictionary<string, string> Values { get; set; } = new();
}