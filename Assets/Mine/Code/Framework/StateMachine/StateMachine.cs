#if UNIRX_SUPPORT
using System;
using UniRx;

namespace Mine.Code.Framework.StateMachine
{
    [Serializable]
    public class StateMachine<T> where T : Enum
    {
        #region Fields
    
        T currentState;
    
        Subject<T> beginSubject = new();
        Subject<T> endSubject = new();

        #endregion
    
        #region Properties

        public T CurrentState => currentState;
        public bool IsPlay { get; set; }

        public IObservable<T> OnBegin => beginSubject.Share();
        public IObservable<T> OnEnd => endSubject.Share();
        public IObservable<T> OnUpdateStream => Observable.EveryUpdate().Select(_ => currentState).Share();
        public IObservable<T> OnFixedUpdateStream => Observable.EveryFixedUpdate().Select(_ => currentState).Share();
        public IObservable<T> OnLateUpdateStream => Observable.EveryLateUpdate().Select(_ => currentState).Share();

        #endregion

        #region Public Methords

        /// <summary>
        /// 스테이트 머신 개시
        /// </summary>
        public void StartFsm(T initState)
        {
            IsPlay = true;
            currentState = initState;
            beginSubject.OnNext(currentState);
        }

        /// <summary>
        /// 스테이트 머신 종료
        /// </summary>
        public void FinishFsm()
        {
            endSubject.OnNext(currentState);
            IsPlay = false;
        }

        public void Transition(T state)
        { 
            //현재 스테이트 종료
            endSubject.OnNext(currentState);

            //다음 스테이트 개시
            currentState = state;
            beginSubject.OnNext(currentState);
        }
    
        public IObservable<T> OnBeginState(T state) => OnBegin.Where(x => x.Equals(state));
        public IObservable<T> OnEndState(T state) => OnEnd.Where(x => x.Equals(state));
        public IObservable<T> OnUpdateState(T state) => OnUpdateStream.Where(x => x.Equals(state));
        public IObservable<T> OnFixedUpdateState(T state) => OnFixedUpdateStream.Where(x => x.Equals(state));
        public IObservable<T> OnLateUpdateState(T state) => OnLateUpdateStream.Where(x => x.Equals(state));

        #endregion
    }
}

#endif