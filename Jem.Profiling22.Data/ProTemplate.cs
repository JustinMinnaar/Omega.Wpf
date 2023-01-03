using System.Drawing;

namespace Jem.Profiling22.Data;

public enum ProfileTemplateType { Page, Section, Header, Footer, LineL, LineR }

/// <summary>Defines identification of a page, and extraction of data from the page.</summary>
public class ProTemplate : IdNamed
{

    #region class

    public ProTemplate(string name)
    {
        Name = name;
    }

    public ProTemplate(ProProfile profile, string name, OcrPage? sourcePage, ProfileTemplateType type, CSize? maxMovement = null)
    {
        Profile = profile;
        SourcePage = sourcePage;
        Name = name;
        Type = type;
        MaxMovement = maxMovement ?? new();
    }

    #endregion

    #region Properties

    public ProProfile? Profile { get; set; }
    public OcrPage? SourcePage { get; set; }
    public ProfileTemplateType Type { get; }
    public bool IsRequired { get; set; }
    public ProTemplate? RelativeTo { get; set; }
    public bool RelativeToMustExists { get; set; }

    public float DefaultMaxSymbolHeight = 40f;

    /// <summary>The maximum distance this template can move relative to the page.</summary>
    public CSize MaxMovement { get; set; }
    public bool IsSkip { get; set; }
    public CRect Rect { get; set; }

    /// <summary>the number of rows that can exist for this template.</summary>
    // public int MaxLineCount { get; set; } = 1;
    public double? LinesTop { get; set; }
    public double? LinesBottom { get; set; }
    public double? LinesHeight { get; set; }
    //public CRect RepeatRect { get; set; }
    //public int RepeatMaxCount { get; set; } = 1;
    public int? ExpectedMinimumPageNumber { get; set; }
    public int? ExpectedMaximumPageNumber { get; set; }
    public bool IsEndOfPage { get; set; }
    public bool IsOptional { get; set; }

    #endregion

    #region Identifiers

    /// <summary>The identifiers that are required to align and identify this template.</summary>
    public List<ProIdentifier> Identifiers { get; set; } = new();

    public ProIdentifier AddIdentifier(double x, double y, double w, double h, CSize allowMovement,
        string? sample = null, string? mask = null, bool isBlank = false, float? maxSymbolHeight = null)
    {
        var identifier = AddIdentifier(allowMovement, maxSymbolHeight ?? DefaultMaxSymbolHeight);
        identifier.AddPhrase(new CRect(x, y, w, h), sample, mask, isBlank);
        return identifier;
    }

    public ProIdentifier AddIdentifier(CSize allowMovement, float? maxSymbolHeight = null)
        => AddIdentifier(SourcePage!, allowMovement, maxSymbolHeight: maxSymbolHeight);

    public ProIdentifier AddIdentifier(OcrPage oPage, CSize allowMovement, int? minPageNumber = 0, int? maxPageNumber = 0, float? maxSymbolHeight = null)
    {
        var identifier = new ProIdentifier(oPage, allowMovement, minPageNumber, maxPageNumber, maxSymbolHeight ?? DefaultMaxSymbolHeight);
        Identifiers.Add(identifier);
        return identifier;
    }

    #endregion

    #region Extractors

    /// <summary>The text extraction areas to be processed if the template matches.</summary>
    public List<ProExtractor> Extractors { get; set; } = new();
    public bool IsStartOfLines { get; set; }

    public ProExtractor AddExtractor(string name, double x, double y, double w, double h, Snaps snaps, bool isblock = false, string? validCharacters=null, float? maxSymbolHeight = null, bool isRequired = false)
        => AddExtractor(name, new CRect(x, y, w, h), snaps, isblock, validCharacters, maxSymbolHeight, isRequired);

    public ProExtractor AddExtractor(CRect rect, Snaps snaps, bool isblock = false, string? validCharacters = null,float? maxSymbolHeight = null, bool isRequired = false)
        => AddExtractor(string.Empty, rect, snaps, isblock, validCharacters, maxSymbolHeight, isRequired);

    public ProExtractor AddExtractor(string name, CRect rect, Snaps snaps, bool isblock = false, 
        string? validCharacters = null, float? maxSymbolHeight = null, bool isRequired = false  )
    {
        // Snap the left, top, right, and bottom of the rect if needed
        var symbols = SourcePage!.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight ?? DefaultMaxSymbolHeight);
        if (symbols != null)
        {
            var syms = new ProfilePhrase(symbols, allowMovement: MaxMovement);

            var left = rect.Left;
            var top = rect.Top;
            var right = rect.Right;
            var bottom = rect.Bottom;
            if (symbols.Symbols.Count > 0 && syms.Bounds.IsNotEmpty)
            {
                if ((snaps & Snaps.Left) == Snaps.Left) left = syms.Bounds.Left;
                if ((snaps & Snaps.Top) == Snaps.Top) top = syms.Bounds.Top;
                if ((snaps & Snaps.Right) == Snaps.Right) right = syms.Bounds.Right;
                if ((snaps & Snaps.Bottom) == Snaps.Bottom) bottom = syms.Bounds.Bottom;
            }
            rect = new CRect(left, top, right - left, bottom - top);
        }

        var t = new ProExtractor(this, SourcePage!, name, rect, isblock, validCharacters,
            maxSymbolHeight ?? DefaultMaxSymbolHeight, Profile?.FontColor, isRequired);
        Extractors.Add(t);
        return t;
    }

    #endregion

    public void Move(double x, double y)
    {
        foreach (var identifier in Identifiers)
        {
            foreach (var phrase in identifier.Phrases)
            {
                phrase.SearchRect = phrase.SearchRect.Moved(x, y);
                foreach (var symbol in phrase.Symbols)
                    symbol.Move(x, y);
            }
        }
        foreach (var extractor in Extractors)
        {
            extractor.Rect = extractor.Rect.Moved(x, y);
            var identifier = extractor.IdentifierPhrase;
            if (identifier != null)
            {
                identifier.SearchRect = identifier.SearchRect.Moved(x, y);
                foreach (var symbol in identifier.Symbols)
                    symbol.Move(x, y);
            }
        }
        Rect = Rect.Moved(x, y);
        // this.RepeatBottom += y;
        // this.RepeatRect = this.RepeatRect.Moved(x, y);
    }

    public void Compile()
    {

        var rect = new CRect();
        foreach (var identifier in Identifiers)
        {
            identifier.Compile();
            rect = rect.Union(identifier.Bounds);
        }
        foreach (var extractor in Extractors)
        {
            extractor.Compile();
            rect = rect.Union(extractor.Rect);
        }
        Rect = rect;

        SourcePage = null;
    }

    public List<int> RequiredSymbolsAtLeft = new ();
}