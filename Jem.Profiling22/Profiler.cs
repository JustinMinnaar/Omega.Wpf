//using Jem.CommonLibrary22;
//using Jem.OcrLibrary22;

//using static System.Net.Mime.MediaTypeNames;

//namespace Jem.Profiling22;

//public class OldProfiler
//{
//    public float Version { get; set; }

//    public float MinTemplateScore = 0.6f;

//    #region Identify

//    /// <summary>Locates the best matching profile for the page specified.</summary>
//    /// <param name="oPage">An ocr page containing symbols to search.</param>
//    /// <returns>The resulting profile, template, score, and theory.</returns>
//    public List<ProfileResult> IdentifyPageProfiles(OcrDocument oDocument, string source)
//    {
//        var list = new List<ProfileResult>();
//        foreach (var oPage in oDocument.Pages)
//        {
//            var pResult = IdentifyBestProfile(oDocument, oPage, source);
//            if (pResult == null) continue;
//            list.Add(pResult);
//        }
//        return list;
//    }

//    /// <summary>Locates the best matching profile for the page specified.</summary>
//    /// <param name="oPage">An ocr page containing symbols to search.</param>
//    /// <returns>The resulting profile, template, score, and theory.</returns>
//    public ProfileResult? IdentifyBestProfile(OcrDocument oDocument, OcrPage oPage, string source)
//    {
//        ProfileResult? best = null;
//        foreach (var profile in root.Profiles)
//        {
//            var pResult = IdentifyProfile(oDocument, oPage, profile, source);
//            if (pResult == null) continue;

//            if (best == null || best.Score < pResult.Score)
//                best = pResult;
//        }
//        return best;
//    }

//    /// <summary>Locates the template using its identifiers within the search space allowed, on the page specified.</summary>
//    /// <param name="oPage">An ocr page containing symbols to search.</param>
//    /// <returns>The result as found, shift, scale, rotation.</returns>
//    public ProfileResult? IdentifyProfile(OcrDocument oDocument, OcrPage oPage, ProProfile profile, string source)
//    {
//        if (oDocument == null) throw new ArgumentNullException(nameof(oDocument));
//        if (oPage == null) throw new ArgumentNullException(nameof(oPage));
//        if (profile == null) throw new ArgumentNullException(nameof(profile));

//        ProfileResult? best = null;
//        if (oPage.BlocksCount == 0 && oPage.ImagesCount == 0)
//        {
//            best = new(
//                source: source,
//                pageIndex: oPage.PageIndex,
//                isBlank: true,
//                profile: null,
//                template: null,
//                score: 1f,
//                theory: null);
//            return best;
//        }

//        var bestIdentifier = IdentifyBestIdentifier(oPage, profile.Identifiers);
//        if (bestIdentifier != null)
//        {
//            var pResult = new ProfileResult
//                (
//                    source: source,
//                    pageIndex: oPage.PageIndex,
//                    isBlank: false,
//                    profile: profile,
//                    template: null,
//                    score: bestIdentifier.Score,
//                    theory: bestIdentifier.Theory
//                );
//            return pResult;
//        }

//        foreach (var template in profile.Templates)
//        {
//            if (template.Type != ProfileTemplateType.Page &&
//                template.Type != ProfileTemplateType.Section) continue;

//            var tResult = IdentifyTemplate(oPage, template);
//            if (tResult == null) continue;

//            var pResult = new ProfileResult
//            (
//                source: oDocument.Name,
//                pageIndex: oPage.PageIndex,
//                isBlank: false,
//                profile: profile,
//                template: tResult.Template,
//                score: tResult.Score,
//                theory: tResult.Theory
//            );
//            if (best == null ||
//                best.Score < pResult.Score ||
//                best.Score == pResult.Score && best.Theory?.Move.Height > pResult.Theory?.Move.Height)
//            {
//                best = pResult;
//            }
//        }
//        return best;
//    }

