#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime
{
    public enum InstantiateType
    {
        InstantiateByPrefab,
#if ADDRESSABLE_SUPPORT
        InstantiateByAddressable
#endif
    }
    
    [DefaultExecutionOrder(-1)]
    public abstract class UIContainer : MonoBehaviour
    {
        static readonly Dictionary<int, UIContainer> Cached = new();
        [SerializeField] bool isDontDestroyOnLoad; 

        #region Properties

        [field: SerializeField] public string ContainerName { get; private set; }
        [field: SerializeField] public InstantiateType InstantiateType { get; private set; } = InstantiateType.InstantiateByPrefab;
        [field: SerializeField] public ViewShowAnimation ShowAnimation { get; private set; } = new();

        [field: SerializeField] public ViewHideAnimation HideAnimation { get; private set; } = new();

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (isDontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// 인자 값으로 받은 Transform의 가장 인접한 Container를 반환한다.
        /// 기본적으로 캐싱하여 사용되며 캐싱된 값이 없을 경우 새로 생성하여 반환한다.
        /// 설정에 따라 캐싱 여부를 결정할 수 있다.
        /// </summary>
        /// <param name="transform"> Container를 찾을 기준 Transform </param>
        /// <param name="useCache"> 캐싱 사용 여부 </param>
        /// <returns></returns>
        public static UIContainer Of(Transform transform, bool useCache = true) => Of((RectTransform)transform, useCache);

        /// <summary>
        /// 인자 값으로 받은 RectTransform 가장 인접한 Container를 반환한다.
        /// 기본적으로 캐싱하여 사용되며 캐싱된 값이 없을 경우 새로 생성하여 반환한다.
        /// 설정에 따라 캐싱 여부를 결정할 수 있다.
        /// </summary>
        /// <param name="rectTransform"> Container를 찾을 기준 RectTransform </param>
        /// <param name="useCache"> 캐싱 사용 여부 </param>
        /// <returns></returns>
        public static UIContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if (useCache && Cached.TryGetValue(hashCode, out var container))
            {
                return container;
            }

            container = rectTransform.GetComponentInParent<UIContainer>();
            if (container != null)
            {
                Cached.Add(hashCode, container);
                return container;
            }

            return null;
        }

        public static async UniTask<bool> BackAsync()
        {
            if (UIContext.FocusContext)
            {
                await ((IHasHistory)UIContext.FocusContext.UIContainer).PrevAsync();
                return true;
            }

            return false;
        }

        #endregion
    }

    public abstract class UIContainer<T> : UIContainer where T : UIContainer<T>
    {
        #region Fields

        static readonly Dictionary<string, T> containers = new();
        
        #endregion

        #region Properties

        public static T Main
        {
            get
            {
                T main = global::Mine.Code.Framework.Manager.UINavigator.Runtime.MainUIContainers.In.GetMain<T>();
                if (main) return main;
                
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                    {
                        main = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<T>()).FirstOrDefault(x => x);
                        global::Mine.Code.Framework.Manager.UINavigator.Runtime.MainUIContainers.In.SetMain(main);
                        if (main) return main;
                    }
                }

                return null;
            }
        }

        static readonly Dictionary<int, T> Cached = new();

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(ContainerName))
            {
                containers[ContainerName] = (T)this;
            }
        }

        protected virtual void OnDestroy()
        {
            containers.Remove(ContainerName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 인자 값으로 받은 Transform의 가장 인접한 Container를 반환한다.
        /// 기본적으로 캐싱하여 사용되며 캐싱된 값이 없을 경우 새로 생성하여 반환한다.
        /// 설정에 따라 캐싱 여부를 결정할 수 있다.
        /// </summary>
        /// <param name="transform"> Container를 찾을 기준 Transform </param>
        /// <param name="useCache"> 캐싱 사용 여부 </param>
        /// <returns></returns>
        public new static T Of(Transform transform, bool useCache = true) => Of((RectTransform)transform, useCache);

        /// <summary>
        /// 인자 값으로 받은 RectTransform 가장 인접한 Container를 반환한다.
        /// 기본적으로 캐싱하여 사용되며 캐싱된 값이 없을 경우 새로 생성하여 반환한다.
        /// 설정에 따라 캐싱 여부를 결정할 수 있다.
        /// </summary>
        /// <param name="rectTransform"> Container를 찾을 기준 RectTransform </param>
        /// <param name="useCache"> 캐싱 사용 여부 </param>
        /// <returns></returns>
        public new static T Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if (useCache && Cached.TryGetValue(hashCode, out var container))
            {
                return container;
            }

            container = rectTransform.GetComponentInParent<T>();
            if (container != null)
            {
                Cached.Add(hashCode, container);
                return container;
            }

            return null;
        }

        /// <summary>
        /// 이름을 키로 캐싱해둔 Container 목록에서 해당 이름의 Container를 찾아 반환한다.
        /// 이름은 인스펙터 상에서 정해준다.
        /// </summary>
        /// <param name="containerName"> 찾고 있는 Container의 이름 </param>
        /// <returns></returns>
        public static T Find(string containerName)
        {
            if (containers.TryGetValue(containerName, out var container)) return container;
            Util.Debug.Debug.LogError($"Container with name {containerName} not found");
            return null;
        }

        #endregion
    }
}

#endif