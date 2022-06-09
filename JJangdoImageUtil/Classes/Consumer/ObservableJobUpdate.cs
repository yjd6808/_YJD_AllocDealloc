using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public abstract class ObservableJobUpdate<T> : ObservableJob
    {
        protected Action<T> _action;

        protected ObservableJobUpdate(int id, Action<T> action, Type type) : base(id, type)
        {
            _action = action;
        }

        public State RunUpdateAction(T target)
        {
            if (IsCanceled())
                return State.Canceled;

            try
            {
                _action(target);
                return State.Completed;
            }
            catch
            {
                return State.Failed;
            }
        }
    }
}
