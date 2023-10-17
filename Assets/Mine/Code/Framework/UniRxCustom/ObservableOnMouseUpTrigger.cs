#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseUp에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseUpTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseUp;

        #endregion

        #region property

        public IObservable<Unit> OnMouseUpAsObservable()
        {
            return onMouseUp ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseUp()
        {
            if (onMouseUp != null)
            {
                onMouseUp.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseUp != null)
            {
                onMouseUp.OnCompleted();
            }
        }

        #endregion
    }
}
#endif