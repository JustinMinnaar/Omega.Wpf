namespace Jem.Profiling22.Data
{
    public class ProStamps : List<ProStamp>
    {
        public ProStamp CreateStamp(string name, OcrPage sourcePage, CRect sourceRect, CSize allowMovement)
        {
            var stamp = new ProStamp(name, allowMovement);
            var symbols = sourcePage.ExtractSymbolsAndText(sourceRect)?.Symbols;
            if (symbols != null)
            {
                foreach (var symbol in symbols)
                {
                    if (char.IsDigit(symbol.Text[0])) symbol.Text = "#";
                }
                stamp.AddSymbols(symbols);
            }
            this.Add(stamp);
            return stamp;
        }
    }
}