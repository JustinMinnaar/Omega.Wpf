namespace Jem.Profiling22;

public class ExtractedText
{
    public ExtractedText(ProTemplate template, ProExtractor extractor, string text, CRect rect)
    {
        Template = template;
        Extractor = extractor;
        Text = text;
        Rect = rect;
    }

    public override string ToString() => $"{Template.Name}-{Extractor.Name}='{Text}' ({Rect})";

    public ProTemplate Template { get; set; }
    public ProExtractor Extractor { get; set; }
    public string Text { get; set; }
    public CRect Rect { get; set; }
}

