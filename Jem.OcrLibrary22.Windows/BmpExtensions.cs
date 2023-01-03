using System.Drawing;
using System.Drawing.Imaging;

namespace Jem.OcrLibrary22.Windows;

public static class BmpExtensions
{
    public static void SavePng(this Bitmap bmp, string path)
    {
        // NOTE: GDI+ has issues with attempting too many bitmap/graphics functions in parallel
        var bytes = ToPngBytes(bmp);
        File.WriteAllBytes(path, bytes);
        // bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
    }

    public static byte[] ToPngBytes(this Bitmap bmp)
    {
        using (var ms = new MemoryStream())
        {
            bmp.Save(ms, format: ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
