#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Modal
{
    public sealed class ModalContainer : UIContainer<ModalContainer>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField] Backdrop modalBackdrop; // 생성된 모달의 뒤에 배치될 레이어

        #endregion

        #region Properties

        [field: SerializeField] public List<Modal> RegisterModalsByPrefab { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Modal>> RegisterModalsByAddressable { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록 
#endif
        Dictionary<Type, Modal> Modals { get; set; }

        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Modal> History { get; } = new();

        public Modal CurrentView => History.TryPeek(out var currentView) ? currentView : null;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterModalsByPrefab = RegisterModalsByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Modal>()).ToList();
#endif

            // modals에 등록된 모든 Modal들을 Type을 키값으로 한 Dictionary 형태로 등록
            Modals = RegisterModalsByPrefab.GroupBy(x => x.GetType()).Select(x => x.FirstOrDefault()).ToDictionary(modal => modal.GetType(), modal => modal);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable) RegisterModalsByAddressable.ForEach(x => x.ReleaseAsset());
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 생성할 Modal의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null) where T : Modal
        {
            if (Modals.TryGetValue(typeof(T), out var modal))
                return await NextAsync(modal as T, onPreInitialize, onPostInitialize);

            Util.Debug.Debug.LogError($"Modal not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextModalName"> 생성할 Modal의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Modal> NextAsync(
            string nextModalName,
            Action<Modal> onPreInitialize = null,
            Action<Modal> onPostInitialize = null)
        {
            var modal = Modals.Values.FirstOrDefault(x => x.GetType().Name == nextModalName);
            if (modal != null) return await NextAsync(modal, onPreInitialize, onPostInitialize);

            Util.Debug.Debug.LogError($"Modal not found : {nextModalName}");
            return null;
        }

        public async UniTask<Modal> NextAsync(
            Type nextModalType,
            Action<Modal> onPreInitialize = null,
            Action<Modal> onPostInitialize = null)
        {
            if (Modals.TryGetValue(nextModalType, out var modal))
                return await NextAsync(modal, onPreInitialize, onPostInitialize);

            Util.Debug.Debug.LogError($"Modal not found : {nextModalType.Name}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1)
        {
            count = Mathf.Clamp(count, 1, History.Count);

            if (!CurrentView) return;
            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            await UniTask.WhenAll(Enumerable.Range(0, count).Select(_ => HideViewAsync()));
        }

        async UniTask HideViewAsync()
        {
            var currentView = History.Pop();
            if (currentView.BackDrop)
            {
                await UniTask.WhenAll
                (
                    currentView.BackDrop.DOFade(0, 0.2f).ToUniTask(),
                    currentView.HideAsync()
                );
            }
            else await currentView.HideAsync();

            if (currentView.BackDrop) Destroy(currentView.BackDrop.gameObject);
            Destroy(currentView.gameObject);
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextModal,
            Action<T> onPreInitialize,
            Action<T> onPostInitialize) where T : Modal
        {
            if (CurrentView != null && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;

            var backdrop = await ShowBackdrop();

            nextModal.gameObject.SetActive(false);
            nextModal =
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextModal, transform);
#else
                Instantiate(nextModal, transform);
#endif

            nextModal.UIContainer = this;

            nextModal.OnPreInitialize.FirstOrDefault().Subscribe(_ => onPreInitialize?.Invoke(nextModal)).AddTo(nextModal);
            nextModal.OnPostInitialize.FirstOrDefault().Subscribe(_ => onPostInitialize?.Invoke(nextModal)).AddTo(nextModal);

            if (backdrop)
            {
                nextModal.BackDrop = backdrop;
                if (!nextModal.BackDrop.TryGetComponent<Button>(out var button))
                    button = nextModal.BackDrop.gameObject.AddComponent<Button>();

                button.OnClickAsObservable().Subscribe(_ => PrevAsync().Forget());
            }

            History.Push(nextModal);

#pragma warning disable 4014
            if (nextModal.BackDrop) CurrentView.BackDrop.DOFade(1, 0.2f);
#pragma warning restore 4014

            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        async UniTask<CanvasGroup> ShowBackdrop()
        {
            if (!modalBackdrop) return null;

            var backdrop = Instantiate(modalBackdrop.gameObject, transform, true);
            if (!backdrop.TryGetComponent<CanvasGroup>(out var canvasGroup))
                canvasGroup = backdrop.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var rectTransform = (RectTransform)backdrop.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            return canvasGroup;
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterModalsByPrefab.Clear();
            else
                RegisterModalsByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif