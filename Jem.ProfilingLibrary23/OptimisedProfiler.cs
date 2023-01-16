using Jem.OcrLibrary22;
using Jem.Profiling22;

using System.Threading.Tasks.Sources;
using System.Xml.Linq;

using static System.Net.Mime.MediaTypeNames;

namespace Jem.ProfilingLibrary23;

public sealed class OptimisedProfiler
{
    public float Version = 23.0f;

    public OptimisedProfiler(ProBag bag) : this(new JProBag(bag)) { }

    public OptimisedProfiler(JProBag optimisedBag) { Root = optimisedBag; }

    public JProBag Root { get; }

    #region Identify

    public ODocumentResult IdentifyDocument(OcrDocument oDocument, string? source)
    {
        var cDocument = new CompiledOcrDocument(oDocument);
        return IdentifyDocument(oDocument, cDocument, source);
    }

    public ODocumentResult IdentifyDocument(OcrDocument oDocument, CompiledOcrDocument cDocument, string? source)
    {
        var count = cDocument.Pages.Count;
        var pageResults = new JProProfileResult?[count];

        for (int i = 0; i < count; i++)
        {
            var oPage = oDocument.Pages[i];
            if (oPage.IsEmpty) continue;

            var cPage = cDocument.Pages[i];
            pageResults[i] = IdentifyProfile(oDocument.Path, cPage);
        }

        var results = new ODocumentResult(oDocument, cDocument, pageResults: pageResults, source: source);
        return results;
    }

    public JProProfileResult? IdentifyPage(string? documentPath, OcrPage oPage)
    {
        var cPage = new CompiledOcrPage(oPage);
        return IdentifyProfile(documentPath, cPage);
    }

    public JProProfileResult? IdentifyProfile(string? documentPath, CompiledOcrPage cPage)
    {
        JProProfileResult? bestProfileResult = null;

        foreach (var group in Root.Groups)
            foreach (var profile in group.Profiles)
            {
                var profileResult = profile.IdentifyPage(documentPath, cPage);
                if (profileResult?.Profile == null) continue;

                if (bestProfileResult == null || bestProfileResult.Score < profileResult.Score)
                    bestProfileResult = profileResult;
            }

        return bestProfileResult;
    }

    public SkipKeyValue? IdentifySkipable(string oDocumentText)
    {
        var lines = oDocumentText.Split("\n");
        foreach (var line in lines)
        {
            var text = line.NoSpaces();
            if (text == null || text.Length == 0) continue;

            foreach (var kv in Root.Skipables)
            {
                if (text.Contains(kv.Value) == true)
                    return kv;
            }
        }
        return null;
    }

    #endregion Identify

    #region Extract

    /// <summary>Extract all pages that were identified.</summary>
    public void Extract(ODocumentResult documentResult)
    {
        Extract(documentResult, documentResult.ODocument, documentResult.CDocument);
    }

    /// <summary>Extract all pages that were identified.</summary>
    public void Extract(ODocumentResult documentResult, OcrDocument oDocument, CompiledOcrDocument cDocument)
    {
        JProProfileResult? lastPageResult = null;

        for (int i = 0; i < documentResult.PageResults.Length; i++)
        {
            var pageResult = documentResult.PageResults[i];

            if (lastPageResult?.Profile?.AssumeAllPages == true)
            {
                var pageNumberText = lastPageResult.TryFindFieldResult("PageRange", "PageNumber")?.Value;
                var pageCountText = lastPageResult.TryFindFieldResult("PageRange", "PageCount")?.Value;
                if (
                    pageNumberText != null && int.TryParse(pageNumberText, out var pageNumber) &&
                    pageCountText != null && int.TryParse(pageCountText, out var pageCount) &&
                    pageNumber < pageCount)
                {
                    if (pageResult == null)
                    {
                        pageResult = new JProProfileResult();
                        documentResult.PageResults[i] = pageResult;
                    }

                    pageResult.DocumentPath = lastPageResult.DocumentPath;
                    pageResult.PageIndex = lastPageResult.PageIndex + 1;
                    pageResult.Profile = lastPageResult.Profile;
                    pageResult.Theory = lastPageResult.Theory;
                    pageResult.ReachedEndOfDocument = (pageNumber == pageCount) || lastPageResult.ReachedEndOfDocument;
                }
            }

            if (pageResult == null) continue;

            var oPage = oDocument.Pages.First(a => a.PageIndex == pageResult.PageIndex);
            var cPage = cDocument.Pages.First(a => a.PageIndex == pageResult.PageIndex);
            Extract(pageResult, oPage, cPage);

            lastPageResult = pageResult;
        }
    }