//    /// <summary>Locates the template using its identifiers within the search space allowed, on the page specified.</summary>
//    /// <param name="oPage">An ocr page containing symbols to search.</param>
//    /// <returns>The result as found, shift, scale, rotation.</returns>
//    public TemplateResult? IdentifyTemplate(OcrPage oPage, ProTemplate template, CTheory? theory = null)
//    {
//        // Which identifier aligns best?
//        var iResult = IdentifyBestIdentifier(oPage, template, theory);
//        if (iResult == null) return null;

//        // What is the score of all identifiers?
//        var newTheory = new CTheory(iResult.Theory);
//        var (bestIdentifier, score) = ScoreIdentifiers(oPage, template, newTheory);
//        if (bestIdentifier == null) return null;
//        if (score == null || score < MinTemplateScore) return null;

//        var tResult = new TemplateResult($"{oPage.PageIndex + 1}", template, score.Value, iResult.Theory);
//        return tResult;
//    }

//    private (ProIdentifier?, double?) ScoreIdentifiers(OcrPage oPage, ProTemplate template, CTheory theory)
//    {
//        ProIdentifier? bestIdentifier = null;
//        double? bestScore = null;

//        foreach (var identifier in template.Identifiers)
//        {
//            double score = ScoreIdentifier(oPage, template, theory, identifier);
//            if (bestScore == null || bestScore < score)
//            {
//                bestIdentifier = identifier;
//                bestScore = score;
//            }
//        }

//        return (bestIdentifier, bestScore);
//    }

//    private double ScoreIdentifier(OcrPage oPage, ProTemplate template, CTheory theory, ProIdentifier identifier)
//    {
//        double score = 0;
//        foreach (var phrase in identifier.Phrases)
//        {
//            if (phrase.IsBlank) continue;

//            var result = LocatePhrase(oPage, phrase, theory);
//            if (result == null) continue;

//            score += result.Score;
//        }
//        score /= identifier.Phrases.Count;
//        return score;
//    }

//    public IdentifierResult? IdentifyBestIdentifier(OcrPage oPage, ProTemplate template, CTheory? theory = null)
//    {
//        return IdentifyBestIdentifier(oPage, template.Identifiers, theory);
//    }

//    public IdentifierResult? IdentifyBestIdentifier(OcrPage oPage, IList<ProIdentifier> identifiers, CTheory? theory = null)
//    {
//        IdentifierResult? best = null;
//        foreach (var iResult in LocateIdentifiers(oPage, identifiers, theory))
//        {
//            if (iResult.Score < MinTemplateScore) continue;
//            if (best == null || best.Score < iResult.Score) best = iResult;
//        }
//        return best;
//    }

//    public IEnumerable<IdentifierResult> LocateIdentifiers(OcrPage oPage, ProTemplate template, CTheory? theory = null)
//    {
//        var identifiers = template.Identifiers;
//        foreach (var result in LocateIdentifiers(oPage, template.Identifiers, theory))
//            yield return result;
//    }

//    public IEnumerable<IdentifierResult> LocateIdentifiers(OcrPage oPage, IEnumerable<ProIdentifier> identifiers, CTheory? theory = null)
//    {
//        foreach (var identifier in identifiers)
//        {
//            foreach (var phrase in identifier.Phrases)
//            {
//                var result = LocatePhrase(oPage, phrase, theory);
//                if (result != null) yield return result;
//            }
//        }
//    }


//    /// <summary>Using the specified shift, scale, and rotation, attempts to locate the symbols of the phrase (if they exists) and determine the shift, scale, and rotation of the found text relative to the search location.</summary>
//    public IdentifierResult? LocatePhrase(OcrPage oPage, ProfilePhrase phrase, CTheory? theory = null)
//    {
//        // We move the search rectangle using the theory
//        var rect = phrase.SearchRect;
//        if (theory != null)
//            rect = theory.AppliedMoveScaleRotate(rect);

