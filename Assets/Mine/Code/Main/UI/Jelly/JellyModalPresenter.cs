using System.Linq;
using Mine.Code.App.Model;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Setting;
using Mine.Code.Main.System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.UI.Jelly
{
    public class JellyModalPresenter : VObject<JellyModalContext>, IStartable
    {
        #region Field

        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        [Inject] readonly JellyModalContext.UIView view;
        [Inject] readonly JellyModalContext.UIModel model;
        [Inject] readonly ShopSystem shopSystem;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;

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
            // 상점 Modal을 활성화 시킬 시 Jelly Preset 목록 중 가장 최근에 Unlock된 Jelly의 Page를 상점에 띄워준다.
            model.CurrentPage.Value = jellyFarmDBModel.JellyPresets.LastOrDefault(preset => preset.Value<bool>("isUnlocked")).Value<int>("id");
            // Model의 현재 Page에 대한 데이터가 갱신될 시 UI 텍스트에 반영한다.
            model.CurrentPage.Subscribe(index => view.JellyIndexText.text = $"#{(index + 1):00}").AddTo(Context.gameObject);

            // Model 갱신에 따른 Unlock 및 Lock Page 연출 처리 방식을 정의 한다.
            InitializeUnlock();
            InitializeLock();
        }

        void InitializeView()
        {
            // Left Button을 클릭 했을 시 Page을 전환한다.
            view.LeftButton.OnClickAsObservable().Subscribe(_ =>
            {
                soundManager.PlaySfx(uISetting.Button);
                model.CurrentPage.Value--;
            }).AddTo(Context.gameObject);

            // Right Button을 클릭 했을 시 Page을 전환한다.
            view.RightButton.OnClickAsObservable().Subscribe(_ =>
            {
                soundManager.PlaySfx(uISetting.Button);
                model.CurrentPage.Value++;
            }).AddTo(Context.gameObject);

            // Buy Button을 누를 시 현재 Page의 Jelly를 구매한다.
            view.BuyButton.OnClickAsObservable().Subscribe(_ => { shopSystem.Buy(model.CurrentPage.Value); }).AddTo(Context.gameObject);

            // Unlock Button을 누를 시 현재 Page의 Jelly를 해금한다.
            view.UnlockButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (shopSystem.Unlock(model.CurrentPage.Value))
                {
                    // 해금 후 현재 Page의 정보를 갱신한다.
                    model.CurrentPage.SetValueAndForceNotify(model.CurrentPage.Value);
                }
            }).AddTo(Context.gameObject);
        }

        void InitializeUnlock()
        {
            // 현재 Page가 Unlock된 Page일 경우 연출 정의
            model.CurrentPage.Select(index => jellyFarmDBModel.JellyPresets[index]).Where(preset => (bool)preset["isUnlocked"]).Subscribe(preset =>
            {
                view.LockFolder.SetActive(false);
                view.UnlockFolder.SetActive(true);
                view.UnlockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
                view.UnlockJellyNameText.text = preset["jellyName"].ToString();
                view.UnlockJellyCostText.text = preset["jellyCost"].ToString();
            }).AddTo(Context.gameObject);
        }

        void InitializeLock()
        {
            // 현재 Page가 Lock된 Page일 경우 연출 정의
            model.CurrentPage.Select(index => jellyFarmDBModel.JellyPresets[index]).Where(preset => !(bool)preset["isUnlocked"]).Subscribe(preset =>
            {
                view.UnlockFolder.SetActive(false);
                view.LockFolder.SetActive(true);
                view.LockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
                view.LockJellyCostText.text = preset["jellyCost"].ToString();
            }).AddTo(Context.gameObject);
        }

        #endregion
    }
}