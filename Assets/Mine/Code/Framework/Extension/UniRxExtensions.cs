using System;
using Mine.Code.Framework.Lifecycle;
using Mine.Code.Framework.UniRxCustom;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Mine.Code.Framework.Extension
{
    public static class UniRxExtensions
    {
        #region For UniRx

#if UNIRX_SUPPORT

        public static IObservable<Unit> OnDrawGizmosAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return component.GetOrAddComponent<ObservableOnDrawGizmosTrigger>().OnDrawGizmosAsObservable();
        }

        public static IObservable<Unit> OnGUIAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();

            return component.GetOrAddComponent<ObservableOnGUITrigger>().OnGUIAsObservable();
        }
        
        public static IObservable<Unit> OnMouseDownAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseDownTrigger>().OnMouseDownAsObservable();
        }
        
        public static IObservable<Unit> OnMouseDragAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseDragTrigger>().OnMouseDragAsObservable();
        }
        
        public static IObservable<Unit> OnMouseEnterAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseEnterTrigger>().OnMouseEnterAsObservable();
        }
        
        public static IObservable<Unit> OnMouseExitAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseExitTrigger>().OnMouseExitAsObservable();
        }
        
        public static IObservable<Unit> OnMouseOverAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseOverTrigger>().OnMouseOverAsObservable();
        }
        
        public static IObservable<Unit> OnMouseUpAsButtonAsObservable(this GameObject gameObject)
        {
            if (!gameObject) return Observable.Empty<Unit>();
            
            return gameObject.GetOrAddComponent<ObservableOnMouseUpAsButtonTrigger>().OnMouseUpAsButtonAsObservable();
        }
        
        public static IObservable<Unit> OnMouseUpAsObservable(this Component component)
        {
            if (!component || !component.gameObject) return Observable.Empty<Unit>();
            
            return component.GetOrAddComponent<ObservableOnMouseUpTrigger>().OnMouseUpAsObservable();
        }

        // 일정 시간 내 Double Click을 감지하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnDoubleClick(this Button button, float interval = 0.5f)
        {
            return button.OnClickAsObservable().Buffer(TimeSpan.FromSeconds(interval)).Where(xs => xs.Count >= 2).AsUnitObservable().Share();
        }

        // 버튼을 터치하고 있는 중인 것을 감지하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnTouchingAsObservable(this Button button)
        {
            return Observable.EveryUpdate().SkipUntil(button.OnPointerDownAsObservable()).TakeUntil(button.OnPointerUpAsObservable()).RepeatUntilDestroy(button).AsUnitObservable().Share();
        }

        // 현재 진행 중인 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration).AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, string animationName)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }

        // 지정된 애니메이션이 종료 시 이벤트 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAnimationCompleteAsObservable(this Animator animator, int animationHash)
        {
            return Observable.EveryUpdate().Where(_ => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash).Where(_ => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - animator.GetAnimatorTransitionInfo(0).duration)
                .AsUnitObservable().FirstOrDefault();
        }

        // 사운드 재생이 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnAudioCompleteAsObservable(this AudioSource audioSource)
        {
            return Observable.EveryUpdate().Where(_ => !audioSource.isPlaying).AsUnitObservable().FirstOrDefault();
        }

        // 파티클 재생 종료 시 이벤트를 호출하는 기능을 UniRx에 추가
        public static IObservable<Unit> OnParticleCompleteAsObservable(this ParticleSystem particleSystem)
        {
            return Observable.EveryUpdate().Where(_ => !particleSystem.IsAlive()).AsUnitObservable().FirstOrDefault();
        }

        public static IDisposable AddTo<T>(this T disposable) where T : IDisposable
            => disposable.AddTo(SceneLife.In.gameObject);
        
#endif

        #endregion
    }
}
