using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace JJangdoImageUtil
{
    public static class StreamExtension
    {
        public static void Reset(this Stream stream)
        {
            stream.Position = 0;
            stream.SetLength(0);
        }
    }
}
