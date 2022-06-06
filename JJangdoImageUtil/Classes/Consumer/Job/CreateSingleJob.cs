using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public class CreateSingleJob<T> : ObservableJobCreate<T> where T : class
    {
        public CreateSingleJob(string jobName, List<Func<T>> funcs) : base(jobName, funcs, Type.CreateSingleJob)
        {
        }
    }
}
