#if UNIRX_SUPPORT

using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    /// <summary>
    /// OnGUI에 대한 UniRx Trigger 추가
    /// </summary>
    [DisallowMultipleComponent]
    public class ObservableOnGUITrigger : ObservableTriggerBase
    {
        #region variable

        private Subject<Unit> _onGui;

        #endregion

        #region property

        public IObservable<Unit> OnGUIAsObservable()
        {
            return _onGui ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        private void OnGUI()
        {
            if (_onGui != null)
            {
                _onGui.OnNext(default(Unit));
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (_onGui != null)
            {
                _onGui.OnCompleted();
            }
        }

        #endregion
    }
} 

#endif