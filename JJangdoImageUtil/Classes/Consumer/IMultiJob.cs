using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public interface IMultiJob
    {
        void WaitOne();
        void Signal();
    }
}
