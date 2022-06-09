// 작성자 : 윤정도
// 단순 외부 쓰레드에서 실행하는 변경 작업 : 순차실행을 보장
// 예를들어 한번에 하나의 타겟에 대해서만 작업을 진행하기 때문에 
// 객체간의 독립성이 보장된다.

using System;

namespace JJangdoImageUtil
{
    public class UpdateSingleJob<T> : ObservableJobUpdate<T> where T : class
    {
        public UpdateSingleJob(int id, Action<T> action) : base(id, action, Type.UpdateSingleJob)
        {
        }
    }
}
