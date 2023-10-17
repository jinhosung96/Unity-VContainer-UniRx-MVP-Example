#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using Cysharp.Threading.Tasks;
using UnityEngine;

// ReSharper disable Unity.NoNullPropagation

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    [System.Serializable]
    public class ViewShowAnimation
    {
        #region Fields

        [SerializeReference] public MoveShowAnimation moveAnimation;
        [SerializeReference] public RotateShowAnimation rotateAnimation;
        [SerializeReference] public ScaleShowAnimation scaleAnimation;
        [SerializeReference] public FadeShowAnimation fadeAnimation;

        #endregion

        #region Public Methods
        
        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                scaleAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
            );
        }

        #endregion
    }
    
    [System.Serializable]
    public class ViewHideAnimation
    {
        #region Fields

        [SerializeReference] MoveHideAnimation moveAnimation;
        [SerializeReference] RotateHideAnimation rotateAnimation;
        [SerializeReference] ScaleHideAnimation scaleAnimation;
        [SerializeReference] FadeHideAnimation fadeAnimation;
        
        #endregion

        #region Public Methods
        
        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                scaleAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
            );
        }

        #endregion
    }
}
#endif