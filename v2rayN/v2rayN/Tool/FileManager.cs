using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace v2rayN.Tool
{
    class FileManager
    {
        public static void UncompressFile(string fileName, byte[] content)
        {
            // Because the uncompressed size of the file is unknown,
            // we are using an arbitrary buffer size.
            byte[] buffer = new byte[4096];
            int n;

            using (var fs = File.Create(fileName))
            using (var input = new GZipStream(new MemoryStream(content),
                CompressionMode.Decompress, false))
            {
                while ((n = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, n);
                }
            }
        }
    }
}
