using System.Collections.Concurrent;

namespace Jem.OcrLibrary22;

public sealed class OcrFolder
{
    public OcrFolder() { }

    public List<OcrDocument> Documents { get; } = new();

    public OcrDocument? TryGet(string name)
    {
        foreach(var oDocument in Documents)
        {
            if (oDocument.Name == name)
                return oDocument;
        }
        return null;
    }

    public void SaveBinaryFile(string ocrPath)
    {
        using var stream = File.OpenWrite(ocrPath);
        var bw = new BinaryWriter(stream);
        WriteBinary(bw);
    }

    public void WriteBinary(BinaryWriter bw)
    {
        bw.Write(Documents.Count);
        foreach (var oDocument in Documents)
            oDocument.WriteBinary(bw);
    }

    public static OcrFolder? TryLoadBinaryFile(string ocrPath)
    {
        if (File.Exists(ocrPath))
        {
            using var stream = File.OpenRead(ocrPath);
            var br = new BinaryReader(stream);
            var oFolder = new OcrFolder();
            oFolder.ReadBinary(br);
            return oFolder;
        }
        return null;
    }

    public void ReadBinary(BinaryReader br)
    {
        var count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var oDocument = new OcrDocument();
            oDocument.ReadBinary(br);
            Documents.Add(oDocument);   
        }
    }

    //public async Task SaveJsonFileAsync(string jsonOcrPath)
    //{
    //    using var jsonOcrStream = File.OpenWrite(jsonOcrPath);
    //    await JsonSerializer.SerializeAsync(jsonOcrStream, this);
    //}

    //public static async Task<OcrFolder?> TryLoadJsonFileAsync(string jsonOcrPath)
    //{
    //    if (File.Exists(jsonOcrPath))
    //    {
    //        using var jsonOcrStream = File.OpenRead(jsonOcrPath);
    //        var oFolder = await JsonSerializer.DeserializeAsync<OcrFolder>(jsonOcrStream);
    //        return oFolder;
    //    }
    //    return null;
    //}
}
