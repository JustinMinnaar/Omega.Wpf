//using Jem.CommonLibrary22;
//using Jem.OcrLibrary22;

//namespace Jem.ProfilingLibrary23
//{
//    public sealed class JProIdentifier
//    {
//        #region class

//        //public JProIdentifier(CSize maxMoveDistance, double minSuccessScore)
//        //{
//        //    MaxMoveDistance = maxMoveDistance;
//        //    MinSuccessScore = minSuccessScore;
//        //}

//        public JProIdentifier(ProIdentifier identifier)
//        {
//            this.identifier = identifier;
//            //var symbols = new List<OcrSymbol>();
//            //foreach (var phrase in identifier.Phrases)
//            //{
//            //    symbols.AddRange(phrase.Symbols);
//            //}
//            //this.Symbols = symbols.ToArray();
//        }
//        //public OcrSymbol[] Symbols { get; }

//        public override string ToString()
//        {
//            return identifier.ToText();
//        }

//        #endregion class


//        #region Identify

//        public ProIdentifier identifier;

//        private JProIdentifierCompiled? compiled;

//        public JProIdentifierResult? Identify(CompiledOcrPage cPage)
//        {
//            compiled ??= new(identifier);
//            return compiled.Identify(cPage);
//        }

//        public double Score(CompiledOcrPage cPage, CTheory theory, CSize maxStretch)
//        {
//            compiled ??= new(identifier);
//            return compiled.Score(cPage, theory, maxStretch);
//        }

//        #endregion Identify

//        ///// <summary>Test the stencil (as if it were moved by considerMovePixels) and see how well its identifier characters line up.</summary>
//        ///// <param name="pageOcr"></param>
//        ///// <param name="compiled"></param>
//        ///// <param name="cPage">For performance reasons, we optimized symbols for testing.</param>
//        ///// <param name="considerMovePixels"></param>
//        ///// <returns></returns>
//        //private JProfilePhraseResult TestStencilMatching(CCompiledOcr cPage, CTheory theory, CSize theoryStretch)
//        //{
//        //    var step = writer?.WriteLine($"Test for {minSuccessScore * 100:0}% of {CompiledSymbols.Count} identifier symbols.");

//        //    var countGood = 0;
//        //    var countBad = 0;

//        //    foreach (var identifier in compiled.CompiledSymbols)
//        //    {
//        //        var identifierCenterPoint = identifier.Rect.CenterPoint;

//        //        //var shiftingX = (int)(identifierCenterPoint.X * shiftingWidth / ocr.PageRect.Width);
//        //        //var shiftingY = (int)(identifierCenterPoint.Y * shiftingHeight / ocr.PageRect.Height);

//        //        //var shiftingCenterPoint = new CPoint
//        //        //{
//        //        //    X = shiftingX * ocr.PageRect.Width / shiftingWidth,
//        //        //    Y = shiftingY * ocr.PageRect.Height / shiftingHeight
//        //        //};

//        //        //var alignDistance = CMaths.DistanceBetween(shiftingCenterPoint, identifierCenterPoint);
//        //        //var alignHeight = (int)(shifting[shiftingX, shiftingY] * shiftingHeight / alignDistance);

//        //        //if (alignHeight != 0)
//        //        //{
//        //        //    identifierCenterPoint.Y += alignHeight;
//        //        //}

//        //        var theoryPoint = theory.ApplyMoveScaleRotate(identifierCenterPoint);
//        //        if (double.IsNaN(theoryPoint.X))
//        //            throw new InvalidOperationException($"An internal error occurred calculating the center point of identifier '{identifier.MatchCharacter}'!");

//        //        var shp = log?.AddShape(identifier,
//        //                $"Test Matching on identifier '{identifier.MatchCharacter}' at {identifier.Rect} with theory {theoryPoint}");