//        if (phrase.IsBlank) rect.Inflate(0, -rect.Height / 2);

//        // Now we can find symbols on the page that fall into the search rectangle       
//        // and see if they correspond with the symbols we are looking for
//        var phraseBounds = phrase.Bounds;
//        float maxSymbolHeight = OcrPage.DEFAULT_MAX_SYMBOL_HEIGHT;
//        if (maxSymbolHeight < phraseBounds.Height) maxSymbolHeight = (float)phraseBounds.Height + 1f;

//        var ocrOut = oPage.ExtractSymbolsAndText(rect, maxSymbolHeight: maxSymbolHeight);
//        var symbols = ocrOut?.Symbols;
//        if (phrase.Symbols.Count == 0)
//        {
//            if (symbols == null)
//                return new IdentifierResult(1, theory ?? new(), phrase);
//            else
//                return new IdentifierResult(0, theory ?? new(), phrase);
//        }

//        // try find specific symbols first, then try matching the mask symbols afterwards
//        var searchSymbols = new List<OcrSymbol>();
//        var searchCharacters = new List<string>();
//        var q = phrase.Symbols.Where(s => !phrase.IsMask || !"#ULA".Contains(s.Text[0]))
//            .Union(phrase.Symbols.Where(s => phrase.IsMask && "#ULA".Contains(s.Text[0]))).ToList();

//        foreach (var s in q)
//        {
//            if (searchCharacters.Count >= 6) break;
//            if (searchCharacters.Contains(s.Text)) continue;
//            searchCharacters.Add(s.Text);
//            searchSymbols.Add(s);
//        }

//        IdentifierResult? bestResult = null;
//        if (symbols != null)
//            foreach (var phraseSymbol in searchSymbols)
//            {
//                // find closest matches
//                var candidates = (from s in symbols
//                                  where phraseSymbol.Matches(s.Text[0], phrase.IsMask)
//                                  orderby CMaths.DistanceBetween(s.Rect.CenterPoint, phraseSymbol.Rect.CenterPoint)
//                                  select s).Take(100).ToList();
//                foreach (var candidate in candidates)
//                {
//                    var distanceX = candidate.Rect.Left - phraseSymbol.Rect.Left;
//                    var distanceY = candidate.Rect.Top - phraseSymbol.Rect.Top;
//                    var newTheory = new CTheory { Move = new CSize(distanceX, distanceY) };

//                    var result = Score(oPage, phrase, newTheory, maxSymbolHeight);
//                    if (result == null) continue;

//                    //if (result.Score >= (phrase.MinMatchPercentage / 2f ?? MinTemplateScore))
//                    {
//                        if (bestResult == null || bestResult.Score < result.Score)
//                        {
//                            bestResult = result;
//                        }
//                    }
//                }

//                if (bestResult?.Score == 1) break;
//            }

//        return bestResult;
//    }

//    /// <summary>Score the phrase against the page at the theorised location.</summary>
//    /// <param name="oPage">The ocr page to look for the phrase on.</param>
//    /// <param name="theory">The theory determines where to look relative to the original phrase.</param>
//    /// <returns>A score of the number of characters of the phrase that were matched.</returns>
//    private IdentifierResult? Score(OcrPage oPage, ProfilePhrase phrase, CTheory theory, float maxSymbolHeight)
//    {
//        double score = 0;
//        double distanceX = 0;
//        double distanceY = 0;

//        // var maxSymbolHeight = (float)(phrase.Bounds.Height);

//        var ocrRect = theory.AppliedMoveScaleRotate(phrase.Bounds).Inflated(2, 2);
//        var ocrFound = oPage.ExtractSymbolsAndText(ocrRect, maxSymbolHeight: maxSymbolHeight);
//        if (ocrFound == null)
//        {
//            if (phrase.IsBlank) return new IdentifierResult(1, theory, phrase);
//            return null;
//        }

