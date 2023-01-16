using Jem.CommonLibrary22;

using System.Collections.ObjectModel;
using System.ComponentModel.Design;

namespace Jem.ProfilingLibrary23;

public class JProExtractor : IdNamed
{
    public JProExtractor(ProExtractor extractor)
    {
        this.Id = extractor.Id;
        this.Name = extractor.Name;
        this.Rect = extractor.Rect;
        this.Color = extractor.Color;
        this.IsRequired = extractor.IsRequired;
        this.IsBlock = extractor.IsBlock;
        this.MaxSymbolHeight = extractor.MaxSymbolHeight;
        this.RepeatTerminators = extractor.RepeatTerminators;

        if (extractor.IdentifierPhrase != null)
        {
            Identifier = new JProIdentifier(extractor.IdentifierPhrase);
        }
        this.Fields = extractor.Fields.ToArray();
    }

    public JProIdentifier? Identifier { get; }
    public ProExtractionFieldText[] Fields { get; }
    public CRect Rect { get; }
    public OcrColor? Color { get; }
    public bool IsBlock { get; }
    public float MaxSymbolHeight { get; }
    public ObservableCollection<string>? RepeatTerminators { get; }
    public bool IsRequired { get; internal set; }

    internal JProExtractorResult? ExtractFromLine(OcrPage oPage, CRect lineRect, CTheory? theory, int rowNumber)
    {
        var rowRect = new CRect(Rect.Left, lineRect.Top, Rect.Width, lineRect.Height);
        return ExtractFromRect(oPage, rowRect, theory, rowNumber);
    }

    internal JProExtractorResult? ExtractFromPage(OcrPage oPage, CTheory? theory, int rowNumber)
    {
        var rowRect = this.Rect;
        return ExtractFromRect(oPage, rowRect, theory, rowNumber);
    }

    private JProExtractorResult? ExtractFromRect(OcrPage oPage, CRect rowRect, CTheory? theory, int rowNumber)
    {
        if (theory != null)
            rowRect = theory.AppliedMoveScaleRotate(rowRect);

        var output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: this.MaxSymbolHeight, color: this.Color);
        if (output != null)
        {
            var top = output.Symbols.Min(s => s.Rect.Top);
            rowRect.Top = top;
            output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: this.MaxSymbolHeight, color: this.Color);
        }

        var symbols = output?.Symbols;
        var text = output?.Text;

        if (symbols == null) return null;
        if (text == null) return null;
        var eResult = new JProExtractorResult(this, rowNumber, rowRect, text, symbols, theory);

        var beginIndex = 0;
        foreach (var field in this.Fields)
        {
            if (this.IsBlock)
            {
                var lines = oPage.ExtractLines(rowRect, maxSymbolHeight: this.MaxSymbolHeight);
                var line = lines.Skip(field.LineNumber).FirstOrDefault();
                text = line?.Text ?? string.Empty;
                beginIndex = 0;
            }

            var value = field.Extract(text, beginIndex, out var endIndex, sample: null);
            if (!string.IsNullOrEmpty(value))
            {
                var isValid = true;
                if (field.Format == FieldFormat.DD)
                    if (value.Length < 1 || value.Length > 2 || !char.IsDigit(value[0]) || !char.IsDigit(value[value.Length - 1])
                        || !int.TryParse(value, out var dd) || dd < 1 || dd > 31)
                        isValid = false;

                if (isValid)
                    if (field.Format == FieldFormat.DDMMM)
                        if (value.Length < 4 || value.Length > 6 || !char.IsDigit(value[0]) || !char.IsLetter(value[value.Length - 1]))
                            isValid = false;

                if (isValid)
                    if (field.Format == FieldFormat.MMM)
                        if (value.Length != 3 || !CoreConstants.MonthCodes.Contains(value))
                            isValid = false;

                if (isValid)
                    if (field.Format == FieldFormat.Currency)
                        if (value.Length < 1 || !char.IsDigit(value[0]))
                            isValid = false;

                if (isValid)
                    if (field.ValidCharacters != null)
                    {
                        foreach (var c in value)
                        {
                            if (!field.ValidCharacters.Contains(c))
                            { isValid = false; break; }
                        }
                    }

                if (isValid)
                {
                    var fResult = new JProFieldResult(this, field, rowRect, value, beginIndex, endIndex);
                    eResult.FieldResults.Add(fResult);
                    //OnFieldExtracted(fResult);
                }
                else return null;
            }

            if (this.RepeatTerminators != null)
                foreach (var rt in this.RepeatTerminators)
                {
                    if (value?.Contains(rt) == true)
                        return null;
                }

            beginIndex = endIndex;
        }

        return eResult;
    }
}