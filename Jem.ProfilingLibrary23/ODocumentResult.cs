using Jem.CommonLibrary22;
using Jem.OcrLibrary22;

namespace Jem.Profiling22;

public class ODocumentResult
{
    public ODocumentResult(OcrDocument oDocument, CompiledOcrDocument cDocument, JProProfileResult?[] pageResults, string? source)
    {
        this.ODocument = oDocument;
        this.CDocument = cDocument;

        this.Source = source;
        this.PageResults = pageResults;
    }

    public string? Source { get; }
    public JProProfileResult?[] PageResults { get; }

    public Dictionary<string, string> Values { get; } = new();
    public string? TryGetValue(string key)
    {
        Values.TryGetValue(key, out var value);
        return value;
    }

    public void WriteTo(IWriter? writer)
    {
        var lines = GetLines();
        foreach (var line in lines)
            writer?.WriteLine(line);

        //foreach (var rPage in this.PageResults)
        //{
        //    if (rPage == null) continue;

        //    foreach (var rTemplate in rPage.TemplateResults)
        //    {
        //        foreach (var row in rTemplate.RowResults)
        //            writer?.WriteLine(row.ToString());
        //    }
        //}
    }

    public bool CompareToFile()
    {
        var path = ODocument.Path + ".results.txt";
        if (!File.Exists(path)) return false;

        var expected = File.ReadAllLines(path);
        var actual = GetLines().ToArray();

        for (int i = 0; i < actual.Length; i++)
        {
            var a = actual[i];
            var e = i < expected.Length ? expected[i] : null;
            if (a != e) return false;

            // "    /00 PageNumber='1' PageCount='4' AccountNumber='62406061507' StatementDate='21/02/08' "
            // "        /00 PageNumber='1' PageCount='4' AccountNumber='62406061507' StatementDate='21/02/08' "
        }
        return true;
    }

    public void SaveToFileIfMissing()
    {
        var path = ODocument.Path + ".results.txt";
        if (File.Exists(path)) return;

        SaveToFile(path);
    }

    public void SaveToFile()
    {
        var path = ODocument.Path + ".results.txt";
        SaveToFile(path);
    }

    public void SaveToFile(string path)
    {
        var lines = GetLines().ToArray();
        File.WriteAllLines(path, lines);
    }

    public IEnumerable<string> GetLines()
    {
        foreach (var rPage in this.PageResults)
        {
            if (rPage == null) continue;

            var t = $"PageIndex='{rPage.PageIndex}' Profile='{rPage.Profile?.Name}' Ocr='{this.ODocument.Path}'";
            var underline = new string('-', t.Length);

            yield return t;
            yield return underline;

            foreach (var rTemplate in rPage.TemplateResults)
            {
                foreach (var row in rTemplate.RowResults)
                {
                    //var line = row.ToStringParts();
                    var lines = row.ToString().Split("\r\n");
                    foreach (var line in lines)
                        yield return line;
                }
            }
            yield return "";
        }
    }

    public List<string> Errors { get; } = new();
    public OcrDocument ODocument { get; }
    public CompiledOcrDocument CDocument { get; }
}

