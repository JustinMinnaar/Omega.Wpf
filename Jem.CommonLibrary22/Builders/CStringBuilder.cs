namespace Jem.CommonLibrary22;

/// <summary>Used to build multi-line XML, C# or DEF with or without indentation.</summary>
public class CStringBuilder
{
    public CStringBuilder(bool flatten = false, int indentSize = 2)
    {
        this.flatten = flatten;
        IndentSize = indentSize;
        sb = new StringBuilder();
    }

    public CStringBuilder(StringBuilder sb, bool flatten = false, int indentSize = 2)
        : this(flatten, indentSize)
    {
        this.sb = sb;
    }

    protected char lastChar;
    protected bool flatten { get; private set; }
    protected StringBuilder sb { get; private set; }
    protected bool begin { get; private set; } = true;
    protected int indent { get; private set; } = 0;
    protected int IndentSize { get; set; } = 2;

    public void Append(string? text)
    {
        Begin();
        if (text != null)
        {
            sb.Append(text);
            lastChar = text[text.Length - 1];
        }
    }

    public void AppendLine(string? text = null)
    {
        Begin();
        if (flatten)
        {
            if (text != null) { Append(text); lastChar = text[text.Length - 1]; }
        }
        else
        {
            sb.AppendLine(text);
            lastChar = '\n';
        }
        End();
    }

    public void Indent()
    {
        indent++;
    }

    public void Indent(string text)
    {
        Append(text); indent++;
    }

    public void IndentLine()
    {
        AppendLine(); indent++;
    }

    public void IndentLine(string text)
    {
        AppendLine(text); indent++;
    }

    public void Outdent()
    {
        indent--;
    }

    public void Outdent(string text)
    {
        indent--; Append(text);
    }

    public void OutdentLine()
    {
        indent--; AppendLine();
    }

    public void OutdentLine(string text)
    {
        indent--; AppendLine(text);
    }

    private void Begin()
    {
        if (!begin) return;
        if (indent > 0 && !flatten)
            sb.Append(new string(' ', indent * IndentSize));
        else
            if (flatten && char.IsLetterOrDigit(lastChar)) sb.Append(' ');
        begin = false;
    }

    private void End()
    { begin = true; }

    public override string ToString()
    {
        return sb.ToString();
    }
}