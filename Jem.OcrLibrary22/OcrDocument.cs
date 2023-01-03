using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Jem.OcrLibrary22;

public sealed class OcrDocument : OcrElement
{
    private object padlock = new();

    public void Clean()
    {
        foreach (var oPage in Pages)
            oPage.Clean();
    }

    public override string ToString()
    {
        return $"{Path} ({PagesCount} pages) [v{ProcessingVersion}]";
    }

    #region Pages

    [JsonIgnore] public int PagesCount { get => _Pages.Count; }

    public IReadOnlyList<OcrPage> Pages => _Pages; private List<OcrPage> _Pages = new();

    public OcrPage AddPage(OcrPage oPage)
    {
        _Pages.Add(oPage);
        return oPage;
    }

    public override void Compile()
    {
        // note: we don't remove empty pages, but do remove empty blocks, lines, and words.

        var first = true;
        var index = 0;
        foreach (var oPage in Pages)
        {
            oPage.Compile();

            oPage.OwnerDocument = this;
            oPage.PageIndex = index++;

            if (first)
            {
                this.TextDirection = oPage.TextDirection;
                this.Rect = oPage.Rect;
                first = false;
            }
            else
            {
                this.Rect = this.Rect.Union(oPage.Rect);
            }
        }
    }

    #endregion

    #region Properties

    public float ProcessingVersion { get; set; }
    [JsonIgnore] public string? Name => System.IO.Path.GetFileNameWithoutExtension(Path);
    public string? Path { get; set; }
    [JsonIgnore] public int CountPageImages => Pages.Sum(p => p.Images.Count);
    public bool IsIdentified { get; set; }
    public bool IsOptimised { get; set; }
    public string? Engine { get; set; }

    [JsonIgnore] public object? Tag { get; set; }

    #endregion

    #region Json

    public static async Task<OcrDocument> LoadFromJsonFileAsync(string ocrFilePath)
    {
        if (!File.Exists(ocrFilePath))
            throw new FileNotFoundException(ocrFilePath);

        var ocrDocument = await TryLoadFromJsonFileAsync(ocrFilePath);
        if (ocrDocument == null)
            throw new UnknownOcrFormat(ocrFilePath);

        return ocrDocument;
    }

    public static async Task<OcrDocument?> TryLoadFromJsonFileAsync(string ocrFilePath)
    {
        try
        {
            if (!File.Exists(ocrFilePath)) return null;

            var json = await File.ReadAllTextAsync(ocrFilePath);
            return JsonSerializer.Deserialize<OcrDocument>(json);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<OcrDocument> LoadFromBinaryFileAsync(string binOcrFilePath, bool preservePath = false)
    {
        // var folder = AppDomain.CurrentDomain.BaseDirectory;

        if (!File.Exists(binOcrFilePath))
            throw new FileNotFoundException(binOcrFilePath);

        var ocrDocument = await TryLoadFromBinaryFileAsync(binOcrFilePath, preservePath);
        if (ocrDocument == null)
            throw new UnknownOcrFormat(binOcrFilePath);

        return ocrDocument;
    }

    public static async Task<OcrDocument?> TryLoadFromBinaryFileAsync(string binOcrFilePath, bool preservePath = false)
    {
        try
        {
            if (!File.Exists(binOcrFilePath)) return null;

            var bytes = await File.ReadAllBytesAsync(binOcrFilePath);
            using var stream = new MemoryStream(bytes);
            using var br = new BinaryReader(stream);

            var ocrDocument = new OcrDocument();
            ocrDocument.ReadBinary(br);
            ocrDocument.Compile();

            if (!preservePath && ocrDocument.Path != binOcrFilePath)
            {
                ocrDocument.Path = binOcrFilePath;
            }

            return ocrDocument;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region ToText

    public override void ToText(StringBuilder sb)
    {
        foreach (var page in Pages)
        {
            page.ToText(sb);
            sb.AppendLine(); // extra blank line between pages
        }
    }

    #endregion

    #region Binary

    public override void ReadBinary(BinaryReader br)
    {
        var format = br.ReadString();
        if (format != nameof(OcrDocument))
            throw new UnknownOcrFormat($"Unknown format '{format}' for {nameof(OcrDocument)}!");

        var version = br.ReadByte();
        if (version > 6)
            throw new UnknownOcrFormat($"Version '{version}' not supported for {nameof(OcrDocument)}!");

        if (version >= 4)
        {
            ProcessingVersion = br.ReadSingle();
        }

        if (version>= 6)
            base.ReadBinary2(br);
        else
            base.ReadBinary(br);

        var hasPath = br.ReadBoolean();
        if (hasPath) Path = br.ReadString();

        if (version >= 2)
        {
            var hasEngine = br.ReadBoolean();
            if (hasEngine) Engine = br.ReadString();
        }

        var pageCount = br.ReadInt32();
        if (pageCount > 65535)
            throw new UnknownOcrFormat($"Page count {pageCount} greater than 65535!");

        for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
        {
            var page = new OcrPage();
            page.ReadBinary(br);
            AddPage(page);
        }

        if (version >= 5)
        {
            this.IsOptimised = br.ReadBoolean();
            this.IsIdentified = br.ReadBoolean();
        }
    }

    public override void WriteBinary(BinaryWriter bw)
    {
        byte version = 6;

        bw.Write(nameof(OcrDocument));
        bw.Write(version);
        if (version >= 4)
        {
            bw.Write(ProcessingVersion);
        }

        if (version >= 6)
            base.WriteBinary2(bw);
        else
            base.WriteBinary(bw);

        bw.Write(Path != null);
        if (Path != null) bw.Write(Path);

        if (version >= 2)
        {
            if (Engine == null) bw.Write(false);
            else { bw.Write(true); bw.Write(Engine); }
        }

        bw.Write(Pages.Count);
        foreach (var page in Pages)
        {
            page.WriteBinary(bw);
        }

        if (version >= 5)
        {
            bw.Write(IsOptimised);
            bw.Write(IsIdentified);
        }
    }

    public async Task SaveToBinaryFileAsync(string ocrFilePath)
    {
        if (cantSaveAfterResize)
            throw new OcrException($"Ocr has been resized, can't save to '{ocrFilePath}'!");

        var stream = new MemoryStream();
        var bs = new BinaryWriter(stream);
        WriteBinary(bs);

        await File.WriteAllBytesAsync(ocrFilePath, stream.ToArray());
    }

    #endregion

    public OcrPage AddTestPage()
    {
        var oPage = new OcrPage() { Size = new CSize(2100, 3100) };
        AddPage(oPage);
        return oPage;
    }

    public void RotatePages(double angleDegrees)
    {
        foreach (var page in Pages)
        {
            page.Rotate(angleDegrees);
        }
    }

    private bool cantSaveAfterResize;

    public void ResizeTo2100()
    {
        cantSaveAfterResize = true;
        foreach (var page in Pages)
        {
            page.ResizeTo2100();
        }
        Compile();
    }
}
