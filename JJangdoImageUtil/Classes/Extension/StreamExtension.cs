// 작성자 : 윤정도
// 
// 스트림 확장 기능을 구현합니다.

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
        // 스트림 재사용을 위해 추가
        public static void Reset(this Stream stream)
        {
            stream.Position = 0;
            stream.SetLength(0);
        }
    }
}
