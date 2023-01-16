using Jem.CommonLibrary22;
using Jem.OcrLibrary22;
using Jem.Profiling22;

using System.Diagnostics;
using System.Runtime.CompilerServices;

using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Jem.ProfilingLibrary23;

public sealed class JProTemplate : IdNamed
{
    /// <summary>One identifier must match in order to match the template.</summary>
    public JProIdentifier[] Identifiers { get; }

    public JProExtractor[] Extractors { get; }

    public bool IsRequired { get; set; }
    public double? LinesTop { get; }
    public double? LinesBottom { get; }
    // public int MaxLinesCount { get; }
    public double? LinesHeight { get; }
    public int? ExpectedMinimumPageNumber { get; }
    public int? ExpectedMaximumPageNumber { get; }
    public bool IsEndOfPage { get; }
    public bool IsOptional { get; }
    public int[] RequiredSymbolsAtLeft { get; }
    public ProfileTemplateType Type { get; set; }
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; }
    public bool IsSkip { get; set; }

    public JProTemplate(ProTemplate template)
    {
        this.Id = template.Id;
        this.Name = template.Name;
        this.Type = template.Type;
        this.ProfileId = template.Profile!.Id;
        this.ProfileName = template.Profile!.Name;
        this.IsSkip = template.IsSkip;
        this.IsRequired = template.IsRequired;
        this.LinesTop = template.LinesTop;
        this.LinesBottom = template.LinesBottom;
        // this.MaxLinesCount = template.MaxLineCount;
        this.LinesHeight = template.LinesHeight;
        this.ExpectedMinimumPageNumber = template.ExpectedMinimumPageNumber;
        this.ExpectedMaximumPageNumber = template.ExpectedMaximumPageNumber;
        this.IsEndOfPage = template.IsEndOfPage;
        this.IsOptional = template.IsOptional;
        this.RequiredSymbolsAtLeft = template.RequiredSymbolsAtLeft.ToArray();

        this.Identifiers = CreateIdentifiers(template.Identifiers).ToArray();
        if (template.Type == ProfileTemplateType.Page && Identifiers.Length == 0)
            throw new FailedProfilingException($"Profile '{template.Profile?.Name}' Template '{template.Name}' does not have any identifiers!");

        this.Extractors = CreateExtractors(template.Extractors).ToArray();
    }

    private static IEnumerable<JProIdentifier> CreateIdentifiers(IEnumerable<ProIdentifier> identifiers)
    {
        foreach (var identifier in identifiers)
        {
            yield return new JProIdentifier(identifier);
        }
    }

    private static IEnumerable<JProExtractor> CreateExtractors(IEnumerable<ProExtractor> extractors)
    {
        foreach (var extractor in extractors)
        {
            yield return new JProExtractor(extractor);
        }
    }

    /// <summary>Locate the best identifier for the template and use its theory.</summary>
    public JProTemplateResult? IdentifyPage(CompiledOcrPage cPage)
    {
        JProTemplateResult? bestResult = null;
        foreach (var jIdentifier in this.Identifiers)
        {
            var result = jIdentifier.Identify(cPage);
            if (result == null) continue;

            if (bestResult == null || bestResult.Score < result.Score)
                bestResult = new JProTemplateResult
                {
                    Template = this,
                    JIdentifier = result.JIdentifier,
                    Theory = result.Theory,
                    Score = result.Score
                };
        }
        return bestResult;
    }

    public JProTemplateResult? ExtractFromPage(OcrPage oPage, CompiledOcrPage cPage, CTheory theory, ref int rowNumber)
    {
        var tResult = new JProTemplateResult(this, theory);

        foreach (var identifier in this.Identifiers)
        {
            var identifierResult = FindBestMatchingIdentifier(cPage, this, theory, rowNumber);
            if (identifierResult == null) continue;

            tResult.JIdentifier = identifierResult.JIdentifier;
            tResult.Score = identifierResult.Score;
            if (identifierResult.Theory != null)
            {
                theory = tResult.Theory = identifierResult.Theory;
            }
        }
        if (tResult.JIdentifier == null && this.Identifiers.Length > 0) return null;

        string? pageNumber = oPage.PageNumber;

        var rowResult = new RowResult(jTemplate: this, pageNumber, rowNumber, this.IsSkip);
        foreach (var extractor in this.Extractors)
        {
            if (extractor.Identifier != null)
            {
                var eir = extractor.Identifier.Identify(cPage, theory);
                if (eir?.Theory != null)
                    theory = eir.Theory;
                //else
                //    Debug.Assert(eir != null, "Failed to align identifier");
            }

            var eResult = extractor.ExtractFromPage(oPage, theory, rowNumber);
            if (eResult == null) continue;

            rowResult.ExtractorResults.Add(eResult);
        }
        if (rowResult.ExtractorResults.Count > 0)
        {
            tResult.RowResults.Add(rowResult);
            rowNumber++;
        }

        return tResult;
    }

    public RowResult? ExtractFromLine(OcrPage oPage, OcrPhrase oLine, CTheory? theory, string? pageNumber, int rowNumber)
    {
        var rowResult = new RowResult(jTemplate: this, pageNumber, rowNumber, this.IsSkip);
        foreach (var extractor in this.Extractors)
        {
            var eResult = extractor.ExtractFromLine(oPage, oLine.Bounds, theory, rowNumber);
            if (eResult == null)
            {
                if (extractor.IsRequired) return null;
                continue;
            }

            rowResult.ExtractorResults.Add(eResult);
        }
        return rowResult;
    }

    JProIdentifierResult? FindBestMatchingIdentifier(CompiledOcrPage cPage, JProTemplate jTemplate, CTheory theory, int rowNumber)
    {
        JProIdentifierResult? best = null;
        foreach (var identifier in this.Identifiers)
        {
            var result = identifier.Identify(cPage);
            if (result == null) continue;

            if (best == null || best.Score < result.Score)
                best = result;
        }
        return best;
    }

    public bool SupportsLine(JProProfileResult pResult, OcrPhrase phrase)
    {
        foreach (var jIdentifier in this.Identifiers)
        {
            if (jIdentifier.identifier == null) continue;

            foreach (var idphrase in jIdentifier.identifier.Phrases)
            {
                var r = idphrase.SearchRect;
                var rect = new CRect(r.Left, phrase.Bounds.Top, r.Width, phrase.Bounds.Height);
                var st = phrase.ExtractSymbols(rect, maxSymbolHeight: (int)(Math.Ceiling(phrase.Bounds.Height) + 1));
                var text = "" + (st?.Text);
                if (!idphrase.Matches(text)) return false;
            }
        }

        foreach (var x in RequiredSymbolsAtLeft)
        {
            var t = phrase.Symbols.Any(a => x > a.Rect.Left && x < a.Rect.Right);
            if (!t) return false;
        }
        return true;
    }

    public bool SupportsPageNumber(JProProfileResult pResult)
    {
        // todo: use index to correctly handle multiple repeats of number for different documents in same file
        var pageNumberText = pResult.TryFindFieldResult("PageRange", "PageNumber");
        if (pageNumberText != null)
        {
            if (int.TryParse(pageNumberText.Value, out var pageNumber))
            {
                if (this.ExpectedMinimumPageNumber > pageNumber) return false;
                if (this.ExpectedMaximumPageNumber < pageNumber) return false;
            }
        }
        return true;
    }


}