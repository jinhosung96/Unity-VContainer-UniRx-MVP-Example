using Mine.Code.App.Common;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using Mine.Code.Main.System;
using VContainer;

namespace Mine.Code.Jelly
{
    public class ClickerSystem
    {
        #region Fields

        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly GrowUpSystem growUpSystem;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;

        #endregion
    
        #region Public Methods

        public void Click(JellyContext jellyContext)
        {
            soundManager.PlaySfx(uISetting.Touch);
        
            StopJelly(jellyContext);
            GetGelatin(jellyContext);
            growUpSystem.GetExpByClick(jellyContext);
        }

        public void GetGelatin(JellyContext jellyContext)
        {
            currencyModel.Gelatin.Value += jellyContext.Model.GelatinByClick;
        }

        public void StopJelly(JellyContext jellyContext)
        {
            jellyContext.Animator.SetTrigger(Constants.DoTouch);
            jellyContext.Model.AI.Transition(JellyModel.JellyState.Idle);
        }

        #endregion
    }
}