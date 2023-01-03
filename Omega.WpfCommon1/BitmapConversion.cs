using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Omega.WpfCommon1;

public static class BitmapConversion
{
    public static Bitmap? TryToWinFormsBitmap(this BitmapSource bitmapSource)
    {
        try
        {
            return ToWinFormsBitmap(bitmapSource);
        }
        catch { return null; }
    }

    public static Bitmap ToWinFormsBitmap(this BitmapSource bitmapSource)
    {
        using MemoryStream stream = new();
        BitmapEncoder enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bitmapSource));
        enc.Save(stream);

        using var tempBitmap = new Bitmap(stream);

        // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
        // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
        return new Bitmap(tempBitmap);
    }

    public static BitmapImage? TryToWpfBitmap(this Bitmap bitmap)
    {
        try
        {
            return ToWpfBitmap(bitmap);
        }
        catch { return null; }
    }

    public static BitmapImage ToWpfBitmap(this Bitmap bitmap)
    {
        using MemoryStream stream = new();
        bitmap.Save(stream, ImageFormat.Png);
        return ToWpfBitmap(stream);
    }

    public static async Task<BitmapImage> AsFilePathToWpfBitmapAsync(this string filename)
    {
        var bytes = await File.ReadAllBytesAsync(filename);
        using MemoryStream stream = new(bytes);
        return stream.ToWpfBitmap();
    }

    public static BitmapImage ToWpfBitmap(this MemoryStream stream)
    {
        stream.Position = 0;

        BitmapImage result = new();
        result.BeginInit();
        // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
        // Force the bitmap to load right now so we can dispose the stream.
        result.CacheOption = BitmapCacheOption.OnLoad;
        result.StreamSource = stream;
        result.EndInit();
        result.Freeze();
        return result;
    }
}
