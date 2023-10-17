#if UNIRX_SUPPORT
using System;
using UniRx;
using UnityEngine;

namespace Mine.Code.Framework.UniRxCustom
{
    [Serializable]
    public class IntReactivePropertyWithRange : IntReactiveProperty
    {
        public int Min { get; }
        public int Max { get; }
    
        public IntReactivePropertyWithRange(int min, int max) : base()
        {
            Min = min;
            Max = max;
        }
    
        public IntReactivePropertyWithRange(int initialValue, int min, int max)
        {
            Min = min;
            Max = max;
            SetValue(initialValue);
        }

        protected override void SetValue(int value)
        {
            base.SetValue(Mathf.Clamp(value, Min, Max));
        }
    }
}

#endif