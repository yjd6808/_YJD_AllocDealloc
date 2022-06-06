using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class CreateMultiJob<T> : ObservableJobCreate<T>, IMultiJob where T : class
    {
        private readonly AutoResetEvent _waitHandle;

        public CreateMultiJob(string jobName, List<Func<T>> funcs) : base(jobName, funcs, Type.CreateMultiJob)
        {
            _waitHandle = new AutoResetEvent(false);
        }


        public void WaitOne()
        {
            _waitHandle.WaitOne();
        }

        public void Signal()
        {
            _waitHandle.Set();
        }
    }
}
