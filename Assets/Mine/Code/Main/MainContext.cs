using Mine.Code.Framework.Presenter;
using Mine.Code.Main.Model;
using Mine.Code.Main.System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main
{
    public class MainContext : LifetimeScope
    {
        #region Properties

        [field: SerializeField] public MainFolderModel MainFolderModel { get; private set; }

        #endregion

        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // System 등록
            builder.Register<ShopSystem>(Lifetime.Singleton);
            builder.Register<UpgradeSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SaveSystem>().AsSelf();

            // Model 등록
            builder.RegisterInstance(MainFolderModel);
            builder.Register<CurrencyModel>(Lifetime.Singleton);
            builder.Register<FieldModel>(Lifetime.Singleton);
            builder.Register<UpgradeModel>(Lifetime.Singleton);
            builder.Register<MainFactoryModel>(Lifetime.Singleton);

            // Presenter 등록
            builder.RegisterEntryPoint<BackPresenter>();
        }

        #endregion
    }
}
