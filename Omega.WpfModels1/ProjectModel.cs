using Jem.CommonLibrary22;
using Jem.DocDatabaseLibrary1;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1;

public class ProjectModel : IdNamedModel
{
    public string? Path { get; set; }

    public string? WorkingFolderPath { get; set; }
    public string? ImportFolderPath { get; set; }
    public string? ImportExcludeWildcards { get; set; }
    public bool ImportFilesInSubFolders { get; set; }
    public int ImportProgressPercentage { get; set; }

    public IEnumerable<DocProjectImportStatus> ImportStatuses
    {
        get
        {
            yield return DocProjectImportStatus.New;
            yield return DocProjectImportStatus.Busy;
            yield return DocProjectImportStatus.Completed;
        }
    }
    public DocProjectImportStatus ImportStatus { get; set; }

    public string? ImportResult { get; set; }

    public static ProjectModel From(DocProject dbProject)
    {
        return new ProjectModel
        {
            Id = dbProject.Id,
            Name = dbProject.Name,
            Path = dbProject.ImportFolderPath,
            WorkingFolderPath = dbProject.WorkingFolderPath,
            ImportFolderPath = dbProject.ImportFolderPath,
            ImportExcludeWildcards = dbProject.ImportExcludeWildcards,
            ImportFilesInSubFolders = dbProject.ImportFilesInSubFolders,
            ImportStatus = dbProject.ImportStatus,
            ImportProgressPercentage = dbProject.ImportProgressPercentage,
        };
    }
}
