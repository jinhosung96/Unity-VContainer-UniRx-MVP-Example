using Mine.Code.Main.System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Jelly
{
    public class JellyContext : LifetimeScope
    {
        #region Properties

        [field: SerializeField] public JellyModel Model { get; private set; }
        public Animator Animator => GetComponent<Animator>();

        #endregion
    
        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // System 등록
            builder.Register<ClickerSystem>(Lifetime.Singleton);
            builder.Register<GrowUpSystem>(Lifetime.Singleton);

            // Model 등록
            builder.RegisterInstance(Model);

            // View 등록
            builder.RegisterComponent(gameObject);
            builder.RegisterComponent(Animator);

            // Presenter 등록
            builder.RegisterEntryPoint<JellyAIPresenter>();
            builder.RegisterEntryPoint<JellyClickPresenter>();
            builder.RegisterEntryPoint<JellyDragPresenter>();
            builder.RegisterEntryPoint<JellyGrowUpPresenter>();
        }

        #endregion
    }
}
