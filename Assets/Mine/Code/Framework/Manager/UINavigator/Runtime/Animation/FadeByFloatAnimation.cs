#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation {
    public class FadeByFloatShowAnimation : FadeShowAnimation
    {
        [SerializeField] float from;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.Linear;

        public override async UniTask AnimateAsync(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = from;
            await canvasGroup.DOFade(1, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }

    public class FadeByFloatHideAnimation : FadeHideAnimation
    {
        [SerializeField] float to;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.Linear;

        public override async UniTask AnimateAsync(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            await canvasGroup.DOFade(to, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }
#endif
}