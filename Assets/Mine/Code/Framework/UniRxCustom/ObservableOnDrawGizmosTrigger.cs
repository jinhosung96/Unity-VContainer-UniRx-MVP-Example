#if UNIRX_SUPPORT

using System;
using UniRx;
using UniRx.Triggers;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnDrawGizmos�� ���� UniRx Trigger �߰�
    /// </summary>
    public class ObservableOnDrawGizmosTrigger : ObservableTriggerBase
    {
        #region variable

        private Subject<Unit> _onDrawGizmos;

        #endregion

        #region property

        public IObservable<Unit> OnDrawGizmosAsObservable()
        {
            return _onDrawGizmos ?? (_onDrawGizmos = new Subject<Unit>());
        }

        #endregion

        #region method

        private void OnDrawGizmos()
        {
            if (_onDrawGizmos != null)
            {
                _onDrawGizmos.OnNext(default(Unit));
            }
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (_onDrawGizmos != null)
            {
                _onDrawGizmos.OnCompleted();
            }
        }

        #endregion
    }
} 

#endif