//        //        var ocrSymbol = TryMatchIdenfierWithOcrSymbol(cPage, identifier, theoryPoint, theoryStretch, step);
//        //        if (ocrSymbol != null)
//        //        {
//        //            // How much is this symbol out of alignment?
//        //            //var symbolCenterPoint = ocrSymbol.OcrRect.CenterPoint;
//        //            //var misalignWidth = Math.Round(symbolCenterPoint.X - theoryPoint.X, 0);
//        //            //var misalignHeight = Math.Round(symbolCenterPoint.Y - theoryPoint.Y, 0);

//        //            //shifting[shiftingX, shiftingY] = (int)misalignHeight;

//        //            //var gridX = (int)((identifierCenterPoint.X - compiled.gridRectLeft) / compiled.gridColumnWidth);
//        //            //var gridY = (int)((identifierCenterPoint.Y - compiled.gridRectTop) / compiled.gridRowHeight);
//        //            //compiled.grid[gridX, gridY] = new CSize(misalignWidth, misalignHeight);

//        //            shp?.SetBehaviour(EPro5Behaviour.Good);
//        //            countGood++;
//        //            if (countGood >= compiled.passScore) break;
//        //        }
//        //        else
//        //        {
//        //            shp?.SetBehaviour(EPro5Behaviour.Bad);
//        //            countBad++;
//        //            if (countBad > compiled.failScore) break;
//        //        }

//        //        //if (stepsEnabled && verbose)
//        //        //{
//        //        //    var shpIdentifier = step.AddShape(identifier); shpIdentifier.Behavior = identifier.Behaviour;

//        //        //    var symbolRect = identifier.Rect;
//        //        //    symbolRect.Left = theoryPoint.X - symbolRect.Width / 2;
//        //        //    symbolRect.Top = theoryPoint.Y - symbolRect.Height / 2;
//        //        //    var shpSymbol = step.AddRect(identifier.Character, symbolRect); shpSymbol.Behavior = identifier.Behaviour;
//        //        //    var shpLine = step.AddLine(identifierCenterPoint, theoryPoint); shpLine.Behavior = identifier.Behaviour;
//        //        //}
//        //    }

//        //    return new Pro5StencilResult
//        //    {
//        //        CountSymbols = compiled.CompiledSymbols.Count,
//        //        CountGood = countGood,
//        //        CountBad = countBad,
//        //        RatioGood = (float)countGood / compiled.CompiledSymbols.Count,
//        //        RadioBad = (float)countBad / compiled.CompiledSymbols.Count,
//        //    };
//        //}

//        //private COcrSymbol TryMatchIdenfierWithOcrSymbol(CCompiledOcr compiledOcr, Pro5Identifier identifier, CPoint theoryPoint, CSize theoryStretch, IPro5Log step)
//        //{
//        //    // We allow the symbol to be moved (typically due to warping of a page during scanning)
//        //    var w = theoryStretch.Width;
//        //    var h = theoryStretch.Height;

//        //    var identifierTheoryRect = new CRect(theoryPoint.X - w, theoryPoint.Y - h, w * 2, h * 2);

//        //    var character = identifier.MatchCharacter;
//        //    if (compiledOcr.AllSymbols.TryGetValue(character, out var sameSymbols))
//        //    {
//        //        foreach (var symbol in sameSymbols)
//        //        {
//        //            var shp = step?.AddRect(character, identifierTheoryRect);
//        //            shp?.SetBehaviour(EPro5Behaviour.Bad);

//        //            // Does this ocr symbol fall into the theory rectangle
//        //            if (!identifierTheoryRect.Contains(symbol.OcrRect.CenterPoint)) continue;

//        //            // We expect the symbol to be about the same size on the original and current pages
//        //            // Rotation could change its size
//        //            //var symbolWidth = symbol.OcrRect.Width;
//        //            //var symbolHeight = symbol.OcrRect.Height;
//        //            //var variation = 5;
//        //            //if (symbolWidth < identifier.Rect.Width - variation || symbolWidth > identifier.Rect.Width + variation) continue;
//        //            //if (symbolHeight < identifier.Rect.Height - variation || symbolHeight > identifier.Rect.Height + variation) continue;

//        //            shp?.SetBehaviour(EPro5Behaviour.Good);
//        //            return symbol;
//        //        }
//        //    }

//        //    return null;
//        //}
//    }
//}