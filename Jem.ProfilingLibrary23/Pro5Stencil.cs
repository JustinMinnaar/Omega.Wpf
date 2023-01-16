//using Jem.CommonLibrary22;
//using Jem.OcrLibrary22;
//using Jem.ProfilingLibrary23;

//using System.Xml;

//namespace Jem.ProfilingLibrary23;

///// <summary>Provides an optimized search for a specific stencil on any OCR page.</summary>
//public sealed class JProfileTemplateTester
//{
//    public JProfileTemplateCompiled template;
//    private IWriter? writer;

//    public JProfileTemplateTester(JProfileTemplate template, IWriter? writer = null)
//    {
//        this.template = template;
//        this.writer = writer;
//    }

//    public JProfileTemplateResult LocatePageTemplate(CCompiledOcr ocr)
//    {
//        var result = ConsiderLeftIdentifiers(ocr);
//        if (result != null && result.Score >= template.minSuccessRatio)
//        {
//            result.Template = template.Template;
//        }
//        return result;
//    }

//    private JProfileTemplateResult ConsiderLeftIdentifiers(CCompiledOcr ocr)
//    {
//        foreach (var leftIdentifier in template.CompiledSymbolsLeft)
//        {
//            var match = ConsiderLeftIdentifier(ocr, leftIdentifier);
//            if (match != null && match.RatioGood >= template.minSuccessRatio) return match;
//        }
//        return null;
//    }

//    private JProfileTemplateResult ConsiderLeftIdentifier(CCompiledOcr ocr, OcrSymbol leftIdentifier)
//    {
//        // To support slight distortion of pages during scanning
//        var maxTheoryStretch = new CSize(8, 8);

//        // Find all symbols matching identifier character that fall into the search rectangle
//        var leftIdentifierPoint = leftIdentifier.Rect.CenterPoint;

//        // The rectangle is the maximum distance the page may move,
//        CRect leftSearchRect = NewSearchRect(template.maxMoveDistanceX, template.maxMoveDistanceY, leftIdentifierPoint);

//        // plus movement caused by the maximum rotation that can be applied
//        //leftSearchRect = ExpandSearchRect(ocr, leftSearchRect);

//        // TODO: plus movement caused by the maximum scaling that can be applied
//        // TODO: Remove .ToList() used for debugging from queries, so that we can short circuit these queries for performance

//        var leftSymbolsToConsider = ocr.AllSymbols
//            .Inside(leftIdentifier.MatchCharacter, leftSearchRect)
//            .OrderBy(leftSymbol => DistanceBetween(leftIdentifier, leftSymbol))
//            .Take(template.maxSymbolsToConsiderPerIdentifier).ToList();

//        foreach (var leftSymbol in leftSymbolsToConsider)
//        {
//            var leftSymbolRect = leftSymbol.OcrRect;
//            var leftSymbolPoint = leftSymbolRect.CenterPoint;
//            leftSymbolPoint.X = Math.Floor(leftSymbolPoint.X);
//            leftSymbolPoint.Y = Math.Floor(leftSymbolPoint.Y);

//            var theory = new CTheory
//            {
//                Move = new CSize
//                {
//                    Width = Math.Round(leftSymbolPoint.X - leftIdentifierPoint.X, 1),
//                    Height = Math.Round(leftSymbolPoint.Y - leftIdentifierPoint.Y, 1)
//                }
//            };

//            var match = ConsiderRightIdentifiers
//                (ocr, leftIdentifier, leftIdentifierPoint, leftSymbol, leftSymbolPoint, theory, maxTheoryStretch);
//            if (match != null && match.RatioGood >= template.minSuccessRatio) return match;
//        }

//        return null;
//    }

//    private CRect ExpandSearchRect(CCompiledOcr ocr, CRect leftSearchRect)
//    {
//        var rotateLeftTheory = new CTheory
//        {
//            Move = new CSize(template.maxMoveDistanceX, template.maxMoveDistanceY),
//            Origin = ocr.PageRect.CenterPoint,
//            Scaling = template.maximumScaling,
//            Rotation = template.maximumRotation,
//        };
//        var rotateLeftSearchRect1 = rotateLeftTheory.ApplyMoveScaleRotate(leftSearchRect);

