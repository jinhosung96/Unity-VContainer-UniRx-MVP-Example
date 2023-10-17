#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    public abstract class RotateShowAnimation
    {
        public abstract UniTask AnimateAsync(RectTransform rectTransform);
    }
    
    public abstract class RotateHideAnimation
    {
        public abstract UniTask AnimateAsync(RectTransform rectTransform);
    }
}
#endif