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
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet
{
    public sealed class SheetContainer : UIContainer<SheetContainer>, ISerializationCallbackReceiver
    {
        #region Properties

        [field: SerializeField] public List<Sheet> RegisterSheetsByPrefab { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Sheet들에 대한 목록
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Sheet>> RegisterSheetsByAddressable { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Sheet들에 대한 목록 
#endif
        [field: SerializeField] public bool HasDefault { get; private set; } // 시작할 때 초기 시트 활성화 여부
        Dictionary<Type, Sheet> Sheets { get; set; }

        /// <summary>
        /// 현재 Container의 현재 Sheet
        /// </summary>
        public Sheet CurrentView { get; private set; }

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            
#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterSheetsByPrefab = RegisterSheetsByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Sheet>()).ToList();
#endif

            // sheets에 등록된 모든 Sheet들을 Type을 키값으로 한 Dictionary 형태로 등록
            RegisterSheetsByPrefab = RegisterSheetsByPrefab.Select(x => x.IsRecycle ? Instantiate(x, transform) : x).GroupBy(x => x.GetType()).Select(x => x.FirstOrDefault()).ToList();
            Sheets = RegisterSheetsByPrefab.ToDictionary(sheet => sheet.GetType(), sheet => sheet);

            RegisterSheetsByPrefab.Where(x => x.IsRecycle).ForEach(x =>
            {
                x.UIContainer = this;
                x.gameObject.SetActive(false);
            });
        }

        void OnEnable()
        {
            // 초기 시트를 활성화
            if (HasDefault && RegisterSheetsByPrefab.Any())
            {
                var nextSheet = Sheets[RegisterSheetsByPrefab.First().GetType()];

                if (CurrentView)
                {
                    CurrentView.HideAsync(false).Forget();
                    if (!CurrentView.IsRecycle) Destroy(CurrentView.gameObject);
                }

                CurrentView = nextSheet.IsRecycle ? nextSheet : Instantiate(nextSheet, transform);
                CurrentView.ShowAsync(false).Forget();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable) RegisterSheetsByAddressable.ForEach(x => x.ReleaseAsset());
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Sheet를 활성화한다. <br/>
        /// Sheet를 지정해주는 방법은 제네릭 타입으로 원하는 Sheet의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Sheet가 있다면, 이전 Sheet를 비활성화하고 새로운 Sheet를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Sheet가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// <br/>
        /// resetOnChangeSheet가 true일 경우, 이전 Sheet의 History를 초기화한다. <br/>
        /// </summary>
        /// <typeparam name="T"> 활성화 시킬 Sheet의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null) where T : Sheet
        {
            if (Sheets.TryGetValue(typeof(T), out var sheet))
                return await NextAsync(sheet as T, onPreInitialize, onPostInitialize);

            Debug.LogError($"Sheet not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Sheet를 활성화한다. <br/>
        /// <br/>
        /// 현재 활성화된 Sheet가 있다면, 이전 Sheet를 비활성화하고 새로운 Sheet를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Sheet가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// <br/>
        /// resetOnChangeSheet가 true일 경우, 이전 Sheet의 History를 초기화한다. <br/>
        /// </summary>
        /// <param name="sheetName"> 활성화 시킬 Sheet의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Sheet> NextAsync(string sheetName,
            Action<Sheet> onPreInitialize = null,
            Action<Sheet> onPostInitialize = null)
        {
            var sheet = Sheets.Values.FirstOrDefault(x => x.GetType().Name == sheetName);
            if (sheet != null) return await NextAsync(sheet, onPreInitialize, onPostInitialize);

            Debug.LogError($"Sheet not found : {sheetName}");
            return null;
        }

        public async UniTask<Sheet> NextAsync(Type targetSheet,
            Action<Sheet> onPreInitialize = null,
            Action<Sheet> onPostInitialize = null)
        {
            if (Sheets.TryGetValue(targetSheet, out var nextSheet))
                return await NextAsync(nextSheet, onPreInitialize, onPostInitialize);

            Debug.LogError("Sheet not found");
            return null;
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextSheet,
            Action<T> onPreInitialize,
            Action<T> onPostInitialize) where T : Sheet
        {
            if (CurrentView != null && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;
            if (CurrentView != null && CurrentView == nextSheet) return null;

            var prevSheet = CurrentView;

            nextSheet.gameObject.SetActive(false);
            nextSheet = nextSheet.IsRecycle
                ? nextSheet
                :
#if VCONTAINER_SUPPORT
                VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextSheet, transform);
#else
                Instantiate(nextSheet, transform);
#endif

            nextSheet.UIContainer = this;

            nextSheet.OnPreInitialize.FirstOrDefault().Subscribe(_ => onPreInitialize?.Invoke(nextSheet)).AddTo(nextSheet);
            nextSheet.OnPostInitialize.FirstOrDefault().Subscribe(_ => onPostInitialize?.Invoke(nextSheet)).AddTo(nextSheet);

            CurrentView = nextSheet;

            if (prevSheet != null)
            {
                prevSheet.HideAsync().Forget();
                if (!prevSheet.IsRecycle) Destroy(prevSheet.gameObject);
            }

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
                RegisterSheetsByPrefab.Clear();
            else
                RegisterSheetsByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif