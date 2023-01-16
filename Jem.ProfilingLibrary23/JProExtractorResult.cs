namespace Jem.ProfilingLibrary23;

public class JProExtractorResult
{
    public JProExtractorResult(JProExtractor extractor, int rowNumber, CRect rect, string text, List<OcrSymbol> symbols, CTheory? theory)
    {
        Extractor = extractor;
        RowNumber = rowNumber;
        Rect = rect;
        Text = text;
        Symbols = symbols;
        Theory = theory;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        ToString(sb);
        return sb.ToString();
    }

    public void ToString(StringBuilder sb)
    {
        sb.Append(Extractor.Name);
        sb.Append(" (");
        foreach (var fieldResult in FieldResults)
        {
            sb.Append(fieldResult.Field.Name);
            sb.Append("='");
            sb.Append(fieldResult.Value);
            sb.Append("' ");
        }
        sb.Append(") ");
    }

    public JProExtractor Extractor { get; }
    public int RowNumber { get; }
    public CRect Rect { get; }
    public string Text { get; }
    public List<OcrSymbol> Symbols { get; }
    public List<JProFieldResult> FieldResults { get; } = new();
    public CTheory? Theory { get; set; }

    public JProFieldResult FindFieldResult(string name) => TryFindFieldResult(name) ??
            throw new RowException($"Can't find field extractor for '{name}'!");

    public JProFieldResult? TryFindFieldResult(string name) =>
        FieldResults.FirstOrDefault(f => f.Field.Name == name);

}