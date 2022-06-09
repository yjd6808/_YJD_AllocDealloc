// 작성자 : 윤정도
// ObservableCollection에 대해서 수행해야하는 생성 작업
// 객체 생성을 비동기적으로 수행해줌

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

        protected ObservableJobCreate(int id, List<Func<T>> funcs, Type type) : base(id, type)
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
