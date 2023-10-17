using Mine.Code.App.Model;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Main.Setting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.App.Context
{
    public class AppContext : LifetimeScope
    {
        #region Properties

        [field: SerializeField] public JellyFarmJsonDBModel JellyFarmDBModel { get; private set; }
        [field: SerializeField] public UISetting UISetting { get; private set; }
        [field: SerializeField] public MainSetting MainSetting { get; private set; }

        #endregion
    
        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // Setting 등록
            builder.RegisterInstance(UISetting);
            builder.RegisterInstance(MainSetting);

            // Manager 등록
            builder.RegisterEntryPoint<SoundManager>().AsSelf();

            // Model 등록
            JellyFarmDBModel.LoadDB("JellyPreset", "Currency", "Field", "Upgrade", "Plant");
            builder.RegisterInstance(JellyFarmDBModel);
        }

        #endregion
    }
}
