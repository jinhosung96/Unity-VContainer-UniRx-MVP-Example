using Mine.Code.Framework.Extension;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Jelly
{
    public class JellyClickPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyModel model;
        [Inject] readonly ClickerSystem clickerSystem;
    
        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            InitializeView();
        }

        void InitializeView()
        {
            // 젤리 클릭 시 처리
            Context.OnMouseDownAsObservable().Subscribe(_ => clickerSystem.Click(Context)).AddTo(Context.gameObject);
        }

        #endregion
    }
}
