// 작성자 : 윤정도
// 쓰레드풀에서 실행하기 위한 변경 작업 : 어떤 객체가 먼저 수정될 지 알 수 없음
// 이미지 포맷 변환 대부분이 UpdateMultiJob으로 처리된다.

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

        public UpdateMultiJob(int id, Action<T> action) : base(id, action, Type.UpdateMultiJob)
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
