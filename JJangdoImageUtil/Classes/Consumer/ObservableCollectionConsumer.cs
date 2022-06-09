using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using MoreLinq.Experimental;
using MoreLinq.Extensions;

namespace JJangdoImageUtil
{
    public delegate void StartedHandler(ObservableJob startedJob);
    public delegate void FinishedHandler(ObservableJob finishedJob);
    public delegate void CancelHandler(ObservableJob canceledJob);
    public delegate void UpdateHandler(ObservableJob doingJob);

    public class ObservableCollectionConsumer<T> where T : class
    {
        public enum State
        {
            Initialized,
            Waiting,
            Processing,
            Blocked,
            JoinWait,
            Joined
        }

        private readonly Dispatcher _dispatcher;
        private readonly Thread _consumerThread;            // 작업 처리용 쓰레드
        private int _delay;                                 // 작업 처리단위당 쓰레드 휴식 시간 (간지전용 필요없긴함)
        private int _state;                                 // Consumer 상태
        private ObservableCollection<T> _collection;        // 작업 처리 대상 리스트
        private LinkedList<ObservableJob> _jobQueue;        // 처리 대기중인 작업들
        private ObservableJob _currentJob;                  // 현재 처리중인 작업
        private AutoResetEvent _waitHandle;                 // Consumer 멈출 때 사용하는 녀석
        private object _consumerlock;
        private int _multiCounter;

        public event EventHandler OnEnqueueJob;
        public event StartedHandler OnStartedJob;
        public event CancelHandler OnCanceledJob;
        public event FinishedHandler OnFinishedJob;
        public event UpdateHandler OnUpdatedJob;

    
        public ObservableCollectionConsumer(int delay, ObservableCollection<T> collection, Dispatcher dispatcher)
        {
            _delay = delay;
            _collection = collection;
            _consumerThread = new Thread(ConsumerThread) { IsBackground = true };
            _jobQueue = new LinkedList<ObservableJob>();
            _waitHandle = new AutoResetEvent(false);
            _consumerlock = new object();
            _state = Convert.ToInt32(State.Initialized);
            _dispatcher = dispatcher;

            ObservableJob.SetDispatcher(_dispatcher);
            ThreadPool.SetMinThreads(0, 8);
            ThreadPool.SetMaxThreads(4, 8);
        }

        public int WaitingJobCount
        {
            get
            {
                lock (_consumerlock)
                {
                    return _jobQueue.Count;
                }
            }
        }

        public int JobCount
        {
            get
            {
                lock (_consumerlock)
                {
                    if (_currentJob == null)
                        return _jobQueue.Count;

                    return 1 + _jobQueue.Count;
                }
            }
        }

        public void Start()
        {
            _consumerThread.Start();
        }

        public void Stop()
        {
            if (GetState() >= State.JoinWait)
                return;

            if (_consumerThread.ThreadState == ThreadState.Stopped)
                return;

            CancelAllJobs();

            SetState(State.JoinWait);
            _waitHandle.Set();
        }

        public State GetState()
        {
            return (State)AtomicOperation.Read(ref _state);
        }

        private void SetState(State state)
        {
            Interlocked.Exchange(ref _state, Convert.ToInt32(state));
        }
        
        
        public bool IsProcessing()
        {
            return GetState() == State.Processing;
        }

        public void EnqueueJob(ObservableJob job)
        {
            job.Waiting();

            lock (_consumerlock)
            {
                _jobQueue.AddLast(job);
            }

            _dispatcher.Invoke(() => OnEnqueueJob?.Invoke(this, null));
            _waitHandle.Set();
        }

        public void CancelJobById(int jobId)
        {
            lock (_consumerlock)
            {
                if (_currentJob != null && _currentJob.GetUniqueId() == jobId)
                {
                    _currentJob.SetState(ObservableJob.State.Canceled);
                    return;
                }

                for (var curNode = _jobQueue.First; curNode != null;)
                {
                    var nextNode = curNode.Next;

                    if (curNode.Value.GetUniqueId() == jobId)
                    {
                        curNode.Value.Canceled(OnCanceledJob);
                        _jobQueue.Remove(curNode);
                    }

                    curNode = nextNode;
                }
            }
        }

