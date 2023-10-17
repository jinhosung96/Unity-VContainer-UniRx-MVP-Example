using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using Mine.Code.Main.System;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.UI.Plant
{
    public class PlantModalPresenter : VObject<PlantModalContext>, IStartable
    {
        #region Fields

        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly PlantModalContext.UIView view;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;
        [Inject] readonly UpgradeSystem upgradeSystem;

        #endregion
    
        #region Entry Point

        void IStartable.Start()
        {
            InitializeModel();
            InitializeView();
        }

        #endregion

        #region Private Methods

        void InitializeModel()
        {
            // UpgradeModel의 ApartmentLevel 데이터의 갱신 시 UI 갱신
            upgradeModel.ApartmentLevel.Subscribe(level =>
            {
                view.ApartmentSubText.text = $"젤리 수용량 {(level + 1) * 2}";
                if(level < upgradeModel.ApartmentMaxLevel) view.ApartmentCostText.text = $"{upgradeModel.ApartmentUpgradeCost}";
                else view.ApartmentUpgradeButton.gameObject.SetActive(false);
            }).AddTo(Context);

            // UpgradeModel의 ClickLevel 데이터의 갱신 시 UI 갱신    
            upgradeModel.ClickLevel.Subscribe(level =>
            {
                view.ClickSubText.text = $"클릭 생산량 {level + 1}";
                if(level < upgradeModel.ClickMaxLevel) view.ClickCostText.text = $"{upgradeModel.ClickUpgradeCost}";
                else view.ClickUpgradeButton.gameObject.SetActive(false);
            }).AddTo(Context);
        }

        void InitializeView()
        {
            // Apartment Upgrade Button 클릭 시 필드의 젤리 수용량 업그레이드
            view.ApartmentUpgradeButton.OnClickAsObservable().Subscribe(_ =>
            {
                soundManager.PlaySfx(uISetting.Button);
                upgradeSystem.ApartmentUpgrade();
            }).AddTo(Context);

            // Click Upgrade Button 클릭 시 클릭 재화 수화량 업그레이드
            view.ClickUpgradeButton.OnClickAsObservable()
                .Where(_ => upgradeModel.ClickLevel.Value < upgradeModel.ClickMaxLevel)
                .Subscribe(_ =>
                {
                    soundManager.PlaySfx(uISetting.Button);
                    upgradeSystem.ClickUpgrade();
                })
                .AddTo(Context);
        }

        #endregion
    }
}
