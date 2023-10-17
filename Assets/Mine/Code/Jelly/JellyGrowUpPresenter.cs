using System;
using Mine.Code.Main.System;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Jelly
{
    public class JellyGrowUpPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyModel model;
        [Inject] readonly GrowUpSystem growUpSystem;
        readonly int maxLevel = 3;
        readonly int maxExp = 50;

        #endregion
    
        #region Entry Point

        void IStartable.Start()
        {
            InitializeModel();
            InitializeScheduler();
        }

        void InitializeModel()
        {
            // Model의 경험치 데이터가 갱신됬을 때 최대 경험치를 충족 시켰을 시 레벨 업 처리
            model.Exp.Where(exp => exp >= maxExp).Subscribe(exp => growUpSystem.LevelUp(Context)).AddTo(Context.gameObject);
            model.Level.Subscribe(_ => growUpSystem.LevelUpEvent(Context)).AddTo(Context.gameObject);
        }

        void InitializeScheduler()
        {
            // 1초마다 경험치 획득
            Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeWhile(_ => model.Level.Value < maxLevel)
                .Where(_ => Context.gameObject.activeInHierarchy)
                .Subscribe(_ => growUpSystem.GetExpByTime(Context))
                .AddTo(Context.gameObject);
	
            // 3초마다 Gelatin 획득
            Observable.Interval(TimeSpan.FromSeconds(3f))
                .Where(_ => Context.gameObject.activeInHierarchy)
                .Subscribe(_ => growUpSystem.AutoGetGelatin(Context))
                .AddTo(Context.gameObject);
        }

        #endregion
    }
}