//        var rotateRightTheory = new CTheory
//        {
//            Move = new CSize(template.maxMoveDistanceX, template.maxMoveDistanceY),
//            Origin = ocr.PageRect.CenterPoint,
//            Scaling = template.maximumScaling,
//            Rotation = -template.maximumRotation,
//        };
//        var rotateRightSearchRect2 = rotateRightTheory.ApplyMoveScaleRotate(leftSearchRect);

//        leftSearchRect.Union(rotateLeftSearchRect1);
//        leftSearchRect.Union(rotateRightSearchRect2);
//        return leftSearchRect;
//    }

//    private JProfileTemplateResult ConsiderRightIdentifiers
//        (CCompiledOcr ocr, OcrSymbol leftIdentifier, CPoint leftIdentifierPoint,
//         OcrSymbol leftSymbol, CPoint leftSymbolPoint, CTheory theory, CSize theoryStretch)
//    {
//        foreach (var rightIdentifier in template.CompiledSymbolsRight)
//        {
//            //var rightIdentifierPoint = rightIdentifier.Rect.CenterPoint;
//            var match = ConsiderRightIdentifier(ocr, leftIdentifier, leftIdentifierPoint, leftSymbol, leftSymbolPoint, rightIdentifier, theory, theoryStretch);
//            if (match != null && match.RatioGood >= template.minSuccessRatio) return match;
//        }
//        return null;
//    }

//    private JProfileTemplateResult ConsiderRightIdentifier(
//        CCompiledOcr ocr, OcrSymbol leftIdentifier, CPoint leftIdentifierPoint,
//        OcrSymbol leftSymbol, CPoint leftSymbolPoint, OcrSymbol rightIdentifier,
//        CTheory originalTheory, CSize theoryStretch)
//    {
//        // Find all symbols matching identifier character that fall into the search rectangle
//        var rightIdentifierPoint = rightIdentifier.Rect.CenterPoint;
//        //rightIdentifierPoint.X += originalTheory.Move.Width;
//        //rightIdentifierPoint.Y += originalTheory.Move.Height;

//        CRect rightSearchRect = NewSearchRect(template.maxMoveDistanceX, template.maxMoveDistanceY, rightIdentifierPoint);

//        // plus movement caused by the maximum rotation that can be applied
//        // rightSearchRect = ExpandSearchRect(ocr, rightSearchRect);

//        // TODO: plus movement caused by the maximum scaling that can be applied

//        var rightSymbolsToConsider = ocr.AllSymbols.Inside(rightIdentifier.MatchCharacter, rightSearchRect)
//            .OrderBy(rightSymbol => DistanceBetween(rightIdentifier, rightSymbol))
//            .Take(template.maxSymbolsToConsiderPerIdentifier).ToList();

//        foreach (var rightSymbol in rightSymbolsToConsider)
//        {
//            var rightSymbolPoint = rightSymbol.OcrRect.CenterPoint;
//            var rightSymbolMove = new CSize
//            {
//                Width = rightSymbolPoint.X - rightIdentifierPoint.X,    //- theoryMove.Width,
//                Height = rightSymbolPoint.Y - rightIdentifierPoint.Y,   // - theoryMove.Height,
//            };

//            var step = writer?.AddStep(template.Template, $"Identifiers '{leftIdentifier.MatchCharacter}' and '{rightIdentifier.MatchCharacter}'");
//            var shpLeftIdentifier = step?.AddShape(leftIdentifier);
//            var shpLeftLine = step?.AddLine(leftIdentifierPoint, leftSymbolPoint);
//            var shpLeftSymbol = step?.AddRect(leftSymbol.OcrCharacter, leftSymbol.OcrRect);
//            var shpRightIdentifier = step?.AddShape(rightIdentifier);
//            var shpLeftToRightLine = step?.AddLine(leftIdentifierPoint, rightIdentifierPoint);
//            var shpRightLine = step?.AddLine(rightIdentifierPoint, rightSymbolPoint);
//            var shpRightSymbol = step?.AddRect(rightSymbol.OcrCharacter, rightSymbol.OcrRect);

//            var scaling = TheorizeScaling(leftIdentifierPoint, leftSymbolPoint, rightIdentifierPoint, rightSymbolPoint, step);
//            if (scaling < template.minimumScaling || scaling > template.maximumScaling)
//            {
//                shpLeftToRightLine?.SetBehaviour(EPro5Behaviour.Bad);
//                if (step != null) writer?.AddMessage(" - BAD SCALE!");
//                continue;
//            }

