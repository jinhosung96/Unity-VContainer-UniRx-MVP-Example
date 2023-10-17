using Mine.Code.Framework.Manager.UINavigator.Runtime.Modal;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.UI.Main
{
    public class MainSheetContext : Sheet
    {
        #region Inner Classes

        [global::System.Serializable]
        public class UIView
        {
            [field: SerializeField] public Button JellyButton { get; private set; }
            [field: SerializeField] public Button PlantButton { get; private set; }
            [field: SerializeField] public Button SellButton { get; private set; }
            [field: SerializeField] public TextMeshProUGUI GelatinText { get; private set; }
            [field: SerializeField] public TextMeshProUGUI GoldText { get; private set; }
        }

        #endregion
    
        #region Properties
    
        [field: SerializeField] public UIView View { get; private set; }

        #endregion

        #region Override Methods

        protected override void Configure(IContainerBuilder builder)
        {
            // View 등록
            builder.RegisterInstance(View);

            // Presenter 등록
            builder.RegisterEntryPoint<MainSheetPresenter>();
        }

        #endregion
    }
}
