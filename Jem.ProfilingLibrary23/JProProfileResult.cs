using Jem.CommonLibrary22;
using Jem.ProfilingLibrary23;

using System.Runtime.Versioning;

namespace Jem.Profiling22;

public class JProProfileResult : JProTemplateResult
{
    public override string ToString()
    {
        return $"PageIndex:{PageIndex} Profile:'{Profile?.Name}' {base.ToString()}";
    }

    public JProFieldResult? TryFindFieldResult(string name)
        => TryFindFieldResult(name, name);

    public JProFieldResult? TryFindFieldResult(string extractorName, string fieldName)
    {
        foreach (var tResult in TemplateResults)
        {
            foreach (var rResult in tResult.RowResults)
            {
                var r = rResult.TryFindFieldResult(extractorName, fieldName);
                if (r != null) return r;
            }
        }
        return null;
    }

    public string? DocumentPath { get; set; }
    public int PageIndex { get; set; }
    public JProProfile? Profile { get; set; }

    public bool IsBlank { get; set; }
    public List<JProTemplateResult> TemplateResults { get; set; } = new();

    public List<RowResult> HeaderRows => EnumerateHeaderRows().ToList();
    IEnumerable<RowResult> EnumerateHeaderRows()
    {
        foreach (var tResult in TemplateResults)
        {
            if (tResult.Template?.Type == ProfileTemplateType.LineL) continue;
            if (tResult.Template?.Type == ProfileTemplateType.LineR) continue;
            foreach (var rResult in tResult.RowResults)
            {
                yield return rResult;
            }
        }
    }

    public List<RowResult> LineRows => EnumerateLineRows().ToList();
    IEnumerable<RowResult> EnumerateLineRows()
    {
        foreach (var tResult in TemplateResults)
        {
            if (tResult.Template?.Type != ProfileTemplateType.LineL) continue;
            foreach (var rResult in tResult.RowResults)
            {
                yield return rResult;
            }
        }

        foreach (var tResult in TemplateResults)
        {
            if (tResult.Template?.Type != ProfileTemplateType.LineR) continue;
            foreach (var rResult in tResult.RowResults)
            {
                yield return rResult;
            }
        }
    }

    public int CountHeaderRowsExtracted { get; set; }
    public int CountLineRowsExtracted { get; set; }

    public double? LinesTop { get; set; }
    public double? LinesBottom { get; set; }
}