//            var rotation = TheorizeRotation(leftIdentifierPoint, leftSymbolPoint, rightIdentifierPoint, rightSymbolPoint, step);
//            if (rotation < template.minimumRotation || rotation > template.maximumRotation)
//            {
//                shpLeftToRightLine?.SetBehaviour(EPro5Behaviour.Bad);
//                step?.AddMessage(" - BAD ROTATION!");
//                continue;
//            }

//            // If the page was scaled and rotated (around its center) where would our left OCR point have been before such scaling and rotation?
//            var centerOfPage = ocr.PageRect.CenterPoint;
//            var theory = new CTheory
//            {
//                Origin = centerOfPage,
//                Scaling = scaling,
//                Rotation = rotation,
//            };
//            var theoryReversed = new CTheory
//            {
//                // don't move, just rotate and scale around center of page
//                Origin = centerOfPage,
//                Rotation = -theory.Rotation,
//                Scaling = 1f / theory.Scaling
//            };

//            // Now we have an idea of where the left identifier would be if this rotation to the page was applied.
//            // This allows us to figure out how far to move the page after scaling and rotation to realign the page
//            var leftSymbolPointBeforeRotation = theoryReversed.ApplyMoveScaleRotate(leftSymbolPoint);
//            var leftIdentifierPointAfterRotation = theory.ApplyMoveScaleRotate(leftIdentifierPoint);

//            // note: floating point rounding may cause a miscalculation of 1 pixel when value is odd.
//            leftSymbolPointBeforeRotation.X = Math.Round(leftSymbolPointBeforeRotation.X, 0);
//            leftSymbolPointBeforeRotation.Y = Math.Round(leftSymbolPointBeforeRotation.Y, 0);
//            //leftIdentifierPointAfterRotation.X = Math.Round(leftIdentifierPointAfterRotation.X, 0);
//            //leftIdentifierPointAfterRotation.Y = Math.Round(leftIdentifierPointAfterRotation.Y, 0);

//            // The move distance is now the distance to where the left symbol would be if not rotated
//            var moveWidth = Math.Round(leftSymbolPointBeforeRotation.X - leftIdentifierPoint.X, 0);
//            var moveHeight = Math.Round(leftSymbolPointBeforeRotation.Y - leftIdentifierPoint.Y, 0);
//            //var moveWidth = Math.Round(leftSymbolPoint.X - leftIdentifierPointAfterRotation.X, 0);
//            //var moveHeight = Math.Round(leftSymbolPoint.Y - leftIdentifierPointAfterRotation.Y, 0);
//            theory.Move = new CSize(moveWidth, moveHeight);
//            if (Math.Abs(theory.Move.Width) >= template.maxMoveDistanceX) continue;
//            if (Math.Abs(theory.Move.Height) >= template.maxMoveDistanceY) continue;

//            //var identifierDistanceX = rightIdentifierPoint.X - leftIdentifierPoint.X;
//            //var identifierDistanceY = rightIdentifierPoint.Y - leftIdentifierPoint.Y;
//            //var symbolDistanceX = rightSymbolPoint.X - leftSymbolPoint.X;
//            //var symbolDistanceY = rightSymbolPoint.Y - leftSymbolPoint.Y;
//            //var scalingX = symbolDistanceX / identifierDistanceX;
//            //var scalingY = symbolDistanceY / identifierDistanceY;

//            // How well does the stencil match when moved here?
//            var match = TestTemplateMatching(ocr, theory, theoryStretch);
//            if (match.RatioGood >= template.minSuccessRatio)
//            {
//                if (step != null) step?.SetBehavior(EPro5Behaviour.Good);

//                match.Theory = theory;
//                //new CTheory
//                //{
//                //    Move = theory.Move,
//                //    Scaling = theory.Scaling,
//                //    RotationAngle = theory.RotationAngle,
//                //    RotationOrigin = theory.RotationOrigin
//                //};
//                return match;
//            }
//            else
//            {
//                if (step != null) step.SetBehavior(EPro5Behaviour.Bad);
//            }
//        }
//        return null;
//    }

//    private static double TheorizeRotation(CPoint leftIdentifierPoint, CPoint leftSymbolPoint, CPoint rightIdentifierPoint, CPoint rightSymbolPoint, IWriter step)
//    {
//        // We want the rotation around the left symbol, as we calculated the difference between
//        // the angle between left/right identifier and the angle between left/right symbols
//        //theory.Origin = new CPoint(Math.Round(leftSymbolPoint.X, 1), Math.Round(leftSymbolPoint.Y, 1));

//        var identifierAngle = Math.Round(CMaths.AngleBetween(rightIdentifierPoint, leftIdentifierPoint), 2);
//        var symbolsAngle = Math.Round(CMaths.AngleBetween(rightSymbolPoint, leftSymbolPoint), 2);
//        var rotation = Math.Round(symbolsAngle - identifierAngle, 2);
//        step?.AddMessage($" rotation:{rotation:0.00} around {leftSymbolPoint.X:0},{leftSymbolPoint.Y:0}");

//        return rotation;
//    }

//    private static double TheorizeScaling(CPoint leftIdentifierPoint, CPoint leftSymbolPoint, CPoint rightIdentifierPoint, CPoint rightSymbolPoint, IWriter step)
//    {
//        var identifierDistance = CMaths.DistanceBetween(rightIdentifierPoint, leftIdentifierPoint);
//        var symbolsDistance = CMaths.DistanceBetween(rightSymbolPoint, leftSymbolPoint);
//        var scaling = Math.Round(symbolsDistance / identifierDistance, 2);
//        if (double.IsNaN(scaling)) throw new InvalidOperationException("An internal error occurred whilst calculating scaling of the image.");

//        step?.AddMessage($" scaling:{scaling:0.00}");
//        return scaling;
//    }

//    /// <summary>Test the stencil (as if it were moved by considerMovePixels) and see how well its identifier characters line up.</summary>
//    /// <param name="pageOcr"></param>
//    /// <param name="compiled"></param>
//    /// <param name="ocr">For performance reasons, we optimized symbols for testing.</param>
//    /// <param name="considerMovePixels"></param>
//    /// <returns></returns>
//    private JProfileTemplateResult TestTemplateMatching(CCompiledOcr ocr, CTheory theory, CSize theoryStretch)
//    {
//        //var shiftingWidth = 50;
//        //var shiftingHeight = 50;
//        //var shifting = new int[(int)(ocr.PageRect.Width / shiftingWidth) + 1, (int)(ocr.PageRect.Height / shiftingHeight) + 1];

//        var step = writer?.AddStep(template.Template, $"Test for {template.Template.MinSuccessPercentage}% of {template.CompiledSymbols.Count} identifier symbols.");

//        var countGood = 0;
//        var countBad = 0;

//        foreach (var identifier in template.CompiledSymbols)
//        {
//            var identifierCenterPoint = identifier.Rect.CenterPoint;

//            //var shiftingX = (int)(identifierCenterPoint.X * shiftingWidth / ocr.PageRect.Width);
//            //var shiftingY = (int)(identifierCenterPoint.Y * shiftingHeight / ocr.PageRect.Height);

//            //var shiftingCenterPoint = new CPoint
//            //{
//            //    X = shiftingX * ocr.PageRect.Width / shiftingWidth,
//            //    Y = shiftingY * ocr.PageRect.Height / shiftingHeight
//            //};

//            //var alignDistance = CMaths.DistanceBetween(shiftingCenterPoint, identifierCenterPoint);
//            //var alignHeight = (int)(shifting[shiftingX, shiftingY] * shiftingHeight / alignDistance);

//            //if (alignHeight != 0)
//            //{
//            //    identifierCenterPoint.Y += alignHeight;
//            //}

//            var theoryPoint = theory.ApplyMoveScaleRotate(identifierCenterPoint);
//            if (double.IsNaN(theoryPoint.X))
//                throw new InvalidOperationException($"An internal error occurred calculating the center point of identifier '{identifier.MatchCharacter}'!");

//            var shp = writer?.AddShape(identifier,
//                    $"Test Matching on identifier '{identifier.MatchCharacter}' at {identifier.Rect} with theory {theoryPoint}");

//            var ocrSymbol = TryMatchIdenfierWithOcrSymbol(ocr, identifier, theoryPoint, theoryStretch, step);
//            if (ocrSymbol != null)
//            {
//                // How much is this symbol out of alignment?
//                //var symbolCenterPoint = ocrSymbol.OcrRect.CenterPoint;
//                //var misalignWidth = Math.Round(symbolCenterPoint.X - theoryPoint.X, 0);
//                //var misalignHeight = Math.Round(symbolCenterPoint.Y - theoryPoint.Y, 0);

