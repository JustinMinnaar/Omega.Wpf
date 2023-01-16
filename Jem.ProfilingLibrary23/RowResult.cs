using Jem.ProfilingLibrary23;

using System.Text;

namespace Jem.Profiling22;

public class RowResult
{
    public RowResult(JProTemplate jTemplate, string? pageNumber, int rowNumber, bool isSkip)
    {
        JTemplate = jTemplate;
        PageNumber = pageNumber;
        RowNumber = rowNumber;
        IsSkip = isSkip;
    }

    public string ToStringParts()
    {
        var sb = new StringBuilder();
        var lead = $" {PageNumber,3}/{RowNumber:00}";
        sb.Append(lead);
        foreach (var eResult in ExtractorResults)
        {
            foreach (var fResult in eResult.FieldResults)
            {
                sb.Append(" '");
                sb.Append(fResult.Value);
                sb.Append("'");
            }
        }
        return sb.ToString();
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        // Template='{Template.Name}' 
        // sb.Append($"{RowNumber:00} ");
        var lead = $" {PageNumber,3}/{RowNumber:00} Template='{this.JTemplate.Name}' ";
        sb.Append(lead);
        var space = new string(' ', lead.Length);
        foreach (var eResult in ExtractorResults)
        {
            foreach (var fResult in eResult.FieldResults)
            {
                sb.Append(fResult.ToString());
                if (fResult.Field.ReportOnNewLine) { sb.AppendLine(); sb.Append(space); }
            }
        }
        return sb.ToString();
    }

    public string? PageNumber { get; }
    public int RowNumber { get; }
    public bool IsSkip { get; }
    public List<JProExtractorResult> ExtractorResults { get; } = new();
    public JProTemplate JTemplate { get; }

    public JProExtractorResult? TryFindExtractorResult(string name) =>
        ExtractorResults.FirstOrDefault(e => e.Extractor.Name == name);

    public JProFieldResult? TryFindFieldResult(string name) =>
        TryFindFieldResult(name, name);

    public JProFieldResult? TryFindFieldResult(string extractorName, string fieldName)
    {
        foreach (var eResult in ExtractorResults)
        {
            if (eResult.Extractor.Name != extractorName) continue;

            var fResult = eResult.TryFindFieldResult(fieldName);
            if (fResult != null) return fResult;
        }
        return null;
    }


}