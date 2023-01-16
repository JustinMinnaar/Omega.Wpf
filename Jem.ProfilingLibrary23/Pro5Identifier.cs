//namespace Jem.ProfilingLibrary23;

///// <summary>Held by JProfilePhrase, identifiers are used to align stencils to the OCR results.</summary>
//public class OcrSymbol
//{
//    public override string ToString()
//    {
//        var text = $"{Rect}";
//        if (MatchCharacter >= 32) text += $"'{MatchCharacter}'";
//        if (MatchCharacters != null) text += $" '{MatchCharacters}'";
//        if (MatchAny) text += " any";
//        if (MatchAnyDigit) text += " digits";
//        if (MatchAnyUpper) text += " upper";
//        if (MatchAnyLower) text += " lower";
//        return text;
//    }

//    public OcrSymbol()
//    {
//    }

//    public OcrSymbol(CRect rect, char character)
//    {
//        this.Rect = rect;
//        this.MatchCharacter = character;
//    }

//    public OcrSymbol(CRect rect,
//        string matchCharacters = null,
//        bool matchAny = false,
//        bool matchAnyUpper = false,
//        bool matchAnyLower = false,
//        bool matchAnyDigit = false)
//    {
//        Rect = rect;
//        MatchAny = matchAny;
//        MatchCharacters = matchCharacters;
//        MatchAnyLower = matchAnyLower;
//        MatchAnyUpper = matchAnyUpper;
//        MatchAnyDigit = matchAnyDigit;
//    }

//    public bool Matches(char character)
//    {
//        if (MatchCharacter == character) return true;

//        if (MatchAny && character > (char)32) return true;
//        if (MatchAnyDigit && char.IsDigit(character)) return true;
//        if (MatchAnyLower && char.IsLower(character)) return true;
//        if (MatchAnyUpper && char.IsUpper(character)) return true;
//        if (MatchCharacters != null && MatchCharacters.IndexOf(character) >= 0) return true;

//        return false;
//    }

//    public char MatchCharacter
//    {
//        get => MatchCharacters != null && MatchCharacters.Length > 0 ? MatchCharacters[0] : (char)0;
//        set => MatchCharacters = value.ToString();
//    }

//    /// <summary>If not null, any characters considered a valid match for the identifier's character.
//    /// Whilst this is usually just one character, it could be a range of punc or other characters.
//    /// Use the Any... for faster matching of letters and digits.</summary>
//    public string MatchCharacters { get; set; }

//    /// <summary>If true, any uppercase letter is considered a match for this identifier's character.</summary>
//    public bool MatchAnyUpper { get; set; }

//    /// <summary>If true, any lowercase letter is considered a match for this identifier's character.</summary>
//    public bool MatchAnyLower { get; set; }

//    /// <summary>If true, any character is considered a match for this identifier's character.</summary>
//    public bool MatchAny { get; set; }

//    /// <summary>If true, any digit is considered a match for this identifier's character.</summary>
//    public bool MatchAnyDigit { get; set; }

//    /// <summary>Regex string to match on, if not null. This allows matching any character that fits the range.</summary>
//    public string Regex { get; set; }

//    /// <summary>The rectangle that encloses this symbol.</summary>
//    public CRect Rect { get; set; }

//    /// <summary>The baseline is the rectangle without descenders, calculated by ocr.</summary>
//    public CRect BaseRect { get; set; }

//    public bool Important { get; set; }

//    //public void WarpDown(float leftHeight, float rightHeight)
//    //{
//    //    var block = Word.Block;
//    //    var blockRect = block.Rect;

//    //    var xScale = (Rect.Left - blockRect.Left) / blockRect.Width;

//    //    var leftToApply = leftHeight - leftHeight * xScale;
//    //    var rightToApply = rightHeight - rightHeight * (1 - xScale);

//    //    var yValue = leftToApply + rightToApply;

//    //    var newRect = new CRect(Rect.Left, Rect.Top + yValue, Rect.Width, Rect.Height);

//    //    Rect = newRect;
//    //}
//}