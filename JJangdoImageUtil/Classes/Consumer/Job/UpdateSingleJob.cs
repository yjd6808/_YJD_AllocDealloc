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
