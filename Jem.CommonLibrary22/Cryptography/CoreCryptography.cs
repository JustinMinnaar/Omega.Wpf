using System.Security.Cryptography;

namespace Jem.CommonLibrary22;

public static class CoreCryptography
{
    public static string GetSha512Buffered(this Stream streamIn)
    {
        //Process process = Process.GetCurrentProcess();

        const int bufferSizeForMd5Hash = 1024 * 1024 * 8; // 8MB

        //Console.WriteLine("-----------------------------------");
        //Console.WriteLine("Memory Buffer Size {0} MB", bufferSizeForMd5Hash / 1000);
        //Console.WriteLine("-----------------------------------");

        string hashString;
        using var md5Prov = SHA256.Create();

        int readCount;
        long bytesTransfered = 0;
        var buffer = new byte[bufferSizeForMd5Hash];
        while ((readCount = streamIn.Read(buffer, 0, buffer.Length)) != 0)
        {
            // Need to figure out if this is final block
            if (bytesTransfered + readCount == streamIn.Length)
            {
                md5Prov.TransformFinalBlock(buffer, 0, readCount);
            }
            else
            {
                md5Prov.TransformBlock(buffer, 0, bufferSizeForMd5Hash, buffer, 0);
            }
            bytesTransfered += readCount;
            //Console.WriteLine("GetSha512Buffered:{0}MB/{1}MB.   Memory Used: {2}MB",
            //                    bytesTransfered / 1000000,
            //                    streamIn.Length / 1000000,
            //                    process.PrivateMemorySize64 / 1000000);
        }
        if (md5Prov.Hash == null)
            throw new ArgumentNullException($"MD5 returned null!");

        hashString = BitConverter.ToString(md5Prov.Hash).Replace("-", String.Empty);
        md5Prov.Clear();

        return hashString;
    }
}