using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TestWorkService
{
    class Zipper
    {
        public static string Compress(string uncompressedString)
        {
            if (String.IsNullOrEmpty(uncompressedString))
            {
                return uncompressedString;
            }

            using (var compressedStream = new MemoryStream())
            {
                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
                {
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    return Convert.ToBase64String(compressedStream.ToArray());
                }
            }
        }

        public static string Decompress(string compressedString)
        {
            if (String.IsNullOrEmpty(compressedString))
            {
                return compressedString;
            }

            using (var decompressedStream = new MemoryStream())
            {
                using (var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString)))
                {
                    using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompressorStream.CopyTo(decompressedStream);
                    }

                    return Encoding.UTF8.GetString(decompressedStream.ToArray());
                }
            }
        }
    }
}
