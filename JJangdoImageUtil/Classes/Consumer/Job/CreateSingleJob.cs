// 작성자 : 윤정도
// 단순 외부 쓰레드에서 실행하는 생성 작업 : 순차실행을 보장

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class CreateSingleJob<T> : ObservableJobCreate<T> where T : class, IWpfEntity
    {
        public CreateSingleJob(int id, List<Func<T>> funcs) : base(id, funcs, Type.CreateSingleJob)
        {
        }
    }
}