        public void CancelAllJobs()
        {
            lock (_consumerlock)
            {
                _currentJob?.SetState(ObservableJob.State.Canceled);

                foreach (var waitingJob in _jobQueue)
                    waitingJob.Canceled(OnCanceledJob);

                _jobQueue.Clear();
            }
        }

        private void Sleep(int milisecond)
        {
            SetState(State.Blocked);
            Thread.Sleep(milisecond);
            SetState(State.Processing);
        }

        public void ConsumerThread()
        {
            while (true)
            {
                SetState(State.Waiting);
                _waitHandle.WaitOne();

                while (true)
                {
                    if (GetState() == State.JoinWait)
                        goto THREAD_END;

                    lock (_consumerlock)
                    {
                        if (_jobQueue.Count == 0)
                            break;

                        _currentJob = _jobQueue.First();
                        _jobQueue.RemoveFirst();
                    }

                    SetState(State.Processing);

                    switch (_currentJob.GetJobType())
                    {
                        case ObservableJob.Type.UpdateSingleJob:
                        case ObservableJob.Type.UpdateMultiJob:
                            _currentJob.Started(OnStartedJob, _collection.Count);
                            ProcessUpdateJob();
                            break;
                        case ObservableJob.Type.CreateSingleJob:
                        case ObservableJob.Type.CreateMultiJob:
                            _currentJob.Started(OnStartedJob);
                            ProcessCreateJob();
                            break;
                    }


                    lock (_consumerlock)
                        _currentJob = null;
                }

                // 핸들 대기 상태로 전환
                _waitHandle.Reset();
            }

        THREAD_END:
            SetState(State.Joined);
        }

        private void ProcessCreateJob()
        {
            switch (_currentJob.GetJobType())
            {
                case ObservableJob.Type.CreateSingleJob:
                    ProcessCreateSingleJob();
                    break;
                case ObservableJob.Type.CreateMultiJob:
                    ProcessCreateMultiJob();
                    break;
            }
        }

        private void ProcessCreateMultiJob()
        {
            var createMultiJob = _currentJob as CreateMultiJob<T>;

            if (createMultiJob == null)
                throw new Exception("createMultiJob == null");

            Interlocked.Exchange(ref _multiCounter, 0);
            int totalCount = createMultiJob.TotalTargetCount;

            for (int i = 0; i < totalCount; i++)
                ThreadPool.QueueUserWorkItem(ProcessCreateMultiJobProcedure, i);

            if (totalCount > 0)
                createMultiJob.WaitOne();

            ProcessJobEnd();
        }

        private void ProcessCreateMultiJobProcedure(object funcIdx)
        {
            var currentMultiJob = _currentJob as CreateMultiJob<T>;
            var idx = (int)funcIdx;

            try
            {
                if (currentMultiJob.IsCanceled())
                    return;

                ObservableJob.State state;
                T result = currentMultiJob.RunCreationAction<T>(idx, out state);

                switch (state)
                {
                    case ObservableJob.State.Completed:
                        _currentJob.IncrementCompletedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                    case ObservableJob.State.Failed:
                        _currentJob.IncrementFailedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                }

                if (result != null)
                    _dispatcher.Invoke(() => _collection.Add(result));

                if (state == ObservableJob.State.Canceled)
                    return;

                // 작업이 중단되었거나 모두 처리된 경우
                if (!_currentJob.NoDelay)
                    Thread.Sleep(_delay);
            }
            finally
            {
                Interlocked.Increment(ref _multiCounter);

                if (AtomicOperation.Read(ref _multiCounter) >= currentMultiJob.TotalTargetCount)
                    currentMultiJob.Signal();
            }
        }

