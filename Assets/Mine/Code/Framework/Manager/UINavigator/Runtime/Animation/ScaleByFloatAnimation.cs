#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    public class ScaleByFloatShowAnimation : ScaleShowAnimation
    {
        [SerializeField] float from;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.OutQuart;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one * from;
            await rectTransform.DOScale(1, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }
    
    public class ScaleByFloatHideAnimation : ScaleHideAnimation
    {
        [SerializeField] float to;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.InQuart;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one;
            await rectTransform.DOScale(to, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }
}
#endif