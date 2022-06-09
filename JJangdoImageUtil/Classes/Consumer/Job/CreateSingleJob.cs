using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class CreateSingleJob<T> : ObservableJobCreate<T> where T : class
    {
        public CreateSingleJob(int id, List<Func<T>> funcs) : base(id, funcs, Type.CreateSingleJob)
        {
        }
    }
}
