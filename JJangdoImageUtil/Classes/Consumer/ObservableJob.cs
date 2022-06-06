using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JJangdoImageUtil
{
    public abstract class ObservableJob
    {
        public enum Type
        {
            CreateSingleJob,
            CreateMultiJob,
            UpdateSingleJob,
            UpdateMultiJob
        }

        public enum State
        {
            Initialized,
            Waiting,
            Started,
            Completed,
            Canceled,
            Failed,
            Finished
        }



        protected static int SequenceId = 0;
        protected static Dispatcher UiDispatcher;

        protected readonly string _jobName;
        protected int _id;
        protected int _state;
        protected int _noDelay;
        protected Type _type;
        

        protected int _totalTargetCount;
        protected int _completedTargetCount;
        protected int _failedTargetCount;

        public static void SetDispatcher(Dispatcher dispatcher)
        {
            UiDispatcher = dispatcher;
        }

        public ObservableJob() 
        {
        }

        public ObservableJob(string jobName, Type type)
        {
            _id = Interlocked.Increment(ref SequenceId);
            _jobName = jobName;
            _state = Convert.ToInt32(State.Initialized);
            _type = type;
        }

        public int GetId()
        {
            return _id;
        }


        public void Started(StartedHandler handler, int totalTargetCount = 0)
        {
            if (_type == Type.UpdateSingleJob || _type == Type.UpdateMultiJob)
                Interlocked.Exchange(ref _totalTargetCount, totalTargetCount);

            SetState(State.Started);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
        }

        public bool NoDelay
        {
            get => AtomicOperation.Read(ref _noDelay) == Constant.TRUE;
            set => Interlocked.Exchange(ref _noDelay, Convert.ToInt32(value));
        }

        public void Waiting()
        {
            SetState(State.Waiting);
        }


        public void IncrementCompletedTargetCount()
        {
            Interlocked.Increment(ref _completedTargetCount);
        }

        public void IncrementFailedTargetCount()
        {
            Interlocked.Increment(ref _failedTargetCount);
        }

        public int CompletedTargetCount()
        {
            return AtomicOperation.Read(ref _completedTargetCount);
        }

        public int FailedTargetCount()
        {
            return AtomicOperation.Read(ref _failedTargetCount);
        }

        public void Updated(UpdateHandler handler)
        {
            UiDispatcher.Invoke(() => handler?.Invoke(this));
        }

        public void Finished(FinishedHandler handler)
        {
            SetState(State.Finished);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
        }

        public void Canceled(CancelHandler handler)
        {
            SetState(State.Canceled);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
        }

        public State GetState()
        {
            return (State)AtomicOperation.Read(ref _state);
        }

        public Type GetJobType()
        {
            return _type;
        }

        public void SetState(State state)
        {
            Interlocked.Exchange(ref _state, Convert.ToInt32(state));
        }

        public string GetName()
        {
            return _jobName;
        }

        public bool IsFinished()
        {
            return AtomicOperation.Read(ref _state) == Convert.ToInt32(State.Finished);
        }

        public int TotalTargetCount()
        {
            return AtomicOperation.Read(ref _totalTargetCount);
        }

        public double ProgressRatio()
        {
            return (double)(FailedTargetCount() + CompletedTargetCount()) / TotalTargetCount();
        }

        public bool IsCanceled()
        {
            return AtomicOperation.Read(ref _state) == Convert.ToInt32(State.Canceled);
        }
        public override bool Equals(object obj)
        {
            ObservableJob observableJob = obj as ObservableJob;

            if (observableJob == null)
                return false;

            return _id == observableJob._id;
        }

        public override int GetHashCode()
        {
            int hashCode = -1159337759;
            hashCode = hashCode * -1521134295 + _id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_jobName);
            return hashCode;
        }
    }
}
