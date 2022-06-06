using System;

namespace JJangdoImageUtil
{
    public class UpdateSingleJob<T> : ObservableJobUpdate<T> where T : class
    {
        public UpdateSingleJob(string jobName, Action<T> action) : base(jobName, action, Type.UpdateSingleJob)
        {
        }
    }
}
