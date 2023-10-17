#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseExit에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseExitTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseExit;

        #endregion

        #region property

        public IObservable<Unit> OnMouseExitAsObservable()
        {
            return onMouseExit ??= new Subject<Unit>();
        }

        #endregion

        #region unity event
    
        void OnMouseExit()
        {
            if (onMouseExit != null)
            {
                onMouseExit.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseExit != null)
            {
                onMouseExit.OnCompleted();
            }
        }

        #endregion
    }
}
#endif