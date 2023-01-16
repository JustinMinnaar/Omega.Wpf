using Jem.OcrLibrary22;

namespace Jem.ProfilingLibrary23
{
    public sealed class JProIdentifier
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var symbol in symbols)
                sb.Append(symbol.Character);
            return sb.ToString();
        }

        // for debugging
        // private readonly IWriter? writer = new TraceWriter();

        /// <summary>To improve performance, we only consider up to 5 symbols against each identifier.</summary>
        private readonly int maxSymbolsToConsiderPerIdentifier = 5;

        public ProIdentifier? identifier;

        private CSize maxMoveDistance;
        private double minSuccessScore = .9f;
        private JProSymbol[] symbols;
        private JProSymbol[] leftIdentifiers;
        private JProSymbol[] rightIdentifiers;

        public double minimumScaling = 0.95f;
        public double maximumScaling = 1.05f;
        public double minimumRotation = -15f;
        public double maximumRotation = +15f;

        internal JProIdentifier(ProfilePhrase phrase)
        {
            var psymbols = new List<JProSymbol>();
            foreach (var oSymbol in phrase.Symbols)
                psymbols.Add(new JProSymbol(oSymbol));
            symbols = psymbols.ToArray();

            var half = this.symbols.Length / 2;
            leftIdentifiers = GetUniqueSymbols(symbols.Take(half)).Take(maxSymbolsToConsiderPerIdentifier).ToArray();
            rightIdentifiers = GetUniqueSymbols(symbols.Skip(half).Reverse()).Take(maxSymbolsToConsiderPerIdentifier).ToArray();

            minSuccessScore = .9f;
            maxMoveDistance = phrase.AllowMovement; // new CSize(20, 10);
        }

        internal JProIdentifier(ProIdentifier identifier)
        {
            // All phrases are merged together as a single identifier
            // They should be found in their original positions relative to each other.

            this.identifier = identifier;

            symbols = GetSymbols(identifier);

            var half = symbols.Length / 2;
            leftIdentifiers = GetUniqueSymbols(symbols.Take(half)).Take(maxSymbolsToConsiderPerIdentifier).ToArray();
            rightIdentifiers = GetUniqueSymbols(symbols.Skip(half).Reverse()).Take(maxSymbolsToConsiderPerIdentifier).ToArray();

            minSuccessScore = identifier.MinSuccessScore;
            maxMoveDistance = identifier.MaxMoveDistance;
        }

        private JProSymbol[] GetSymbols(ProIdentifier identifier)
        {
            var symbols = new List<JProSymbol>();
            foreach (var phrase in identifier.Phrases)
                foreach (var oSymbol in phrase.Symbols)
                {
                    symbols.Add(new JProSymbol(oSymbol));
                }
            return symbols.ToArray();
        }

        private static JProSymbol[] GetUniqueSymbols(IEnumerable<JProSymbol> symbols)
        {
            var uniqueSymbols = new List<JProSymbol>();
            foreach (var symbol in symbols)
            {
                if (uniqueSymbols.Any(a => a.Character == symbol.Character)) continue;
                uniqueSymbols.Add(symbol);
            }
            return uniqueSymbols.ToArray();
        }

        public double Score(CompiledOcrPage cPage, CTheory theory, CSize maxStretch)
        {
            int countFailed = 0;
            int countMatched = 0;
            double score = 0f;
            foreach (var symbol in symbols)
            {
                var rect = theory.AppliedMoveScaleRotate(symbol.Rect).Inflated(maxStretch);
                var match = cPage.AllSymbols.AnyTouches(symbol.Character, rect);
                if (match) countMatched++; else countFailed++;
            }
            score = (countMatched / symbols.Length);
            return score;
        }

        internal JProIdentifierResult? Identify(CompiledOcrPage cPage, CTheory? pageTheory = null)
        {
            var result = ConsiderLeftIdentifiers(cPage, pageTheory);
            return result;
        }

        private JProIdentifierResult? ConsiderLeftIdentifiers(CompiledOcrPage cPage, CTheory? pageTheory = null)
        {
            var countConsidered = 0;
            foreach (var leftSymbol in leftIdentifiers)
            {
                var result = ConsiderLeftIdentifier(cPage, leftSymbol, pageTheory);
                if (result?.Score >= minSuccessScore) return result;
                countConsidered++;
                if (countConsidered == maxSymbolsToConsiderPerIdentifier) break;
            }
            return null;
        }

        private JProIdentifierResult? ConsiderLeftIdentifier(CompiledOcrPage cPage, JProSymbol leftIdentifier, CTheory? pageTheory = null)
        {
            // To support slight distortion of pages during scanning
            var maxStretch = new CSize(8, 8);

            // Find all symbols matching identifier character that fall into the search rectangle
            var leftIdentifierPoint = leftIdentifier.Rect.CenterPoint;

            // This rectangle is the maximum distance the phrase may move from origin
            CRect leftSearchRect = NewSearchRect(maxMoveDistance.Width, maxMoveDistance.Height, leftIdentifierPoint);
            if (pageTheory != null)
            {
                leftSearchRect = pageTheory.AppliedMoveScaleRotate(leftSearchRect);
            }
            // plus movement caused by the maximum rotation that can be applied
            //leftSearchRect = ExpandSearchRect(ocr, leftSearchRect);

            // TODO: plus movement caused by the maximum scaling that can be applied
            // TODO: Remove .ToList() used for debugging from queries, so that we can short circuit these queries for performance

            var leftSymbolsToConsider = cPage.AllSymbols
                .Touches(leftIdentifier.Character, leftSearchRect)
                .OrderBy(leftSymbol => DistanceBetween(leftIdentifier, leftSymbol))
                .Take(maxSymbolsToConsiderPerIdentifier).ToArray();

            foreach (var leftSymbol in leftSymbolsToConsider)
            {
                var leftSymbolRect = leftSymbol.Rect;
                var leftSymbolPoint = leftSymbolRect.CenterPoint;
                //leftSymbolPoint.X = Math.Floor(leftSymbolPoint.X);
                //leftSymbolPoint.Y = Math.Floor(leftSymbolPoint.Y);

                var theory = new CTheory
                {
                    Move = new CSize
                    {
                        Width = Math.Round(leftSymbolPoint.X - leftIdentifierPoint.X, 1),
                        Height = Math.Round(leftSymbolPoint.Y - leftIdentifierPoint.Y, 1)
                    }
                };

                var match = ConsiderRightIdentifiers
                    (cPage, leftIdentifier, leftIdentifierPoint, leftSymbol, leftSymbolPoint, maxStretch, theory);
                if (match?.Score >= minSuccessScore) return match;
            }

            return null;
        }

        private JProIdentifierResult? ConsiderRightIdentifiers
                (CompiledOcrPage cPage, JProSymbol leftIdentifier, CPoint leftIdentifierPoint,
                 OcrSymbol leftSymbol, CPoint leftSymbolPoint, CSize maxStretch, CTheory theory)
        {
            var countConsidered = 0;
            foreach (var rightIdentifier in rightIdentifiers)
            {
                var match = ConsiderRightIdentifier(cPage,
                    leftIdentifier, leftIdentifierPoint,
                    leftSymbol, leftSymbolPoint,
                    rightIdentifier,
                    maxStretch, theory);
                if (match?.Score >= minSuccessScore) return match;

                countConsidered++;
                if (countConsidered == maxSymbolsToConsiderPerIdentifier) break;
            }
            return null;
        }

        private JProIdentifierResult? ConsiderRightIdentifier(
            CompiledOcrPage cPage, JProSymbol leftIdentifier, CPoint leftIdentifierPoint,
            OcrSymbol leftSymbol, CPoint leftSymbolPoint, JProSymbol rightIdentifier,
            CSize maxStretch, CTheory leftTheory)
        {
            _ = leftSymbol;

            // Find all symbols matching identifier character that fall into the search rectangle
            var rightIdentifierPoint = leftTheory.AppliedMoveScaleRotate(rightIdentifier.Rect.CenterPoint);

            CRect rightSearchRect = NewSearchRect(maxMoveDistance.Width, maxMoveDistance.Height, rightIdentifierPoint);

            // plus movement caused by the maximum rotation that can be applied
            // rightSearchRect = ExpandSearchRect(ocr, rightSearchRect);

            // TODO: plus movement caused by the maximum scaling that can be applied

            var rightSymbolsToConsider = cPage.AllSymbols
                .Touches(rightIdentifier.Character, rightSearchRect)
                .OrderBy(rightSymbol => DistanceBetween(rightIdentifier, rightSymbol))
                .Take(maxSymbolsToConsiderPerIdentifier).ToList();

            foreach (var rightSymbol in rightSymbolsToConsider)
            {
                var rightSymbolPoint = rightSymbol.Rect.CenterPoint;
                var rightSymbolMove = new CSize
                {
                    Width = rightSymbolPoint.X - rightIdentifierPoint.X,    //- theoryMove.Width,
                    Height = rightSymbolPoint.Y - rightIdentifierPoint.Y,   // - theoryMove.Height,
                };

                //writerk?.WriteLine($"Identifiers '{leftIdentifier.Character}' and '{rightIdentifier.Character}'");

                //var shpLeftIdentifier = writer?.AddShape(leftIdentifier);
                //var shpLeftLine = writer?.AddLine(leftIdentifierPoint, leftSymbolPoint);
                //var shpLeftSymbol = writer?.AddRect(leftSymbol.OcrCharacter, leftSymbol.OcrRect);
                //var shpRightIdentifier = writer?.AddShape(rightIdentifier);
                //var shpLeftToRightLine = writer?.AddLine(leftIdentifierPoint, rightIdentifierPoint);
                //var shpRightLine = writer?.AddLine(rightIdentifierPoint, rightSymbolPoint);
                //var shpRightSymbol = writer?.AddRect(rightSymbol.OcrCharacter, rightSymbol.OcrRect);

                var scaling = 1; // TheorizeScaling(leftIdentifierPoint, leftSymbolPoint, rightIdentifierPoint, rightSymbolPoint);
                if (scaling < minimumScaling || scaling > maximumScaling)
                {
                    //shpLeftToRightLine?.SetBehaviour(EPro5Behaviour.Bad);
                    //writer?.WriteLine(" - BAD SCALE!");
                    continue;
                }

                var rotation = 0; // TheorizeRotation(leftIdentifierPoint, leftSymbolPoint, rightIdentifierPoint, rightSymbolPoint);
                if (rotation < minimumRotation || rotation > maximumRotation)
                {
                    //shpLeftToRightLine?.SetBehaviour(EPro5Behaviour.Bad);
                    //writer?.WriteLine(" - BAD ROTATION!");
                    continue;
                }

                // If the page was scaled and rotated (around its center) where would our left OCR point have been before such scaling and rotation?
                var centerOfPage = cPage.PageRect.CenterPoint;
                var theory = new CTheory
                {
                    Origin = centerOfPage,
                    Scaling = scaling,
                    Rotation = rotation,
                };
                var theoryReversed = new CTheory
                {
                    // don't move, just rotate and scale around center of page
                    Origin = centerOfPage,
                    Rotation = -theory.Rotation,
                    Scaling = 1f / theory.Scaling
                };

                // Now we have an idea of where the left identifier would be if this rotation to the page was applied.
                // This allows us to figure out how far to move the page after scaling and rotation to realign the page
                var leftSymbolPointBeforeRotation = theoryReversed.AppliedMoveScaleRotate(leftSymbolPoint);
                var leftIdentifierPointAfterRotation = theory.AppliedMoveScaleRotate(leftIdentifierPoint);

                // note: floating point rounding may cause a miscalculation of 1 pixel when value is odd.
                //leftSymbolPointBeforeRotation.X = Math.Round(leftSymbolPointBeforeRotation.X, 0);
                //leftSymbolPointBeforeRotation.Y = Math.Round(leftSymbolPointBeforeRotation.Y, 0);
                //leftIdentifierPointAfterRotation.X = Math.Round(leftIdentifierPointAfterRotation.X, 0);
                //leftIdentifierPointAfterRotation.Y = Math.Round(leftIdentifierPointAfterRotation.Y, 0);

                // The move distance is now the distance to where the left symbol would be if not rotated
                //var moveWidth = Math.Round(leftSymbolPointBeforeRotation.X - leftIdentifierPoint.X, 0);
                //var moveHeight = Math.Round(leftSymbolPointBeforeRotation.Y - leftIdentifierPoint.Y, 0);
                var moveWidth = (leftSymbolPointBeforeRotation.X - leftIdentifierPoint.X);
                var moveHeight = (leftSymbolPointBeforeRotation.Y - leftIdentifierPoint.Y);
                //var moveWidth = Math.Round(leftSymbolPoint.X - leftIdentifierPointAfterRotation.X, 0);
                //var moveHeight = Math.Round(leftSymbolPoint.Y - leftIdentifierPointAfterRotation.Y, 0);
                theory.Move = new CSize(moveWidth, moveHeight);
                if (Math.Abs(theory.Move.Width) >= maxMoveDistance.Width) continue;
                if (Math.Abs(theory.Move.Height) >= maxMoveDistance.Height) continue;

                //var identifierDistanceX = rightIdentifierPoint.X - leftIdentifierPoint.X;
                //var identifierDistanceY = rightIdentifierPoint.Y - leftIdentifierPoint.Y;
                //var symbolDistanceX = rightSymbolPoint.X - leftSymbolPoint.X;
                //var symbolDistanceY = rightSymbolPoint.Y - leftSymbolPoint.Y;
                //var scalingX = symbolDistanceX / identifierDistanceX;
                //var scalingY = symbolDistanceY / identifierDistanceY;

                // How well does the stencil match when moved here?
                var matchScore = Score(cPage, theory, maxStretch);
                if (matchScore >= minSuccessScore)
                {
                    //step?.SetBehavior(EPro5Behaviour.Good);

                    var result = new JProIdentifierResult
                    {
                        JIdentifier = this,
                        Score = matchScore,
                        Theory = new CTheory
                        {
                            Move = theory.Move,
                            Scaling = theory.Scaling,
                            Rotation = theory.Rotation,
                            Origin = theory.Origin,
                        },
                    };
                    return result;
                }
                else
                {
                    // step?.SetBehavior(EPro5Behaviour.Bad);
                }
            }
            return null;
        }

        private double TheorizeRotation(
            CPoint leftIdentifierPoint, CPoint leftSymbolPoint, CPoint rightIdentifierPoint, CPoint rightSymbolPoint)
        {
            // We want the rotation around the left symbol, as we calculated the difference between
            // the angle between left/right identifier and the angle between left/right symbols
            //theory.Origin = new CPoint(Math.Round(leftSymbolPoint.X, 1), Math.Round(leftSymbolPoint.Y, 1));

            var identifierAngle = Math.Round(CMaths.AngleBetween(rightIdentifierPoint, leftIdentifierPoint), 2);
            var symbolsAngle = Math.Round(CMaths.AngleBetween(rightSymbolPoint, leftSymbolPoint), 2);
            var rotation = Math.Round(symbolsAngle - identifierAngle, 2);

            //writer?.WriteLine($"  rotation:{rotation:0.00} around {leftSymbolPoint.X:0},{leftSymbolPoint.Y:0}");

            return rotation;
        }

        private double TheorizeScaling(
            CPoint leftIdentifierPoint, CPoint leftSymbolPoint, CPoint rightIdentifierPoint, CPoint rightSymbolPoint)
        {
            var identifierDistance = CMaths.DistanceBetween(rightIdentifierPoint, leftIdentifierPoint);
            var symbolsDistance = CMaths.DistanceBetween(rightSymbolPoint, leftSymbolPoint);
            var scaling = Math.Round(symbolsDistance / identifierDistance, 2);
            if (double.IsNaN(scaling)) throw new InvalidOperationException("An internal error occurred whilst calculating scaling of the image.");

            //writer?.WriteLine($"  scaling:{scaling:0.00}");

            return scaling;
        }

        private double DistanceBetween(JProSymbol identifier, OcrSymbol symbol)
        {
            var identifierPoint = identifier.Rect.CenterPoint;
            var symbolPoint = symbol.Rect.CenterPoint;
            return CMaths.DistanceBetween(identifierPoint, symbolPoint);
        }

        private static CRect NewSearchRect(double maxMoveDistanceX, double maxMoveDistanceY, CPoint point)
        {
            return new CRect
            {
                Left = (point.X - maxMoveDistanceX),
                Top = (point.Y - maxMoveDistanceY),
                Width = maxMoveDistanceX * 2,
                Height = maxMoveDistanceY * 2
            };
        }
    }
}