using System;
using System.Collections.Generic;

namespace Omega.WpfModels1;

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