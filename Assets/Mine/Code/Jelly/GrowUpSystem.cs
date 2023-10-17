using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using VContainer;

namespace Mine.Code.Jelly
{
    public class GrowUpSystem
    {
        #region Fields

        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;
        [Inject] readonly MainSetting mainSetting;

        #endregion
    
        #region Public Methods

        public void LevelUp(JellyContext jellyContext)
        {
            jellyContext.Model.Level.Value++;
        }
    
        public void LevelUpEvent(JellyContext jellyContext)
        {
            soundManager.PlaySfx(uISetting.Grow);
            var animator = jellyContext.Animator;
            animator.runtimeAnimatorController = mainSetting.ControllersByLevel[jellyContext.Model.Level.Value - 1];
            jellyContext.Model.Exp.Value = 0;
        }
    
        public void GetExpByClick(JellyContext jellyContext)
        {
            jellyContext.Model.Exp.Value++;
        }
    
        public void GetExpByTime(JellyContext jellyContext)
        {
            jellyContext.Model.Exp.Value++;
        }

        public void AutoGetGelatin(JellyContext jellyContext)
        {
            currencyModel.Gelatin.Value += jellyContext.Model.GelatinByTime;
        }

        #endregion
    }
}
