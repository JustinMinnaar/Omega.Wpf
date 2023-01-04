namespace Jem.Profiling22.Data;

public class ProIdentifier : IdNamed
{
    public ProIdentifier(OcrPage oPage, CSize maxMoveDistance, int? minPageNumber = null, int? maxPageNumber = null, float maxSymbolHeight = OcrPage.DEFAULT_MAX_SYMBOL_HEIGHT)
    {
        OPage = oPage;
        MaxMoveDistance = maxMoveDistance;
        MinPageNumber = minPageNumber;
        MaxPageNumber = maxPageNumber;
        MaxSymbolHeight = maxSymbolHeight;
    }

    public string ToText()
    {
        var sb = new StringBuilder();
        ToText(sb); 
        return sb.ToString();
    }

    private void ToText(StringBuilder sb)
    {
        foreach (var phrase in Phrases)
        {
            if (sb.Length > 0)
                sb.Append(' ');

            sb.Append(phrase.ToString());
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        ToText(sb);
        sb.Append($" MaxMoveDistance={MaxMoveDistance}");
        if (MinPageNumber != null)
            sb.Append($" MinPageNumber={MinPageNumber}");
        if (MaxPageNumber != null)
            sb.Append($" MaxPageNumber={MaxPageNumber}");
        return sb.ToString();
    }

    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var phrase in Phrases)
            {
                bounds = bounds.Union(phrase.Bounds);
            }
            return bounds;
        }
    }

    /// <summary>Creates a manual identification record by adding symbols for the text in the rectangle specified, and allowing movement of this text within the range specified.</summary>
    /// <param name="rectSample">The rectangle to search for the text phrase inside the original file.</param>
    /// <param name="sample"></param>
    /// <param name="maxMovement"></param>
    /// <returns>The phrase created for identification.</returns>
    /// <exception cref="TextMismatchDefiningProfile">The text expected on the ocr page when creating this phrase.</exception>
    public ProIdentifier AddPhrase(double x, double y, double w, double h,
        string? sample = null, string? mask = null, bool isBlank = false)
        => AddPhrase(new CRect(x, y, w, h), sample, mask, isBlank);

    public ProIdentifier AddPhrase(CRect rectSample,
        string? sample = null, string? mask = null, bool isBlank = false)
    {
        var result = new OcrPhrase();
        if (!isBlank) result = OPage!.ExtractSymbolsAndText(rectSample, maxSymbolHeight: MaxSymbolHeight);
        if (result == null) throw new TextMismatchDefiningProfile($"Found no symbols instead of '{sample}' for identifier on template '{this}'!");

        // var result = (isBlank ? null : OPage!.ExtractSymbolsAndText(rectSample)) ?? new OcrSymbols();
        if (mask != null && result.Symbols.Count != mask.NoSpaces().Length)
            throw new TextMismatchDefiningProfile($"Found '{result.Symbols.Count}' symbols instead of '{mask.Length}' symbols for mask '{mask}' for identifier on template '{this}'!");

        var phrase = new ProfilePhrase(result, MaxMoveDistance, mask, isBlank);

        if (result.IsEmpty)
            phrase.SearchRect = rectSample.Inflated(MaxMoveDistance);

        if (sample != null)
        {
            var truth = result.Text; // OPage!.ExtractText(rectSample) ?? string.Empty;
            if (!isBlank && !phrase.Matches(sample))
                throw new TextMismatchDefiningProfile($"Found '{truth}' instead of '{sample}' for identifier on template '{this}'!");
        }

        Phrases.Add(phrase);
        return this;
    }

    internal virtual void Compile()
    {
        foreach (var phrase in Phrases) phrase.Compile();
        OPage = null;
    }

    public List<ProfilePhrase> Phrases { get; set; } = new();
    public OcrPage? OPage { get; set; }
    public CSize MaxMoveDistance { get; set; }
    public double MinSuccessScore { get; }

    public int? MinPageNumber { get; set; }
    public int? MaxPageNumber { get; set; }
    public float MaxSymbolHeight { get; set; }
}