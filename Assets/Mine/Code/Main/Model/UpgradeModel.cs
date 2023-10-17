using System;
using System.Linq;
using Mine.Code.App.Model;
using Mine.Code.Framework.UniRxCustom;
using VContainer;

namespace Mine.Code.Main.Model
{
    public class UpgradeModel
    {
        #region Inner Structs

        [Serializable]
        public struct SaveData
        {
            public int apartmentLevel;
            public int clickLevel;
        }

        #endregion

        #region Fields
    
        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        IntReactivePropertyWithRange apartmentLevel;
        IntReactivePropertyWithRange clickLevel;

        #endregion

        #region Properties

        public IntReactivePropertyWithRange ApartmentLevel
        {
            get
            {
                if (apartmentLevel is null) apartmentLevel = new IntReactivePropertyWithRange((int)jellyFarmDBModel.ApartmentLevel, 0, 4);
                return apartmentLevel;
            }
        }

        public IntReactivePropertyWithRange ClickLevel
        {
            get
            {
                if (clickLevel is null) clickLevel = new IntReactivePropertyWithRange((int)jellyFarmDBModel.ClickLevel, 0, 4);
                return clickLevel;
            }
        }
        public int ApartmentMaxLevel => jellyFarmDBModel.Apartment.Count();
        public int ClickMaxLevel => jellyFarmDBModel.Click.Count();
        public int ApartmentUpgradeCost => (int)jellyFarmDBModel.Apartment[ApartmentLevel.Value]["cost"];
        public int ClickUpgradeCost => (int)jellyFarmDBModel.Click[ClickLevel.Value]["cost"];
        public SaveData Data => new () { apartmentLevel = ApartmentLevel.Value, clickLevel = ClickLevel.Value };

        #endregion
    }
}
