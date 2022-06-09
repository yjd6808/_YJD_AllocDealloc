// 작성자 : 윤정도
// 쓰레드풀에서 실행하기 위한 생성 작업 : 어떤 객체가 먼저 생성될지 알 수 없음

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

        public CreateMultiJob(int id, List<Func<T>> funcs) : base(id, funcs, Type.CreateMultiJob)
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
