//using Jem.CommonLibrary22;

//namespace Jem.ProfilingLibrary23;

//public class JProfileTemplateCompiled
//{
//    public JProfile Profile;
//    public JProfileTemplate Template;

//    public int maxMoveDistanceX;
//    public int maxMoveDistanceY;

//    public double minimumScaling;
//    public double maximumScaling;

//    public double minimumRotation;
//    public double maximumRotation;

//    public double minSuccessRatio;
//    public double maxFailureRatio;

//    /// <summary>To improve performance, we only consider up to 25 symbols against each identifier.</summary>
//    public int maxSymbolsToConsiderPerIdentifier = 25;

//    /// <summary>Testing may pass the stencil once it has this number of matching identifiers.</summary>
//    public int passScore;

//    /// <summary>Testing may fail the stencil once its has this number of mismatching identifiers.</summary>
//    public int failScore;

//    public List<OcrSymbol> CompiledSymbols;
//    public List<OcrSymbol> CompiledSymbolsLeft;
//    public List<OcrSymbol> CompiledSymbolsRight;

//    public JProfileTemplateCompiled(JProfileTemplate stencil)
//    {
//        if (stencil.OwnerProfile == null)
//            throw new ArgumentNullException(nameof(JProfileTemplate.OwnerProfile));

//        this.Profile = stencil.OwnerProfile;
//        this.Template = stencil;

//        CompileConstants();
//        CompileIdentifiers();
//        CompileIdentifierSpread();
//        //CompileGrid();

//        CompileExtractors();
//    }

//    private void CompileExtractors()
//    {
//        // each extractor is linked to a phrase to allow for shifting of the extraction field
//        foreach (var extractor in Template.Extractors)
//        {
//            extractor.AlignPhraseLeft = FindClosestPhraseLeftOf(extractor.Rect.CenterPoint);
//            extractor.AlignPhraseRight = FindClosestPhraseRightOf(extractor.Rect.CenterPoint);
//        }
//    }

//    //private JProfilePhrase FindClosestPhraseLeftOf(CPoint extractorCenterPoint)
//    //{
//    //    double bestDistance = 0;
//    //    JProfilePhrase bestPhrase = null;

//    //    foreach (var phrase in Template.Phrases)
//    //    {
//    //        if (phrase.Rect.Right >= extractorCenterPoint.X) continue;

//    //        var leftDistance = CMaths.DistanceBetween(phrase.Rect.CenterPoint, extractorCenterPoint);
//    //        if (bestPhrase == null || leftDistance < bestDistance)
//    //        {
//    //            bestPhrase = phrase;
//    //            bestDistance = leftDistance;
//    //        }
//    //    }

//    //    return bestPhrase;
//    //}

//    private JProfilePhrase FindClosestPhraseRightOf(CPoint extractorCenterPoint)
//    {
//        double bestDistance = 0;
//        JProfilePhrase bestPhrase = null;

//        foreach (var phrase in Template.Phrases)
//        {
//            if (phrase.Rect.Left <= extractorCenterPoint.X) continue;

//            var rightDistance = CMaths.DistanceBetween(phrase.Rect.CenterPoint, extractorCenterPoint);
//            if (bestPhrase == null || rightDistance < bestDistance)
//            {
//                bestPhrase = phrase;
//                bestDistance = rightDistance;
//            }
//        }

//        return bestPhrase;
//    }

//    private void CompileConstants()
//    {
//        minimumScaling = 1 - (Template.MaxScalingPercentage / 100f);
//        maximumScaling = 1 + (Template.MaxScalingPercentage / 100f);

//        minimumRotation = -Template.MaxRotationDegrees;
//        maximumRotation = +Template.MaxRotationDegrees;

//        minSuccessRatio = Math.Round(Template.MinSuccessPercentage / 100f, 2);
//        maxFailureRatio = Math.Round(Template.MaxFailurePercentage / 100f, 2);

//        //var width = Template.BorderRect.Width;
//        //var height = Template.BorderRect.Height;

//        //var widthInvalid = (double.IsNaN(width) || width.Almost(0));
//        //var heightInvalid = (double.IsNaN(height) || height.Almost(0));
//        //if (widthInvalid || heightInvalid)
//        //{
//        //    var msg = $"BorderRect Width {width} and Height {height} of stencil {Template.Name} are not valid for profile {Profile?.Name}!";
//        //    throw new InvalidOperationException(msg);
//        //}

//        this.maxMoveDistanceX = (int)(Template.MaxMoveDistance.Width); // (int)(Template.BorderRect.Width * Template.MaxMoveXPercentage / 100);
//        this.maxMoveDistanceY = (int)(Template.MaxMoveDistance.Height); // (int)(Template.BorderRect.Height * Template.MaxMoveYPercentage / 100);
//    }

//    private void CompileIdentifiers()
//    {
//        // TODO: Sort compiled symbols by least frequent occurrence to optimize performance
//        // This is turned off when debugging to make it easier to validate symbol matching

