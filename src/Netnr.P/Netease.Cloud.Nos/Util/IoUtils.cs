using System;
using System.IO;

namespace Netease.Cloud.NOS.Util
{
    internal static class IoUtils
    {
        private const int BufferSize = 4 * 1024;

        public static long WriteTo(Stream source, Stream dest, long totalSize)
        {
            var buffer = new byte[BufferSize];

            long alreadyRead = 0;
            while (alreadyRead < totalSize)
            {
                var readSize = source.Read(buffer, 0, BufferSize);
                if (readSize < 0)
                    break;
                if (alreadyRead + readSize > totalSize)
                    readSize = (int)(totalSize - alreadyRead);
                alreadyRead += readSize;
                dest.Write(buffer, 0, readSize);
            }
            dest.Flush();

            return alreadyRead;
        }
    }
}
