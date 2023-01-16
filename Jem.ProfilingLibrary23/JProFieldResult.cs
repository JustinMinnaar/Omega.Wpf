namespace Jem.ProfilingLibrary23;

public class JProFieldResult
{
    public JProFieldResult(JProExtractor extractor, ProExtractionFieldText field,
        CRect rect, string value, int beginIndex, int endIndex)
    {
        Extractor = extractor;
        Field = field;
        Value = value;
        Rect = rect;
        BeginIndex = beginIndex;
        EndIndex = endIndex;
    }

    public override string ToString() => $"{Field.Name}='{Value}' ";

    public JProExtractor Extractor { get; }
    public ProExtractionFieldText Field { get; }
    public string Value { get; set; }
    public CRect Rect { get; set; }
    public int BeginIndex { get; }
    public int EndIndex { get; }
    public CTheory? Theory { get; set; }
}

