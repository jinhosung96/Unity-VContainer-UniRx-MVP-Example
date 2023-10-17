#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    public class RotateByVector3ShowAnimation : RotateShowAnimation
    {
        [SerializeField] Vector3 from;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.OutQuart;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.localRotation = Quaternion.Euler(from);
            await rectTransform.DOLocalRotate(Vector3.zero, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }
    
    public class RotateByVector3HideAnimation : RotateHideAnimation
    {
        [SerializeField] Vector3 to;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.InQuart;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
            await rectTransform.DOLocalRotate(to, duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }
    }
}
#endif