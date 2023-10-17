#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseEnter에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseEnterTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseEnter;

        #endregion

        #region property

        public IObservable<Unit> OnMouseEnterAsObservable()
        {
            return onMouseEnter ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseEnter()
        {
            if (onMouseEnter != null)
            {
                onMouseEnter.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseEnter != null)
            {
                onMouseEnter.OnCompleted();
            }
        }

        #endregion
    }
}
#endif