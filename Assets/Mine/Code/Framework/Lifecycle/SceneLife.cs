using UnityEngine;

namespace Mine.Code.Framework.Lifecycle
{
    public sealed class SceneLife : MonoBehaviour
    {
        #region Field

        static SceneLife instance;

        #endregion

        #region Property

        public static SceneLife In => instance = instance ? instance : FindObjectOfType<SceneLife>() ?? new GameObject(nameof(SceneLife)).AddComponent<SceneLife>();

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            if (instance) Destroy(gameObject);
        }

        #endregion
    }
}