    /// <summary>Extract a page that was identified.</summary>
    private void Extract(JProProfileResult pageResult, OcrPage oPage, CompiledOcrPage cPage)
    {
        pageResult.LinesTop = null;
        pageResult.LinesBottom = null;
        pageResult.ReachedEndOfLines = false;
        pageResult.ReachedEndOfDocument = false;
        //pageResult.Score = 0;

        pageResult.TemplateResults.Clear();

        var rowTheory = new CTheory(pageResult.Theory);

        var rowNumber = 0;
        ExtractTemplatesOfType(oPage, cPage, pageResult, ProfileTemplateType.Page, rowTheory, ref rowNumber);

        oPage.PageNumber = pageResult.TryFindFieldResult("PageRange", "PageNumber")?.Value;

        ExtractTemplatesOfType(oPage, cPage, pageResult, ProfileTemplateType.Section, rowTheory, ref rowNumber);
        ExtractTemplatesOfType(oPage, cPage, pageResult, ProfileTemplateType.Header, rowTheory, ref rowNumber);
        ExtractTemplatesOfType(oPage, cPage, pageResult, ProfileTemplateType.Footer, rowTheory, ref rowNumber);

        if (pageResult.LinesTop != null && pageResult.LinesBottom != null)
        {
            ExtractLines(pageResult, pageResult.Profile!, oPage, cPage, rowTheory, ProfileTemplateType.LineL);
            ExtractLines(pageResult, pageResult.Profile!, oPage, cPage, rowTheory, ProfileTemplateType.LineR);
        }
    }

    private void ExtractTemplatesOfType(
        OcrPage oPage, CompiledOcrPage cPage, JProProfileResult profileResult,
        ProfileTemplateType type, CTheory pageTheory, ref int rowNumber)
    {
        // Blank pages can result in no profile matching
        if (profileResult.Profile == null) return;

        var jProfile = profileResult.Profile;
        var jTemplates = (from t in jProfile.Templates
                          where t.Type == type
                          select t).ToList();

        foreach (var jTemplate in jTemplates)
        {
            if (!jTemplate.SupportsPageNumber(profileResult)) continue;

            var data = jTemplate.ExtractFromPage(oPage, cPage, pageTheory, ref rowNumber);
            if (data == null)
            {
                // When managing rows or similiar, we stop if the template was required and not found
                if (jTemplate.IsRequired) return;
                continue;
            }

            var topMovement = pageTheory.Move.Height;
            if (data.Theory != null) topMovement = data.Theory.Move.Height;

            if (jTemplate.LinesTop != null)
            {
                var top = jTemplate.LinesTop;
                top += topMovement;
                profileResult.LinesTop = top;
            }

            if (jTemplate.LinesBottom != null)
            {
                var bottom = jTemplate.LinesBottom;
                bottom += pageTheory.Move.Height;
                profileResult.LinesBottom = bottom;
            }

            if (data.RowResults.Count > 0)
            {
                //foreach (var row in data.RowResults)
                //{
                //    rowNumber++;
                //    row.RowNumber = rowNumber;
                //}
                profileResult.TemplateResults.Add(data);
            }
            if (data.ReachedEndOfDocument) { profileResult.ReachedEndOfDocument = true; break; }
            if (data.ReachedEndOfLines) { profileResult.ReachedEndOfLines = true; break; }
            if (data.IsEndOfPage) { break; }

            if (data.RowResults.Count > 0)
            {
                if (jTemplate.Type == ProfileTemplateType.LineL || jTemplate.Type == ProfileTemplateType.LineR)
                {
                    profileResult.CountLineRowsExtracted += data.RowResults.Count;
                    if (profileResult.CountLineRowsExtracted >= profileResult.Profile.MaxLineCountPerPage) return;
                }
                else
                {
                    profileResult.CountHeaderRowsExtracted += data.RowResults.Count;
                }
            }

            if (data.IsEndOfPage) return;
        }
    }

