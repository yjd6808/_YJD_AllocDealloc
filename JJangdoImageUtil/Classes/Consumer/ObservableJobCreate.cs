using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public abstract class ObservableJobCreate<T> : ObservableJob where T : class
    {
        protected List<Func<T>> _funcs;

        protected ObservableJobCreate(string jobName, List<Func<T>> funcs, Type type) : base(jobName, type)
        {
            _funcs = funcs;
            _totalTargetCount = funcs.Count;
        }

        public T RunCreationAction<T>(int idx, out State state) where T : class
        {
            if (IsCanceled())
            {
                state = State.Canceled;
                return null;
            }

            try
            {
                var result = _funcs[idx]() as T;

                if (result != null)
                    state = State.Completed;

                state = State.Failed;
                return result;
            }
            catch
            {
                state = State.Failed;
                return null;
            }
        }
    }
}
