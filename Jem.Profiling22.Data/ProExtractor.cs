using System.Collections.ObjectModel;

namespace Jem.Profiling22.Data;

/// <summary>
///     Defines an area from which to extract fields.
///     This is relative to the identified template but aligns using its own identifiers.
/// </summary>
public class ProExtractor : IdNamed
{
    #region class

    public ProExtractor(ProTemplate template, OcrPage oPage, string name, 
        CRect rect, bool isBlock, string? validCharacters, 
        float maxSymbolHeight, OcrColor? color, bool isRequired) :
        base(name)
    {
        Template = template;
        ValidCharacter = validCharacters;
        OPage = oPage;
        Rect = rect;
        Color = color;
        IsBlock = isBlock;
        IsRequired = isRequired;
        MaxSymbolHeight = maxSymbolHeight;
    }

    public override string ToString()
    {
        var name = Name;
        if (name == null && Fields.Count > 0) name = Fields[0].Name;
        return $"{name}"; // ' for template '{this.Template.Name}'
    }

    #endregion

    #region Properties

    public ProTemplate Template { get; }
    public string? ValidCharacter { get; }
    public OcrPage OPage { get; }
    public CRect Rect { get; set; }
    public bool IsBlock { get; set; }
    public bool IsRequired { get; }
    public float MaxSymbolHeight { get; set; }
    public ProfilePhrase? IdentifierPhrase { get; set; }
    public ObservableCollection<ProExtractionFieldText> Fields { get; set; } = new();
    public ObservableCollection<string>? RepeatTerminators { get; set; }
    
    public OcrColor? Color { get; set; }

    #endregion

    //public ProfileTextExtraction FindText(string identifier)
    //{
    //    throw new NotImplementedException();
    //}

    //public ProfileTextExtraction ExtractField(string v1, CRect CRect, string v2)
    //{
    //    throw new NotImplementedException();
    //}


    public ProExtractor AddField(string name, int lineNumber = 0, string? lead = null, string? follow = null, string? sample = null, FieldFormat? format = null, string? validCharacters=null, bool isNegative = false, bool reportOnNewLine = false)
    {
        if (Name?.Length == 0) Name = name;

        string truth;
        if (IsBlock)
        {
            var lines = OPage.ExtractLines(Rect, maxSymbolHeight: MaxSymbolHeight).ToList() ; // todo: optimise
            var line = lines.Skip(lineNumber).FirstOrDefault();
            truth = line?.Text ?? string.Empty;
        }
        else
        {
            truth = OPage.ExtractText(Rect, maxSymbolHeight: MaxSymbolHeight) ?? string.Empty;
        }

        var field = new ProExtractionFieldText
        {
            Name = name,
            Lead = lead,
            ValidCharacters = validCharacters,
            Follow = follow,
            Sample = sample,
            Format = format,
            IsNegative = isNegative,
            LineNumber = lineNumber,
            ReportOnNewLine = reportOnNewLine,
        };
        _ = field.Extract(truth, 0, out var _, sample);
        Fields.Add(field);
        return this;
    }

    public ProExtractor AddIdentifier(string sample, CRect rect, CSize allowMovement, string? mask = null)
    {
        var symbols = OPage.ExtractSymbolsAndText(rect, maxSymbolHeight: MaxSymbolHeight) ??
            throw new TextMismatchDefiningProfile($"Expected '{sample}' but found no identification characters!");

        //var phrase = new ProfilePhrase(symbols, allowMovement, mask);

        if (symbols.Text.NoSpaces() != sample.NoSpaces())
            throw new TextMismatchDefiningProfile($"Expected '{sample}' but found '{symbols.Text}' for identification!");

        //if (symbols.Text!.Length < sample.Length)
        //    throw new TextMismatchDefiningProfile($"Expected '{sample}' but found '{symbols.Text}' for identification!");

        //// sample = sample.Replace(" ", "");
        //var j = 0;
        //for (int i = 0; i < sample.Length; i++)
        //{
        //    var character = sample[i];
        //    if (character == ' ') continue;

        //    var symbol = symbols.Symbols![j++];
        //    if (character != symbol.Text[0])
        //        throw new TextMismatchDefiningProfile($"Mismatch at index {i}. Expected '{sample}' but found '{symbols.Text}' for identification!");
        //}

        IdentifierPhrase = new(symbols, allowMovement, mask);
        return this;
    }

    public ProExtractor AddRepeatTerminator(string value)
    {
        RepeatTerminators ??= new();
        RepeatTerminators.Add(value);
        return this;
    }

    internal virtual void Compile()
    {

    }
}