        private void ProcessCreateSingleJob()
        {
            var createSingleJob = _currentJob as CreateSingleJob<T>;

            if (createSingleJob == null)
                throw new Exception("createSingleJob == null");

            int totalCount = createSingleJob.TotalTargetCount;

            for (int i = 0; i < totalCount; i++)
            {
                ObservableJob.State state;
                T result = createSingleJob.RunCreationAction<T>(i, out state);

                switch (state)
                {
                    case ObservableJob.State.Completed:
                        _currentJob.IncrementCompletedTargetCount();
                        break;
                    case ObservableJob.State.Failed:
                        _currentJob.IncrementFailedTargetCount();
                        break;
                    case ObservableJob.State.Canceled:
                        goto FINISHED;
                }

                if (result != null)
                    _dispatcher.Invoke(() => _collection.Add(result));

                if (!_currentJob.NoDelay)
                    Sleep(_delay);
            }

            FINISHED:
                ProcessJobEnd();
        }

        private void ProcessUpdateJob()
        {
            switch (_currentJob.GetJobType())
            {
                case ObservableJob.Type.UpdateSingleJob:
                    ProcessUpdateSingleJob();
                    break;
                case ObservableJob.Type.UpdateMultiJob:
                    ProcessUpdateMultiJob();
                    break;
            }
        }

        private void ProcessUpdateSingleJob()
        {
            var updateSingleJob = _currentJob as UpdateSingleJob<T>;

            if (updateSingleJob == null)
                throw new Exception("updateSingleJob == null");

            foreach (T target in _collection)
            {
                var state = updateSingleJob.RunUpdateAction(target);

                switch (state)
                {
                    case ObservableJob.State.Completed:
                        _currentJob.IncrementCompletedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                    case ObservableJob.State.Failed:
                        _currentJob.IncrementFailedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                    case ObservableJob.State.Canceled:
                        goto FINISHED;
                }

                

                if (!_currentJob.NoDelay)
                    Sleep(_delay);
            }

        FINISHED:
            ProcessJobEnd();
        }

       

        private void ProcessUpdateMultiJob()
        {
            var updateMultiJob = _currentJob as UpdateMultiJob<T>;

            if (updateMultiJob == null)
                throw new Exception("updateMultiJob == null");

            Interlocked.Exchange(ref _multiCounter, 0);

            if (_collection.Count > 0)
            {
                foreach (T target in _collection)
                {
                    ThreadPool.QueueUserWorkItem(ProcessUpdateMultiJobProcedure, target);
                }

                updateMultiJob.WaitOne();
            }

            ProcessJobEnd();
        }

        private void ProcessUpdateMultiJobProcedure(object target)
        {
            var currentMultiJob = _currentJob as UpdateMultiJob<T>;

            try
            {
                if (currentMultiJob.IsCanceled())
                    return;

                ObservableJob.State state = currentMultiJob.RunUpdateAction(target as T);

                switch (state)
                {
                    case ObservableJob.State.Completed:
                        _currentJob.IncrementCompletedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                    case ObservableJob.State.Failed:
                        _currentJob.IncrementFailedTargetCount();
                        _currentJob.Updated(OnUpdatedJob, state);
                        break;
                }

                if (state == ObservableJob.State.Canceled)
                    return;

                if (!_currentJob.NoDelay)
                    Thread.Sleep(_delay);
            }
            finally
            {
                Interlocked.Increment(ref _multiCounter);

                if (AtomicOperation.Read(ref _multiCounter) >= currentMultiJob.TotalTargetCount)
                    currentMultiJob.Signal();
            }
           
        }

        private void ProcessJobEnd()
        {
            if (_currentJob.IsCanceled())
            {
                // 모든 작업이 완료된 후 취소 요청이 들어와서 취소 처리된 작업인 경우
                if (_currentJob.CompletedTargetCount + _currentJob.FailedTargetCount == _currentJob.TotalTargetCount)
                    _currentJob.Finished(OnFinishedJob);
                else
                    _currentJob.Canceled(OnCanceledJob);
            }
            else
            {
                _currentJob.Finished(OnFinishedJob);
            }
        }
    }
}
