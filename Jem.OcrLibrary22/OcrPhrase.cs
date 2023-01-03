namespace Jem.OcrLibrary22
{
    public sealed class OcrPhrase
    {
        public OcrPhrase()
        {
            Symbols = new();
            Text = "";
        }

        public OcrPhrase(List<OcrSymbol> symbols, string text)
        {
            Symbols = symbols; // .OrderBy(s=>s.Rect.Left).ToList();
            Bounds = symbols.Bounds();
            Text = text;
        }

        public bool IsEmpty => Symbols.Count == 0 || Text.Length == 0;
        public List<OcrSymbol> Symbols { get; }
        public CRect Bounds { get; }
        public string Text { get; }

        public string? Extract(string? lead = null, string? follow = null)
        {
            if (Text == null) return null;

            var iBegin = 0;
            if (lead != null)
            {
                iBegin = Text.IndexOf(lead);
                if (iBegin != -1) iBegin += lead.Length; else return null;
            }

            var iEnd = Text.Length;
            if (follow != null)
            {
                iEnd = Text.IndexOf(follow, iBegin);
                if (iEnd == -1) return null;
            }

            return Text[iBegin..iEnd];
        }

        public OcrPhrase? ExtractSymbols(double left, double right, float maxSymbolHeight = OcrPage.DEFAULT_MAX_SYMBOL_HEIGHT, string? validCharacters = null)
        {
            return ExtractSymbols(new CRect(left, 0, right - left, double.MaxValue), maxSymbolHeight, validCharacters);
        }

        public OcrPhrase? ExtractSymbols(CRect rect, float maxSymbolHeight = OcrPage.DEFAULT_MAX_SYMBOL_HEIGHT, string? validCharacters = null)
        {
            var symbols = new List<OcrSymbol>();
            var sb = new StringBuilder();

            bool beginWord = true;
            foreach (var symbol in Symbols)
            {
                if (symbol.Rect.Right < rect.Left) continue;
                if (symbol.Rect.Left > rect.Right) continue;

                if (symbol.Rect.Bottom < rect.Top) continue;
                if (symbol.Rect.Top > rect.Bottom) continue;

                if (symbol.Rect.Height > maxSymbolHeight) continue;

                if (beginWord && sb.Length > 0 && sb[sb.Length - 1] != ' ')
                    if (validCharacters == null || validCharacters.Contains(' '))
                        sb.Append(' ');
                beginWord = false;

                if (validCharacters == null || validCharacters.Contains(symbol.Text))
                {
                    sb.Append(symbol.Text);
                    symbols.Add(symbol);
                }
            }

            if (symbols.Count > 0)
                return new(symbols, sb.ToString());
            else
                return null;
        }

        public override string? ToString()
        { return Text; }
    }
}