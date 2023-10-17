using Mine.Code.App.Model;
using Mine.Code.Framework.StateMachine;
using Mine.Code.Framework.UniRxCustom;
using Mine.Code.Main.Model;
using UnityEngine;
using VContainer;

namespace Mine.Code.Jelly
{
    [System.Serializable]
    public class JellyModel
    {
        #region Inner Structs

        [System.Serializable]
        public struct SaveData
        {
            public int id;
            public int level;
            public int exp;
        }

        #endregion

        #region Enum

        public enum JellyState
        {
            Idle,
            Move
        }

        #endregion

        #region Fields

        [Inject] readonly FieldModel fieldModel;
        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;

        #endregion
    
        #region Properties

        [field: SerializeField] public int Id { get; private set; }
        public IntReactivePropertyWithRange Level { get; } = new(1, 1, 3);
        public IntReactivePropertyWithRange Exp { get; } = new(0, 50);
        public int GelatinByClick => (Id + 1) * Level.Value * (upgradeModel.ClickLevel.Value + 1);
        public int GelatinByTime => (Id + 1) * Level.Value;
        public int JellyPrice => (Id + 1) * jellyFarmDBModel.JellyPresets[Id].Value<int>("jellyCost") * Level.Value;
        public StateMachine<JellyState> AI { get; } = new();
        public SaveData Data => new() { id = Id, level = Level.Value, exp = Exp.Value };

        #endregion

        #region Public Methods

        public void Respawn()
        {
            Level.Value = 0;
            Exp.Value = 0;
            fieldModel.Jellies.Add(this);
        }

        public void Load(int level, int exp)
        {
            Level.Value = level;
            Exp.Value = exp;
        }
    
        public void Despawn()
        {
            fieldModel.Jellies.Remove(this);
        }

        #endregion
    }
}
