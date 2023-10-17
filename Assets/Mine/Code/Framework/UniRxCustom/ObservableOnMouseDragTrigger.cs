#if UNIRX_SUPPORT
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnMouseDrag에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnMouseDragTrigger : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onMouseDrag;

        #endregion

        #region property

        public IObservable<Unit> OnMouseDragAsObservable()
        {
            return onMouseDrag ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnMouseDrag()
        {
            if (onMouseDrag != null)
            {
                onMouseDrag.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onMouseDrag != null)
            {
                onMouseDrag.OnCompleted();
            }
        }

        #endregion
    }
}
#endif