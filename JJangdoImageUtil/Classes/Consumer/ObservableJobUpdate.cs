// 작성자 : 윤정도
// ObservableCollection에 대해서 수행해야하는 생성 작업
// 객체 변경을 비동기적으로 수행해줌

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