//        var searchSymbols = phrase.Symbols.ToList();

//        // TODO: Consider partial matching for similiar characters found at the position searched
//        // todo: speed up by doing a single extraction of the phrase rectangle and then comparing individual symbols

//        foreach (var phraseSymbol in searchSymbols)
//        {
//            var rect = theory.AppliedMoveScaleRotate(phraseSymbol.Rect).Inflated(2, 2);
//            //var ocrOut = oPage.ExtractSymbolsAndText(rect,  maxSymbolHeight: maxSymbolHeight);
//            var ocrOut = ocrFound.ExtractSymbols(rect, maxSymbolHeight: maxSymbolHeight);
//            var symbols = ocrOut?.Symbols;
//            if (symbols == null) continue;

//            var ocrSymbols = (from s in symbols
//                              where phraseSymbol.Matches(s.Text[0], phrase.IsMask)
//                              orderby CMaths.DistanceBetween(rect.CenterPoint, s.Rect.CenterPoint)
//                              select s
//                              ).ToList();

//            var ocrSymbol = ocrSymbols.FirstOrDefault(a => phraseSymbol.Matches(a.Text[0], phrase.IsMask));
//            if (ocrSymbol != null)
//            {
//                score++;
//                var sRect = ocrSymbol.Rect;
//                distanceX += sRect.Left - phraseSymbol.Rect.Left;
//                distanceY += sRect.Top - phraseSymbol.Rect.Top;
//            }
//            else
//                continue;
//        }
//        distanceX /= phrase.Symbols.Count;
//        distanceY /= phrase.Symbols.Count;
//        score /= phrase.Symbols.Count;

//        var scoreDistance = Math.Abs(distanceX + distanceY);
//        if (scoreDistance >= 1)
//        {
//            scoreDistance /= oPage.Bounds.Width + oPage.Bounds.Height;
//            score -= scoreDistance;
//        }

//        //theory.Move += new CSize(distanceX, distanceY);

//        return new IdentifierResult(score, theory, phrase);
//    }

//    #endregion

//    #region Extract

//    public TemplateResult? ExtractHeader(OcrPage oPage, ProfileTemplate template, CTheory theory, ref int rowNumber)
//    {
//        var tResult = new TemplateResult($"{oPage.PageIndex + 1}", template, 1, theory);

//        // todo: get corrected theory
//        // theory = new();

//        var found = (template.Identifiers.Count == 0);
//        foreach (var identifier in template.Identifiers)
//        {
//            var result = LocatePhrase(oPage, identifier.Phrases[0], theory);
//            if (result == null || result.Score < MinTemplateScore) continue;
//            else
//            {
//                found = true;
//                theory = result.Theory;
//                break;
//            }
//        }
//        if (!found) { return null; }

//        tResult.Theory = new CTheory(theory);

//        rowNumber++;

//        var profileName = tResult.Template.Profile.Name;
//        var rResult = new RowResult(tResult, rowNumber, profileName, template.Id, template.Name, template.Type, template.IsSkip);
//        foreach (var extractor in template.Extractors)
//        {
//            CTheory? eTheory = new CTheory(theory);

//            if (extractor.Identifier != null)
//            {
//                var result = LocatePhrase(oPage, extractor.Identifier, theory);
//                if (result == null) break;
//                eTheory = result.Theory;
//            }

//            var eResult = ExtractFields(extractor, oPage, eTheory, rowNumber);
//            if (eResult == null || eResult.FieldResults.Count == 0) continue;

//            eResult.Theory = eTheory;
//            rResult.ExtractorResults.Add(eResult);
//        }

//        if (rResult.ExtractorResults.Count > 0)
//            tResult.RowResults.Add(rResult);

//        return tResult;
//    }

