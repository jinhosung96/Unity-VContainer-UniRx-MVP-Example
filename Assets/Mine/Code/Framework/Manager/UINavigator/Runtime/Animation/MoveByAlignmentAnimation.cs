#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    public enum Alignment
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }
    
    public class MoveByAlignmentShowAnimation : MoveShowAnimation
    {
        [SerializeField] Alignment from;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.Linear;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, from);
            await rectTransform.DOAnchorPos(PositionFromAlignment(rectTransform, Alignment.None), duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }

        Vector2 PositionFromAlignment(RectTransform rectTransform, Alignment alignment)
        {
            var rect = rectTransform.rect;
            return alignment switch
            {
                Alignment.Left => Vector2.left * rect.width,
                Alignment.Top => Vector2.up * rect.height,
                Alignment.Right => Vector2.right * rect.width,
                Alignment.Bottom => Vector2.down * rect.height,
                _ => Vector2.zero
            };
        }
    }
    
    public class MoveByAlignmentHideAnimation : MoveHideAnimation
    {
        [SerializeField] Alignment to;
        [SerializeField] float startDelay;
        [SerializeField] float duration = 0.25f;
        [SerializeField] Ease ease = Ease.Linear;

        public override async UniTask AnimateAsync(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = PositionFromAlignment(rectTransform, Alignment.None);
            await rectTransform.DOAnchorPos(PositionFromAlignment(rectTransform, to), duration).SetDelay(startDelay).SetEase(ease).SetUpdate(true).ToUniTask();
        }

        Vector2 PositionFromAlignment(RectTransform rectTransform, Alignment alignment)
        {
            var rect = rectTransform.rect;
            return alignment switch
            {
                Alignment.Left => Vector2.left * rect.width,
                Alignment.Top => Vector2.up * rect.height,
                Alignment.Right => Vector2.right * rect.width,
                Alignment.Bottom => Vector2.down * rect.height,
                _ => Vector2.zero
            };
        }
    }
}
#endif