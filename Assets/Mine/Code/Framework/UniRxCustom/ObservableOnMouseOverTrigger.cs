#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseOver에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseOverTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseOver;

        #endregion

        #region property

        public IObservable<Unit> OnMouseOverAsObservable()
        {
            return onMouseOver ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseOver()
        {
            if (onMouseOver != null)
            {
                onMouseOver.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseOver != null)
            {
                onMouseOver.OnCompleted();
            }
        }

        #endregion
    }
}
#endif