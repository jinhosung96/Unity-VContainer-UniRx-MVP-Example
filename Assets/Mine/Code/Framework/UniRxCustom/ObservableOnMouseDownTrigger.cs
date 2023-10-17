#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseDown에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseDownTrigger : ObservableTriggerBase
    {
        #region variable

        private Subject<Unit> onMouseDown;

        #endregion

        #region property

        public IObservable<Unit> OnMouseDownAsObservable()
        {
            return onMouseDown ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseDown()
        {
            if (onMouseDown != null)
            {
                onMouseDown.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseDown != null)
            {
                onMouseDown.OnCompleted();
            }
        }

        #endregion
    }
}

#endif