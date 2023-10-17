#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Animation;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Page;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

// ReSharper disable HeapView.ObjectAllocation

namespace Mine.Code.Framework.Manager.UINavigator.Runtime
{
    public enum AnimationSetting
    {
        Container,
        Custom
    }

    // UI View의 애니메이션 상태
    public enum VisibleState
    {
        Appearing, // 등장 중
        Appeared, // 등장 완료
        Disappearing, // 사라지는 중
        Disappeared // 사라짐
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
#if VCONTAINER_SUPPORT
    public abstract class UIContext : LifetimeScope
#else
    public abstract class UIContext : MonoBehaviour
#endif
    {
        #region Fields

        [SerializeField] AnimationSetting animationSetting = AnimationSetting.Container;
        [SerializeField] ViewShowAnimation showAnimation = new();
        [SerializeField] ViewHideAnimation hideAnimation = new();

        CanvasGroup canvasGroup;

        readonly Subject<Unit> preInitializeEvent = new();
        readonly Subject<Unit> postInitializeEvent = new();
        readonly Subject<Unit> appearEvent = new();
        readonly Subject<Unit> appearedEvent = new();
        readonly Subject<Unit> disappearEvent = new();
        readonly Subject<Unit> disappearedEvent = new();

        #endregion

        #region Properties

        public static List<UIContext> ActiveViews { get; } = new();
        public float LastShowTime { get; private set; }

        public static UIContext FocusContext
        {
            get
            {
                var activeViews = ActiveViews
                    .Where(view => view.gameObject.activeInHierarchy)
                    .Where(view => view is not Sheet.Sheet)
                    .Where(view =>
                    {
                        if (view is not Page.Page page) return true;
                        if (page.UIContainer is PageContainer pageContainer) return pageContainer.DefaultPage != page;
                        return true;
                    });

                if (activeViews.Any()) return activeViews.Aggregate((prev, current) => prev.LastShowTime > current.LastShowTime ? prev : current);

                return null;
            }
        }

        public UIContainer UIContainer { get; set; }
        public CanvasGroup CanvasGroup => canvasGroup ? canvasGroup : canvasGroup = GetComponent<CanvasGroup>();
        public VisibleState VisibleState { get; private set; } = VisibleState.Disappeared;

        /// <summary>
        /// Awake보다 먼저 호출되는 이벤트
        /// </summary>
        public IObservable<Unit> OnPreInitialize => preInitializeEvent.Share();

        /// <summary>
        /// Awake보다 직후 호출되는 이벤트
        /// </summary>
        public IObservable<Unit> OnPostInitialize => postInitializeEvent.Share();

        /// <summary>
        /// UI View가 활성화를 시작할 때 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnAppear => appearEvent.Share();

        /// <summary>
        /// UI View가 활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);

        /// <summary>
        /// UI View가 활성화가 완전히 끝났을 때 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnAppeared => appearedEvent.Share();

        /// <summary>
        /// UI View가 활성화 되어 있는동안 매 프레임 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);

        /// <summary>
        /// UI View가 비활성화를 시작할 때 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnDisappear => disappearEvent.Share();

        /// <summary>
        /// UI View가 비활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);

        /// <summary>
        /// UI View가 비활성화가 완전히 끝났을 때 발생하는 이벤트
        /// </summary>
        public IObservable<Unit> OnDisappeared => disappearedEvent.Share();

        #endregion

        #region Unity Lifecycle

#if VCONTAINER_SUPPORT
        protected override void OnDestroy()
        {
            base.OnDestroy();
#else
        protected virtual void OnDestroy()
        {
#endif
            preInitializeEvent.Dispose();
            postInitializeEvent.Dispose();
            appearEvent.Dispose();
            appearedEvent.Dispose();
            disappearEvent.Dispose();
            disappearedEvent.Dispose();

            ActiveViews.Remove(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// UI View를 활성화할 때 실행되는 로직이다. <br/>
        /// UI View를 활성화하기 전에 앵커와 피봇을 Stretch Stretch로 설정하고 알파를 1로 초기화한다. <br/>
        /// 이 과정에서 Unity UI 시스템의 한계 때문에 1 프레임이 소비된다. <br/>
        /// 그리고 UI View가 활성화 되기 전 후에 이벤트를 발생시키며 현재 VisibleState를 갱신한다. <br/>
        /// <br/>
        /// 나머지는 템플릿 메소드 패턴으로 구성되어 있으며 하위 객체에서 구체적인 로직을 정의하게 되어있다. <br/>
        /// <br/>
        /// 전체 로직의 흐름은 다음과 같다. <br/>
        /// RectTransform 및 CanvasGroup 초기화 후 게임 오브젝트 활성화 -> Appearing State로 변경 -> AppearEvent 송신 -> 전 처리 로직 대기 -> Show 애니메이션 진행 -> Appeared State로 변경 -> AppearedEvent 송신 <br/>
        /// <br/>
        /// 주의 사항 - 클라이언트는 사용할 필요가 없는 API, 추 후 외부에 노출시키지 않을 방법을 찾아볼 것
        /// </summary>
        /// <param name="useAnimation"> 애니메이션 사용 여부, 인스펙터 상에서 경정해주는 isUseAnimation이랑 둘 다 true일 경우에만 애니메이션을 실행한다. </param>
        internal async UniTask ShowAsync(bool useAnimation = true)
        {
            LastShowTime = Time.time;
            ActiveViews.Add(this);

            var rectTransform = (RectTransform)transform;
            await InitializeRectTransformAsync(rectTransform);
            CanvasGroup.alpha = 1;

            await WhenPreAppearAsync();
            preInitializeEvent.OnNext(Unit.Default);
            gameObject.SetActive(true);
            postInitializeEvent.OnNext(Unit.Default);

            VisibleState = VisibleState.Appearing;
            appearEvent.OnNext(Unit.Default);

            if (useAnimation)
            {
                if (animationSetting == AnimationSetting.Custom) await showAnimation.AnimateAsync(rectTransform, CanvasGroup);
                else await UIContainer.ShowAnimation.AnimateAsync(transform, CanvasGroup);
            }

            await WhenPostAppearAsync();

            VisibleState = VisibleState.Appeared;
            appearedEvent.OnNext(Unit.Default);
        }

        /// <summary>
        /// UI View를 비활성화할 때 실행되는 로직이다. <br/>
        /// 그리고 UI View가 비활성화 되기 전 후에 이벤트를 발생시키며 현재 VisibleState를 갱신한다. <br/>
        /// <br/>
        /// 나머지는 템플릿 메소드 패턴으로 구성되어 있으며 하위 객체에서 구체적인 로직을 정의하게 되어있다. <br/>
        /// <br/>
        /// 전체 로직의 흐름은 다음과 같다. <br/>
        /// Disappearing State로 변경 -> DisappearEvent 송신 -> Hide 애니메이션 진행 -> 후 처리 로직 대기 -> Disappeared State로 변경 -> DisappearedEvent 송신 <br/>
        /// <br/>
        /// 주의 사항 - 클라이언트는 사용할 필요가 없는 API, 추 후 외부에 노출시키지 않을 방법을 찾아볼 것
        /// </summary>
        /// <param name="useAnimation"> 애니메이션 사용 여부, 인스펙터 상에서 경정해주는 isUseAnimation이랑 둘 다 true일 경우에만 애니메이션을 실행한다. </param>
        internal async UniTask HideAsync(bool useAnimation = true)
        {
            ActiveViews.Remove(this);

            VisibleState = VisibleState.Disappearing;
            disappearEvent.OnNext(Unit.Default);

            await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());

            await WhenPreDisappearAsync();

            if (useAnimation)
            {
                if (animationSetting == AnimationSetting.Custom) await hideAnimation.AnimateAsync(transform, CanvasGroup);
                else await UIContainer.HideAnimation.AnimateAsync(transform, CanvasGroup);
            }

            gameObject.SetActive(false);
            await WhenPostDisappearAsync();

            VisibleState = VisibleState.Disappeared;
            disappearedEvent.OnNext(Unit.Default);
        }

        #endregion

        #region Virtual Methods

        protected virtual UniTask WhenPreAppearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostAppearAsync() => UniTask.CompletedTask;

        protected virtual UniTask WhenPreDisappearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostDisappearAsync() => UniTask.CompletedTask;

        #endregion

        #region Private Methods

        IObservable<Unit> OnChangingVisibleState(IObservable<Unit> begin, IObservable<Unit> end) => this.UpdateAsObservable().SkipUntil(begin).TakeUntil(end).RepeatUntilDestroy(gameObject).Share();

        async UniTask InitializeRectTransformAsync(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }

        #endregion
    }
}
#endif