using System;
using Mine.Code.App.Model;
using Mine.Code.Framework.UniRxCustom;
using VContainer;

namespace Mine.Code.Main.Model
{
    [Serializable]
    public class CurrencyModel
    {
        #region Inner Structs

        [Serializable]
        public struct SaveData
        {
            public int gelatin;
            public int gold;
        }

        #endregion

        #region Fields
    
        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        IntReactivePropertyWithRange gelatin;
        IntReactivePropertyWithRange gold;

        #endregion

        #region Properties

        public IntReactivePropertyWithRange Gelatin
        {
            get
            {
                if (gelatin is null) gelatin = new IntReactivePropertyWithRange((int)jellyFarmDBModel.Gelatin, 0, 999999999);
                return gelatin;
            }
        }

        public IntReactivePropertyWithRange Gold
        {
            get
            {
                if (gold is null) gold = new IntReactivePropertyWithRange((int)jellyFarmDBModel.Gold, 0, 999999999);
                return gold;
            }
        }
        public SaveData Data => new () { gelatin = Gelatin.Value, gold = Gold.Value };

        #endregion
    }
}