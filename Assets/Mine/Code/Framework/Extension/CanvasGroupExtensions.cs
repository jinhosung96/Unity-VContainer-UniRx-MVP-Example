#if DOTWEEN_SUPPORT
using DG.Tweening;
using UnityEngine;

namespace Mine.Code.Framework.Extension
{
    public static class CanvasGroupExtensions
    {
        public static Tweener FadeOut(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(0.0F, duration);
        }

        public static Tweener FadeIn(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(1.0F, duration);
        }
    }
}
#endif