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
