namespace Jem.Profiling22.Data;

using System.Text;

public class ProfilePhrase
{
    public CSize AllowMovement { get; set; }

    public ProfilePhrase()
    {

    }

    public bool IsMask { get; set; }

    public bool IsBlank { get; set; }

    public ProfilePhrase(OcrPage oPage, CRect rect, CSize allowMovement, string? mask = null, bool isBlank = false)
        : this(oPage.ExtractSymbolsAndText(rect)!, allowMovement, mask, isBlank)
    {
    }

    public ProfilePhrase(OcrPhrase result, CSize allowMovement, string? mask = null, bool isBlank = false)
    {
        this.AllowMovement = allowMovement;

        if (mask != null)
            IsMask = true;

        IsBlank = isBlank;
        if (!isBlank)
        {
            var i = 0;
            //Symbols.Clear();
            foreach (var symbol in result.Symbols)
            {
                var newSymbol = new OcrSymbol(symbol);
                if (IsMask)
                {
                    var maskCharacter = mask!.Substring(i, 1);
                    if ("ULA#*".Contains(maskCharacter))
                        newSymbol.Text = maskCharacter;
                    i++;
                }
                Symbols.Add(newSymbol);
            }
        }

        SearchRect = Bounds.Inflated(allowMovement);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var symbol in Symbols)
            sb.Append(symbol.Text);
        return sb.ToString();
    }

    public bool Matches(string truth)
    {
        truth = truth.Replace(" ", "");
        
        if (IsBlank && truth.Length > 0) return false;

        if (truth.Length > Symbols.Count)
            truth = truth[^Symbols.Count..];
        //if (truth.Length != Symbols.Count)
        //    return false;
        if (IsMask)
        {
            return MatchesMask(truth);
        }
        else
        {
            return MatchesText(truth);
        }

    }

    private bool MatchesMask(string truth)
    {
        if (truth.Length < Symbols.Count) return false;

        var i = 0;
        foreach (var symbol in Symbols)
        {
            var actual = truth[i++];
            if (!symbol.Matches(actual, isMask: true)) return false;
        }
        return true;
    }

    private bool MatchesText(string truth)
    {
        if (truth.Length < Symbols.Count) return false;

        var i = 0;
        foreach (var symbol in Symbols)
        {
            var expecting = symbol.Text[0];
            var actual = truth[i++];

            if (expecting != actual) return false;
        }
        return true;
    }


    /// <summary>The symbols that make up this phrase.</summary>
    public List<OcrSymbol> Symbols { get; set; } = new();

    /// <summary>The search area (relative to the template) in which to find the phrase.</summary>
    /// <remarks>This can be significantly larger than the actual rectangle containing the text, to allow for searching for movement.</remarks>
    public CRect SearchRect { get; set; }
    // public double? MinMatchPercentage { get; set; }

    // public CSize AllowMovement { get; }

    /// <summary>The bounds of the symbols rectangles.</summary>
    public CRect Bounds
    {
        get
        {
            var bounds = new CRect();
            foreach (var symbol in Symbols)
            {
                bounds = bounds.Union(symbol.Rect);
            }
            return bounds;
        }
    }

    internal void Compile()
    {
    }

}