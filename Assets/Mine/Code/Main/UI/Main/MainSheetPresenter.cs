using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using Mine.Code.Main.System;
using Mine.Code.Main.UI.Jelly;
using Mine.Code.Main.UI.Plant;
using UniRx;
using UniRx.Triggers;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.UI.Main
{
    public class MainSheetPresenter : VObject<MainSheetContext>, IStartable
    {
        #region Fields
    
        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly ShopSystem shopSystem;
        [Inject] readonly MainSheetContext.UIView view;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;
    
        #endregion
    
        #region Entry Point

        void IStartable.Start()
        {
            // 버튼 및 텍스트를 초기화 합니다.
            InitializeModel();
            InitializeView();
        }

        #endregion

        #region Private Methods

        void InitializeModel()
        {
            // CurrencyModel의 젤라틴 및 골드 데이터가 갱신될 시 이를 UI 텍스트에 반영한다.
            currencyModel.Gelatin.Subscribe(gelatin => view.GelatinText.text = gelatin.ToString("N0")).AddTo(Context);
            currencyModel.Gold.Subscribe(gold => view.GoldText.text = gold.ToString("N0")).AddTo(Context);
        }

        void InitializeView()
        {
            // 젤리 버튼을 클릭 시 JellySheet를 연다.
            view.JellyButton.OnClickAsObservable().Subscribe(_ =>
            {
                ModalContainer.Main.NextAsync<JellyModalContext>().Forget();
                soundManager.PlaySfx(uISetting.Button);
            }).AddTo(Context);
            
            // 플랜트 버튼을 클릭 시 JellySheet를 연다.
            view.PlantButton.OnClickAsObservable().Subscribe(_ =>
            {
                ModalContainer.Main.NextAsync<PlantModalContext>().Forget();
                soundManager.PlaySfx(uISetting.Button);
            }).AddTo(Context);

            // 판매 버튼 위에 손가락을 올려진 상태인지를 체크한다.
            // 젤리를 드래그한 상태로 판매 버튼 위에서 손가락을 때면 젤리를 판매한다.
            view.SellButton.OnPointerEnterAsObservable().Subscribe(_ => shopSystem.IsActiveSell = true).AddTo(Context);
            view.SellButton.OnPointerExitAsObservable().Subscribe(_ => shopSystem.IsActiveSell = false).AddTo(Context);
        }

        #endregion
    }
}
