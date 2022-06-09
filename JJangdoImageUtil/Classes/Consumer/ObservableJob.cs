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
            CreateSingleJob,    // 외부 싱글 쓰레드(Consumer 쓰레드)에서 생성 작업 후 리스트에 추가하는 작업
            CreateMultiJob,     // 외부 쓰레드 풀에서 생성 작업 후 리스트에 추가하는 작업
            UpdateSingleJob,    // 외부 싱글 쓰레드(Consumer 쓰레드)에서 각 원소에 대해서 작업 처리
            UpdateMultiJob      // 외부 쓰레드 풀에서 각 원소에 대해서 작업 처리
        }

        public enum State
        {
            Initialized,        // 생성되고 큐에 넣어지기 전까지의 상태
            Waiting,            // 큐어 들어가서 꺼내기지 전까지 대기 상태
            Started,            // 작업이 시작되고 특정 원소에 대해서 작업을 진행하기 전까지의 상태
            Completed,          // 특정 원소에 대해서 작업을 성공한 경우
            Canceled,           // 작업이 취소된 경우
            Failed,             // 특정 원소에 대해서 작업을 실패한 경우
            Finished            // 모든 원소에 대해서 작업을 진행 완료한 경우
        }



        protected static int SequenceId = 0;
        protected static Dispatcher UiDispatcher;

        protected int _uniqueId;                // 이 작업의 고유번호
        protected int _id;                      // 사용자가 설정한 번호
        protected int _state;
        protected int _noDelay;
        protected Type _type;

        protected int _totalTargetCount;
        protected int _completedTargetCount;
        protected int _failedTargetCount;

        public event EventHandler OnStarted;
        public event EventHandler OnCompleted;
        public event EventHandler OnCanceled;
        public event EventHandler OnFailed;
        public event EventHandler OnFinished;
       


        public static void SetDispatcher(Dispatcher dispatcher)
        {
            UiDispatcher = dispatcher;
        }

        protected ObservableJob(int id, Type type)
        {
            _uniqueId = Interlocked.Increment(ref SequenceId);
            _id = id;
            _state = Convert.ToInt32(State.Initialized);
            _type = type;
        }

        public int UniqueId => _uniqueId;
        public int Id => _id;
        public int CompletedTargetCount => AtomicOperation.Read(ref _completedTargetCount);
        public int FailedTargetCount => AtomicOperation.Read(ref _failedTargetCount);
        public int TotalTargetCount => AtomicOperation.Read(ref _totalTargetCount);

        public int GetUniqueId()
        {
            return _uniqueId;
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

    
        public void Started(StartedHandler handler, int totalTargetCount = 0)
        {
            if (_type == Type.UpdateSingleJob || _type == Type.UpdateMultiJob)
                Interlocked.Exchange(ref _totalTargetCount, totalTargetCount);

            SetState(State.Started);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
            OnStarted?.Invoke(this, EventArgs.Empty);
        }


        public void Updated(UpdateHandler handler, State state)
        {
            UiDispatcher.Invoke(() => handler?.Invoke(this), DispatcherPriority.Normal);

            if (state == State.Failed)
                OnFailed?.Invoke(this, EventArgs.Empty);
            else if (state == State.Completed)
                OnCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void Finished(FinishedHandler handler)
        {
            SetState(State.Finished);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
            OnFinished?.Invoke(this, EventArgs.Empty);
        }

        public void Canceled(CancelHandler handler)
        {
            SetState(State.Canceled);
            UiDispatcher.Invoke(() => handler?.Invoke(this));
            OnCanceled?.Invoke(this, EventArgs.Empty);
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

        public bool IsFinished()
        {
            return AtomicOperation.Read(ref _state) == Convert.ToInt32(State.Finished);
        }


        public double ProgressRatio()
        {
            return (double)(FailedTargetCount + CompletedTargetCount) / TotalTargetCount;
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

            return _uniqueId == observableJob._uniqueId;
        }

        public override int GetHashCode()
        {
            return _uniqueId.GetHashCode();
        }
    }
}