//        // Find required identifiers, i.e. those that identify the page
//        this.CompiledSymbols = Template.AllIdentifiers();

//        var identifiersCenterPoint = Template.PhrasesRect().TopLeft;

//        if (CompiledSymbols.Count < 6)
//        {
//            throw new InvalidJProfileTemplateException(Template, $"A minimum of 6 identifiers is required for {Template.Err}!");
//        }

//        // Order characters left-to-right in order to determine horizontal scaling
//        var orderedIdentifiers = (from symbol in this.CompiledSymbols
//                                  where char.IsLetterOrDigit(symbol.MatchCharacter)
//                                  orderby CMaths.DistanceBetween(symbol.Rect.CenterPoint, identifiersCenterPoint)
//                                  select symbol).ToList();

//        var halfCount = orderedIdentifiers.Count / 2;
//        var orderedIdentifiersLeft = orderedIdentifiers.Take(halfCount).ToList();
//        var orderedIdentifiersRight = orderedIdentifiers.Skip(halfCount).Reverse().ToList();

//        // TODO: Build frequency count for identifiers and sort identification by least frequent to most frequent to improve performance
//        //var frequencyIdentifiersLeft = DetermineCharacterFrequencyAscending(orderedIdentifiersLeft);
//        //var frequencyIdentifiersRight = DetermineCharacterFrequencyAscending(orderedIdentifiersRight);

//        // TODO: review this limit before going live
//        var testlimit = orderedIdentifiersLeft.Count;

//        CompiledSymbolsLeft = (from identifier in orderedIdentifiersLeft
//                                   //orderby frequencyIdentifiersLeft[identifier.Character]
//                               select identifier)
//                                   .Take(testlimit).ToList();

//        CompiledSymbolsRight = (from identifier in orderedIdentifiersRight
//                                    //orderby frequencyIdentifiersRight[identifier.Character]
//                                select identifier)
//                                    .Take(testlimit).ToList();
//    }

//    private void CompileIdentifierSpread()
//    {
//        passScore = (int)Math.Ceiling(CompiledSymbols.Count * minSuccessRatio);
//        failScore = CompiledSymbols.Count - passScore;

//        var compiledIdentifiers = CompiledSymbols;

//        var toRemoveCount = 0;
//        float nextRemoveDistance = compiledIdentifiers.Count;

//        // Given 35 identifiers and requiring 80%, we remove 7 identifiers at equal spacing
//        toRemoveCount = compiledIdentifiers.Count - (int)passScore;

//        // Thus, 35 / 7 = every 5th identifier can be moved to end
//        nextRemoveDistance = (float)compiledIdentifiers.Count / (float)toRemoveCount;

//        for (int i = 0; i < toRemoveCount; i++)
//        {
//            var id = compiledIdentifiers[(int)Math.Ceiling(i * nextRemoveDistance)];
//            compiledIdentifiers.Remove(id);
//            compiledIdentifiers.Add(id);
//        }
//    }

//    //private static Dictionary<char, int> DetermineCharacterFrequencyAscending(List<Pro5Symbol> identifiers)
//    //{
//    //    var result = new Dictionary<char, int>();
//    //    foreach (var kv in from kv in DetermineCharacterFrequencyUnordered(identifiers) orderby kv.Value select kv)
//    //    {
//    //        result.Add(kv.Key, kv.Value);
//    //    }
//    //    return result;
//    //}

//    //private static Dictionary<char, int> DetermineCharacterFrequencyUnordered(List<Pro5Symbol> identifiers)
//    //{
//    //    var frequency = new Dictionary<char, int>();
//    //    foreach (var identifier in identifiers)
//    //    {
//    //        if (frequency.ContainsKey(identifier.Character))
//    //            frequency[identifier.Character]++;
//    //        else
//    //            frequency.Add(identifier.Character, 1);
//    //    }
//    //    return frequency;
//    //}

//    //private void CompileGrid()
//    //{
//    //    if (stencil.Rect.IsEmpty) return;

//    //    this.gridColumns = (int)Math.Max(stencil.Rect.Width / 500, 2);
//    //    this.gridRows = (int)Math.Max(stencil.Rect.Height / 500, 2);
//    //    this.grid = new CSize[gridColumns, gridRows];
//    //    this.gridRectLeft = stencil.Rect.Left;
//    //    this.gridRectTop = stencil.Rect.Top;
//    //    this.gridColumnWidth = stencil.Rect.Width / gridColumns;
//    //    this.gridRowHeight = stencil.Rect.Height / gridRows;
//    //}

//    ///// <summary>We will use the alignment of individual characters to create an alignment grid for extraction.</summary>
//    //public CSize[,] grid;

//    //public double gridRectLeft;
//    //public double gridRectTop;
//    //public int gridColumns;
//    //public int gridRows;
//    //public double gridColumnWidth;
//    //public double gridRowHeight;
//}