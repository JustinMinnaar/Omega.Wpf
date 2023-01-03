namespace Jem.Profiling22.Data;

public class ProExtractionFieldText : IdNamed
{
    public int LineNumber { get; set; }
    public string? Lead { get; set; }
    public string? ValidCharacters { get; set; }
    public string? Follow { get; set; }
    public string? Sample { get; set; }
    public FieldFormat? Format { get; set; }
    public bool IsNegative { get; internal set; }
    public bool ReportOnNewLine { get; set; }

    public string Extract(string truth, int beginIndex, out int lastIndex, string? sample = null)
    {
        if (ValidCharacters != null)
            truth = truth.TakeAnyOf(ValidCharacters)!;

        var endIndex = truth.Length;
       

        if (Lead != null)
        {
            var leadIndex = truth.IndexOf(Lead);
            //if (leadIndex == -1)
            //    throw new TextMismatchDefiningProfile($"Could not find lead '{Lead}' in text '{truth}'");

            if (leadIndex >= 0)
                beginIndex = leadIndex + Lead.Length;
            //truth = truth.Substring(leadIndex + Lead.Length, truth.Length - Lead.Length - leadIndex).Trim();
        }

        if (Follow != null)
        {
            var followIndex = truth.IndexOf(Follow, beginIndex);
            if (followIndex != -1)
                endIndex = followIndex;
        }

        while (beginIndex < endIndex && truth[beginIndex] == ' ')
            beginIndex++;
        while (endIndex > 0 && truth[endIndex - 1] == ' ')
            endIndex--;

        if (beginIndex < endIndex)
            truth = truth.Substring(beginIndex, endIndex - beginIndex);
        else
            truth = String.Empty;



        lastIndex = endIndex;

        if (sample != null)
            if (truth.Replace(" ", "") != "" + Sample?.Replace(" ", ""))
                throw new TextMismatchDefiningProfile($"Found '{truth}' instead of '{sample}' for field '{Name}'!");

        return truth;
    }
}