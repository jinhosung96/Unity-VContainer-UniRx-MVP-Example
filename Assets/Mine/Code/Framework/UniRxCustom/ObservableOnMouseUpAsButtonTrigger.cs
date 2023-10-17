#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseUpAsButton에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseUpAsButtonTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseUpAsButton;

        #endregion

        #region property

        public IObservable<Unit> OnMouseUpAsButtonAsObservable()
        {
            return onMouseUpAsButton ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseUpAsButton()
        {
            if (onMouseUpAsButton != null)
            {
                onMouseUpAsButton.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseUpAsButton != null)
            {
                onMouseUpAsButton.OnCompleted();
            }
        }

        #endregion
    }
}
#endif