//    //public IEnumerable<ExtractedText> ExtractTexts(ProfileTemplate template, OcrPage oPage, CTheory? theory)
//    //{
//    //    foreach (var extractor in template.Extractors)
//    //    {
//    //        var rect = theory == null ? extractor.Rect : theory.AppliedMoveScaleRotate(extractor.Rect);
//    //        var text = oPage.ExtractTextOrdered(rect);
//    //        var et = new ExtractedText(template, extractor, text, rect);
//    //        yield return et;
//    //    }
//    //}

//    //public void Extract(OcrPage testPage, Profile profile, TemplateResult templateResult)
//    //{
//    //    templateResult.Extract();
//    //}

//    public ExtractorResult? ExtractFields(ProfileExtractor extractor, OcrPage oPage, CTheory? theory, int rowNumber)
//    {
//        var rowRect = extractor.Rect;

//        if (theory != null)
//            rowRect = theory.AppliedMoveScaleRotate(rowRect);

//        //var xlines = oPage.ExtractLines(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight);
//        //var output = xlines.FirstOrDefault();

//        var output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight, color: extractor.Color);
//        if (output != null)
//        {
//            var top = output.Symbols.Min(s => s.Rect.Top);
//            rowRect.Top = top;
//            output = oPage.ExtractSymbolsAndText(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight, color: extractor.Color);
//        }

//        var symbols = output?.Symbols;
//        var text = output?.Text;

//        if (symbols == null) return null;
//        if (text == null) return null;

//        var profile = extractor.Template.Profile;
//        foreach (var fix in profile.TextFixes)
//        {
//            if (text.Contains(fix.Key))
//            {
//                text = text.Replace(fix.Key, fix.Value);
//            }
//        }

//        var eResult = new ExtractorResult(extractor, rowNumber, rowRect, text, symbols, theory);

//        var beginIndex = 0;
//        foreach (var field in extractor.Fields)
//        {
//            if (extractor.IsBlock)
//            {
//                var lines = oPage.ExtractLines(rowRect, maxSymbolHeight: extractor.MaxSymbolHeight);
//                var line = lines.Skip(field.LineNumber).FirstOrDefault();
//                text = line?.Text ?? string.Empty;
//                beginIndex = 0;
//            }

//            var value = field.Extract(text, beginIndex, out var endIndex, sample: null);
//            if (!string.IsNullOrEmpty(value))
//            {
//                var fResult = new FieldResult(extractor, field, rowRect, value, beginIndex, endIndex);
//                eResult.FieldResults.Add(fResult);
//                OnFieldExtracted(fResult);
//            }

//            if (extractor.RepeatTerminators != null)
//                foreach (var rt in extractor.RepeatTerminators)
//                {
//                    if (value?.Contains(rt) == true)
//                        return null;
//                }

//            beginIndex = endIndex;
//        }

//        return eResult;
//    }

//    protected virtual void OnFieldExtracted(FieldResult fResult)
//    {
//        var value = fResult.Value;
//        if (value.Contains('\u0096'))
//            fResult.Value = value.Replace('\u0096', '-');
//    }

//    /// <summary>Drops the page ocr data to reduce memory usage.</summary>
//    /// <remarks>Overwrite to perform custom functionality with the result of the profiler.</remarks>
//    /// <param name="pResult"></param>
//    public virtual string? Finish(IEnumerable<ProfileResult> pResults)
//    {
//        return null;
//    }

//    public virtual IEnumerable<string> Debug(IEnumerable<ProfileResult> pResults)
//    {
//        foreach (var pResult in pResults)
//            foreach (var line in Debug(pResult))
//                yield return line;
//    }

//    public virtual IEnumerable<string> Debug(ProfileResult pResult)
//    {
//        foreach (var row in pResult.HeaderRows)
//            yield return DebugRow(row);

//        foreach (var row in pResult.LineRows)
//            yield return DebugRow(row);
//    }

//    protected virtual string DebugRow(RowResult row)
//    {
//        return row.ToString();
//    }

//    #endregion


//}