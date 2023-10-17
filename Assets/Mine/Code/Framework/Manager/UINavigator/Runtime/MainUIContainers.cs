#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using System.Linq;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Page;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime
{
    public sealed class MainUIContainers : MonoBehaviour
    {
        #region Field

        static MainUIContainers instance;
        [SerializeField] SheetContainer mainSheetContainer;
        [SerializeField] PageContainer mainPageContainer;
        [SerializeField] ModalContainer mainModalContainer;

        #endregion

        #region Property

        public static MainUIContainers In => instance = instance ? instance : FindObjectOfType<MainUIContainers>() ?? new GameObject(nameof(MainUIContainers)).AddComponent<MainUIContainers>();

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            if (instance) Destroy(gameObject);

            if (!mainSheetContainer && !mainPageContainer && !mainModalContainer)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                    {
                        if(!mainSheetContainer) mainSheetContainer = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<SheetContainer>()).FirstOrDefault(x => x);
                        if(!mainPageContainer) mainPageContainer = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<PageContainer>()).FirstOrDefault(x => x);
                        if(!mainModalContainer) mainModalContainer = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<ModalContainer>()).FirstOrDefault(x => x);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public T GetMain<T>() where T : UIContainer<T>
        {
            if (typeof(T) == typeof(SheetContainer)) return mainSheetContainer as T;
            if (typeof(T) == typeof(PageContainer)) return mainPageContainer as T;
            if (typeof(T) == typeof(ModalContainer)) return mainModalContainer as T;
            return null;
        }
    
        public void SetMain<T>(T container) where T : UIContainer<T>
        {
            if (typeof(T) == typeof(SheetContainer)) mainSheetContainer = container as SheetContainer;
            if (typeof(T) == typeof(PageContainer)) mainPageContainer = container as PageContainer;
            if (typeof(T) == typeof(ModalContainer)) mainModalContainer = container as ModalContainer;
        }

        #endregion
    }
}
#endif