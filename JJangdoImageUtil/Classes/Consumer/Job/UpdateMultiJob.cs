using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class UpdateMultiJob<T> : ObservableJobUpdate<T>, IMultiJob where T : class
    {
        private readonly AutoResetEvent _waitHandle;

        public UpdateMultiJob(string jobName, Action<T> action) : base(jobName, action, Type.UpdateMultiJob)
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