    private void ExtractLines(JProProfileResult pResult, JProProfile profile, OcrPage oPage, CompiledOcrPage cPage, CTheory lineTheory, ProfileTemplateType lineType)
    {
        if (pResult.ReachedEndOfDocument) return;

        double? top = pResult.LinesTop;
        double? bottom = pResult.LinesBottom;
        if (top == null || bottom == null) return;

        JProTemplate[] lineTemplates = profile.Templates.Where(t => t.Type == lineType).ToArray();
        if (lineTemplates.Length == 0) return;

        OcrPhrase[] lines = oPage.ExtractLines(new CRect(0, top.Value, 2100, bottom.Value - top.Value), maxSymbolHeight: 35, color: profile.FontColor).ToArray();

        string? pageNumber = pResult.TryFindFieldResult("PageRange", "PageNumber")?.Value;

        var rowNumber = 1;
        foreach (OcrPhrase line in lines)
        {
            foreach (var eod in profile.EndOfDocumentTerminators)
            {
                if (line.Text.Contains(eod, StringComparison.OrdinalIgnoreCase))
                {
                    pResult.ReachedEndOfDocument = true;
                    return;
                }
            }

            foreach (var jTemplate in lineTemplates)
            {
                if (!jTemplate.SupportsPageNumber(pResult)) continue;
                if (!jTemplate.SupportsLine(pResult, line)) continue;

                var rResult = jTemplate.ExtractFromLine(oPage, line, null, pageNumber, rowNumber);
                if (rResult == null) continue;
                if (rResult.ExtractorResults.Count == 0) continue;

                rowNumber++;
                var tResult = new JProTemplateResult(jTemplate, lineTheory);
                tResult.RowResults.Add(rResult);

                if (tResult.RowResults.Count > 0)
                    pResult.TemplateResults.Add(tResult);

                pResult.ReachedEndOfDocument |= tResult.ReachedEndOfDocument;
                pResult.ReachedEndOfLines |= tResult.ReachedEndOfLines;

                break; // finished with templates for this line
            }

            if (pResult.ReachedEndOfLines) break;
            if (pResult.ReachedEndOfDocument) break;
        }
    }

    //public JProExtractorResult? ExtractFields(ProExtractor extractor, OcrPage oPage, CTheory? theory, int rowNumber)
    //{
    //    var rowRect = extractor.Rect;

    //    if (theory != null)
    //        rowRect = theory.AppliedMoveScaleRotate(rowRect);

    //    //var xlines = oPage.ExtractLines(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight);
    //    //var output = xlines.FirstOrDefault();

    //    var output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight, color: extractor.Color);
    //    if (output != null)
    //    {
    //        var top = output.Symbols.Min(s => s.Rect.Top);
    //        rowRect.Top = top;
    //        output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight, color: extractor.Color);
    //    }

    //    var symbols = output?.Symbols;
    //    var text = output?.Text;

    //    if (symbols == null) return null;
    //    if (text == null) return null;

    //    var profile = extractor.Template.Profile;
    //    foreach (var fix in profile.TextFixes)
    //    {
    //        if (text.Contains(fix.Key))
    //        {
    //            text = text.Replace(fix.Key, fix.Value);
    //        }
    //    }

    //    var eResult = new ExtractorResult(extractor, rowNumber, rowRect, text, symbols, theory);

    //    var beginIndex = 0;
    //    foreach (var field in extractor.Fields)
    //    {
    //        if (extractor.IsBlock)
    //        {
    //            var lines = oPage.ExtractLines(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight);
    //            var line = lines.Skip(field.LineNumber).FirstOrDefault();
    //            text = line?.Text ?? string.Empty;
    //            beginIndex = 0;
    //        }

    //        var value = field.Extract(text, beginIndex, out var endIndex, sample: null);
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            var fResult = new FieldResult(extractor, field, rowRect, value, beginIndex, endIndex);
    //            eResult.FieldResults.Add(fResult);
    //            OnFieldExtracted(fResult);
    //        }

    //        if (extractor.RepeatTerminators != null)
    //            foreach (var rt in extractor.RepeatTerminators)
    //            {
    //                if (value?.Contains(rt) == true)
    //                    return null;
    //            }

    //        beginIndex = endIndex;
    //    }

    //    return eResult;
    //}

    //protected void OnFieldExtracted(FieldResult fResult)
    //{
    //    var value = fResult.Value;
    //    if (value.Contains('\u0096'))
    //        fResult.Value = value.Replace('\u0096', '-');
    //}

    public IEnumerable<string> Debug(IEnumerable<JProProfileResult?> pResults)
    {
        foreach (var pResult in pResults)
            foreach (var line in Debug(pResult))
                yield return line;
    }

    public static IEnumerable<string> Debug(JProProfileResult? pResult)
    {
        if (pResult == null) yield break;

        foreach (var row in pResult.HeaderRows)
            yield return DebugRow(row);

        foreach (var row in pResult.LineRows)
            yield return DebugRow(row);
    }

    public static string DebugRow(RowResult row)
    {
        return row.ToString();
    }

    #endregion

}