#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Extension;
using UniRx;
using UnityEngine;
#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
using VContainer.Unity;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Page
{
    public sealed class PageContainer : UIContainer<PageContainer>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Properties
        
        [field: SerializeField] public List<Page> RegisterPagesByPrefab {get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Page>> RegisterPagesByAddressable {get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록 
#endif
        [field: SerializeField] public bool HasDefault { get; private set; } // 시작할 때 초기 시트 활성화 여부
        internal Page DefaultPage { get; private set; }

        Dictionary<Type, Page> Pages { get; set; }

        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Page> History { get; } = new();

        public Page CurrentView => History.TryPeek(out var currentView) ? currentView : null;
        bool IsRemainHistory => DefaultPage ? History.Count > 1 : History.Count > 0;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            
#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterPagesByPrefab = RegisterPagesByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Page>()).ToList();
#endif
            
            // pages에 등록된 모든 Page들을 Type을 키값으로 한 Dictionary 형태로 등록
            RegisterPagesByPrefab = RegisterPagesByPrefab.Select(x => x.IsRecycle ? Instantiate(x, transform) : x).GroupBy(x => x.GetType()).Select(x => x.FirstOrDefault()).ToList();
            Pages = RegisterPagesByPrefab.ToDictionary(page => page.GetType(), page => page);

            RegisterPagesByPrefab.Where(x => x.IsRecycle).ForEach(x =>
            {
                x.UIContainer = this;
                x.gameObject.SetActive(false);
            });

            if(HasDefault && RegisterPagesByPrefab.Any()) DefaultPage = Pages[RegisterPagesByPrefab.First().GetType()];
        }

        void OnEnable()
        {
            if (DefaultPage && Pages.TryGetValue(DefaultPage.GetType(), out var nextPage))
            {
                if (CurrentView)
                {
                    CurrentView.HideAsync(false).Forget();
                    if (!CurrentView.IsRecycle) Destroy(CurrentView.gameObject);
                }

                nextPage = nextPage.IsRecycle ? nextPage : Instantiate(nextPage, transform);
                nextPage.ShowAsync(false).Forget();
                History.Push(nextPage);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterPagesByAddressable.ForEach(x => x.ReleaseAsset());
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 활성화 시킬 Page의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null) where T : Page
        {
            if (Pages.TryGetValue(typeof(T), out var page))
                return await NextAsync(page as T, onPreInitialize, onPostInitialize);

            Debug.LogError($"Page not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextPageName"> 활성화 시킬 Page의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Page> NextAsync(string nextPageName,
            Action<Page> onPreInitialize = null,
            Action<Page> onPostInitialize = null)
        {
            var page = Pages.Values.FirstOrDefault(x => x.GetType().Name == nextPageName);
            if (page != null) return await NextAsync(page, onPreInitialize, onPostInitialize);

            Debug.LogError($"Page not found : {nextPageName}");
            return null;
        }

        /// <summary>
        /// 특정 UI View를 종료하는 메소드이다. <br/>
        /// 해당 UI View가 종료되면 해당 View보다 Histroy상 뒤에 활성화된 View들도 모두 같이 종료된다. <br/>
        /// <br/>
        /// 해당 UI View의 History를 가지고 있는 부모 Sheet를 찾아 해당 History를 최상단부터 차근차근 비교하며 해당 View를 찾는다. <br/>
        /// 그리고 그 View들을 나중에 제거하기 위해 Queue에 담아둔다. <br/>
        /// 해당 View를 찾으면 해당 View를 제거하는데 이 때, 만약 해당 UI View가 Sheet라면, <br/>
        /// ResetOnPop 설정 여부에 따라 해당 Sheet의 부모 Container의 CurrentSheet를 null로 초기화하거나 InitialSheet를 CurrentSheet로 설정한다. <br/>
        /// <br/>
        /// 지정한 UI View가 종료되면 Queue에 담아둔 View들을 모두 종료하고 Modal은 추가로 Backdrop을 제거함과 동시에 Modal 또한 파괴한다. <br/>
        /// 이 때, PopRoutineAsync 메소드를 사용하지 않는 이유는 즉각적으로 제거해주기 위함과 더불어 Sheet의 경우 PopRoutineAsync가 정의되어있지 않기 때문이다. <br/>
        /// </summary>
        public async UniTask<Page> NextAsync(Type nextPageType,
            Action<Page> onPreInitialize = null,
            Action<Page> onPostInitialize = null)
        {
            if (Pages.TryGetValue(nextPageType, out var page))
                return await NextAsync(page, onPreInitialize, onPostInitialize);

            Debug.LogError($"Page not found : {nextPageType.Name}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1)
        {
            count = Mathf.Clamp(count, 1, History.Count);
            if (!IsRemainHistory) return;

            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            CurrentView.HideAsync().Forget();

            for (int i = 0; i < count; i++)
            {
                if (!CurrentView.IsRecycle) Destroy(CurrentView.gameObject);
                History.Pop();
            }

            if (!CurrentView) return;

            await CurrentView.ShowAsync();
        }

        public async UniTask ResetAsync()
        {
            await PrevAsync(History.Count);
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextPage, Action<T> onPreInitialize, Action<T> onPostInitialize) where T : Page
        {
            // 현재 Page가 전환 중이거나, 현재 Page와 다음 Page가 같다면 실행하지 않는다.
            if (CurrentView && (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing || CurrentView == nextPage)) return null;

            // 생성된 객체가 Awake를 호출하기 전에 사전 작업을 미리 해두기 위해 프리팹을 비활성화하고 Instantiate 한다.
            // Awake는 비활성화 된채로 생성된 객체가 처음으로 활성화 될 때 호출된다.
            // 또한 VContainer 라이브러리 사용 여부에 따라 생성 방식을 달리한다.
            nextPage.gameObject.SetActive(false);
            nextPage = nextPage.IsRecycle
                ? nextPage
                :
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextPage, transform);
#else
                Instantiate(nextPage, transform);
#endif
            nextPage.UIContainer = this;

            // Awake 호출 전후로 처리할 이벤트를 등록한다.
            nextPage.OnPreInitialize.FirstOrDefault().Subscribe(_ => onPreInitialize?.Invoke(nextPage)).AddTo(nextPage);
            nextPage.OnPostInitialize.FirstOrDefault().Subscribe(_ => onPostInitialize?.Invoke(nextPage)).AddTo(nextPage);

            // 현재 Page가 있다면, 현재 Page를 비활성화하고 새로운 Page를 활성화한다.
            // 이 때, 새롭게 활성화 되는 Page를 History에 저장한다.
            if (CurrentView) CurrentView.HideAsync().Forget();
            History.Push(nextPage);
            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterPagesByPrefab.Clear();
            else
                RegisterPagesByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif