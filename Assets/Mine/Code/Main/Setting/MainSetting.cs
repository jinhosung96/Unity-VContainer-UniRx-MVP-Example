using Mine.Code.Framework.Extension;
using UnityEngine;

namespace Mine.Code.Main.Setting
{
    [CreateAssetMenu(fileName = "MainSetting", menuName = "ScriptableObjects/MainSetting")]
    public class MainSetting : ScriptableObject
    {
        #region Properties

        [field: SerializeField] public RuntimeAnimatorController[] ControllersByLevel { get; private set; }
        [field: SerializeField] public Vector2 MinRange { get; private set; }
        [field: SerializeField] public Vector2 MaxRange { get; private set; }
        public Vector3 RandomPositionInField => MinRange.RandomPosition(MaxRange);

        #endregion
    }
}
