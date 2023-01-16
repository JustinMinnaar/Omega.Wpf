using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

using System.Drawing;

namespace Jem.ProfilingLibrary23
{
    public class JProSymbol
    {
        public JProSymbol(OcrSymbol oSymbol)
        {
            this.Character = oSymbol.Text[0];
            this.Color = oSymbol.Color;
            this.Rect = oSymbol.Rect;
        }

        public char Character { get; }
        public OcrColor Color { get; }
        public CRect Rect { get; }

        public override string ToString() => $"Character:{Character} Rect:{Rect} Color:{Color} ";
    }
}