//                //shifting[shiftingX, shiftingY] = (int)misalignHeight;

//                //var gridX = (int)((identifierCenterPoint.X - compiled.gridRectLeft) / compiled.gridColumnWidth);
//                //var gridY = (int)((identifierCenterPoint.Y - compiled.gridRectTop) / compiled.gridRowHeight);
//                //compiled.grid[gridX, gridY] = new CSize(misalignWidth, misalignHeight);

//                shp?.SetBehaviour(EPro5Behaviour.Good);
//                countGood++;
//                if (countGood >= template.passScore) break;
//            }
//            else
//            {
//                shp?.SetBehaviour(EPro5Behaviour.Bad);
//                countBad++;
//                if (countBad > template.failScore) break;
//            }

//            //if (stepsEnabled && verbose)
//            //{
//            //    var shpIdentifier = step.AddShape(identifier); shpIdentifier.Behavior = identifier.Behaviour;

//            //    var symbolRect = identifier.Rect;
//            //    symbolRect.Left = theoryPoint.X - symbolRect.Width / 2;
//            //    symbolRect.Top = theoryPoint.Y - symbolRect.Height / 2;
//            //    var shpSymbol = step.AddRect(identifier.Character, symbolRect); shpSymbol.Behavior = identifier.Behaviour;
//            //    var shpLine = step.AddLine(identifierCenterPoint, theoryPoint); shpLine.Behavior = identifier.Behaviour;
//            //}
//        }

//        return new JProfileTemplateResult
//        {
//            CountSymbols = template.CompiledSymbols.Count,
//            CountGood = countGood,
//            CountBad = countBad,
//            RatioGood = (float)countGood / template.CompiledSymbols.Count,
//            RadioBad = (float)countBad / template.CompiledSymbols.Count,
//        };
//    }

//    private OcrSymbol TryMatchIdenfierWithOcrSymbol(CCompiledOcr compiledOcr, OcrSymbol identifier, CPoint theoryPoint, CSize theoryStretch, IWriter step)
//    {
//        // We allow the symbol to be moved (typically due to warping of a page during scanning)
//        var w = theoryStretch.Width;
//        var h = theoryStretch.Height;

//        var identifierTheoryRect = new CRect(theoryPoint.X - w, theoryPoint.Y - h, w * 2, h * 2);

//        var character = identifier.MatchCharacter;
//        if (compiledOcr.AllSymbols.TryGetValue(character, out var sameSymbols))
//        {
//            foreach (var symbol in sameSymbols)
//            {
//                var shp = step?.AddRect(character, identifierTheoryRect);
//                shp?.SetBehaviour(EPro5Behaviour.Bad);

//                // Does this ocr symbol fall into the theory rectangle
//                if (!identifierTheoryRect.Contains(symbol.OcrRect.CenterPoint)) continue;

//                // We expect the symbol to be about the same size on the original and current pages
//                // Rotation could change its size
//                //var symbolWidth = symbol.OcrRect.Width;
//                //var symbolHeight = symbol.OcrRect.Height;
//                //var variation = 5;
//                //if (symbolWidth < identifier.Rect.Width - variation || symbolWidth > identifier.Rect.Width + variation) continue;
//                //if (symbolHeight < identifier.Rect.Height - variation || symbolHeight > identifier.Rect.Height + variation) continue;

//                shp?.SetBehaviour(EPro5Behaviour.Good);
//                return symbol;
//            }
//        }

//        return null;
//    }

//    private double DistanceBetween(OcrSymbol identifier, OcrSymbol symbol)
//    {
//        var identifierPoint = identifier.Rect.CenterPoint;
//        var symbolPoint = symbol.OcrRect.CenterPoint;
//        return CMaths.DistanceBetween(identifierPoint, symbolPoint);
//    }

//    private static CRect NewSearchRect(int maxMoveDistanceX, int maxMoveDistanceY, CPoint leftIdentifierPoint)
//    {
//        return new CRect
//        {
//            Left = (leftIdentifierPoint.X - maxMoveDistanceX),
//            Top = (leftIdentifierPoint.Y - maxMoveDistanceY),
//            Width = maxMoveDistanceX * 2,
//            Height = maxMoveDistanceY * 2
//        };
//    }
//}