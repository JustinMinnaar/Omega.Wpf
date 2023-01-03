namespace Jem.CommonLibrary22;

public static class FileExtensions
{
    public static bool ExistsAndNewerThan(this string path1, string path2)
    {
        var path1Exists = File.Exists(path1);
        var path2Exists = File.Exists(path2);
        if (!path1Exists) return false;

        if (path2Exists)
        {
            var fi1 = new FileInfo(path1);
            var fi2 = new FileInfo(path2);
            if (fi1.LastWriteTime < fi2.LastWriteTime) return false;
        }
        return true;
    }
}
