using System;
using System.Collections.ObjectModel;

namespace Omega.WpfModels1
{
    public class ProjectModel : IdNamedModel
    {

    }

    public class FolderModel : IdNamedModel
    {
    }

    public class FileModel : IdNamedModel
    {
    }

    public class PageModel : IdNamedModel
    {
        public int PageIndex { get; set; }
        public bool? IsBlank { get; set; }
        public bool IsError { get; set; }
        public Guid? ProfileId { get; set; }
        public string? ProfileName { get; set; }
        public float ProfileVersion { get; set; }
    }
}