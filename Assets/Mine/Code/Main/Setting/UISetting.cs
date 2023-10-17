using Mine.Code.Framework.Manager.Sound;
using UnityEngine;

namespace Mine.Code.Main.Setting
{
    [CreateAssetMenu(fileName = "UISetting", menuName = "ScriptableObjects/UISetting")]
    public class UISetting : ScriptableObject
    {
        #region Properties

        [field: SerializeField] public SoundClip Button { get; private set; }
        [field: SerializeField] public SoundClip Buy { get; private set; }
        [field: SerializeField] public SoundClip Clear { get; private set; }
        [field: SerializeField] public SoundClip Fail { get; private set; }
        [field: SerializeField] public SoundClip Grow { get; private set; }
        [field: SerializeField] public SoundClip PauseIn { get; private set; }
        [field: SerializeField] public SoundClip PauseOut { get; private set; }
        [field: SerializeField] public SoundClip Sell { get; private set; }
        [field: SerializeField] public SoundClip Touch { get; private set; }
        [field: SerializeField] public SoundClip UpgradeOrUnlock { get; private set; }

        #endregion
    }
}