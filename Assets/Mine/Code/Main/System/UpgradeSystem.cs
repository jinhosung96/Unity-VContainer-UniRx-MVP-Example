using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using VContainer;

namespace Mine.Code.Main.System
{
    public class UpgradeSystem
    {
        #region Fields

        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;

        #endregion
    
        #region Public Methods

        public void ApartmentUpgrade()
        {
            if(upgradeModel.ApartmentLevel.Value >= upgradeModel.ApartmentMaxLevel) return;
            if (currencyModel.Gold.Value >= upgradeModel.ApartmentUpgradeCost)
            {
                currencyModel.Gold.Value -= upgradeModel.ApartmentUpgradeCost;
                upgradeModel.ApartmentLevel.Value++;
            }
            else soundManager.PlaySfx(uISetting.Fail);
        }
    
        public void ClickUpgrade()
        {
            if(upgradeModel.ClickLevel.Value >= upgradeModel.ClickMaxLevel) return;
            if (currencyModel.Gold.Value >= upgradeModel.ClickUpgradeCost)
            {
                currencyModel.Gold.Value -= upgradeModel.ClickUpgradeCost;
                upgradeModel.ClickLevel.Value++;
            }
            else soundManager.PlaySfx(uISetting.Fail);
        }

        #endregion
    }
}
