// 작성자 : 윤정도
// 쓰레드 풀에서 실행되는 작업임을 나타낸다.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    // IOCP에서 처리되는 작업
    public interface IMultiJob
    {
        void WaitOne();
        void Signal();
    }
}
