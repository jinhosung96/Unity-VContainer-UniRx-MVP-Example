using System;
using DG.Tweening;
using Mine.Code.App.Common;
using Mine.Code.Main.Setting;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Mine.Code.Jelly
{
    public class JellyAIPresenter : VObject<JellyContext>, IStartable
    {
        [Inject] readonly JellyModel model;
        [Inject] readonly MainSetting mainSetting;
        IDisposable disposable;

        void IStartable.Start()
        {
            InitializeModel();
        }

        void InitializeModel()
        {
            // AI State 정의
            InitializeIdleState();
            InitializeMoveState();

            // AI 작동 시작
            model.AI.StartFsm(JellyModel.JellyState.Idle);
        }

        void InitializeIdleState()
        {
            // Idle 상태 진입 시 처리
            // 애니메이션 갱신 및 일정 시간 후 Move 상태로 전환
            model.AI.OnBeginState(JellyModel.JellyState.Idle).Subscribe(_ =>
            {
                Context.GetComponent<Animator>().SetBool(Constants.IsWalk, false);
                disposable = Observable.Timer(TimeSpan.FromSeconds(Random.Range(0.5f, 3f)))
                    .Subscribe(_ => model.AI.Transition(JellyModel.JellyState.Move))
                    .AddTo(Context.gameObject);
            }).AddTo(Context.gameObject);

            // Idle 상태 종료 시 처리
            // 중복 실행 방지를 위해 스트림 종료
            model.AI.OnEndState(JellyModel.JellyState.Idle).Subscribe(_ => disposable.Dispose()).AddTo(Context.gameObject);
        }

        void InitializeMoveState()
        {
            // Move 상태 진입 시 처리
            // 애니메이션 갱신 및 랜덤 위치로 이동 및 도착 시 Idle 상태로 전환
            model.AI.OnBeginState(JellyModel.JellyState.Move).Subscribe(_ =>
            {
                Context.GetComponent<Animator>().SetBool(Constants.IsWalk, true);
    
                // 랜덤한 위치로 일정 속도로 이동
                var randomPosition = mainSetting.RandomPositionInField;
                Context.GetComponent<SpriteRenderer>().flipX = Context.transform.position.x > randomPosition.x;
                Context.transform.DOMove(randomPosition, 1f)
                    .SetSpeedBased().SetEase(Ease.Linear)
                    .OnComplete(() => model.AI.Transition(JellyModel.JellyState.Idle));
            }).AddTo(Context.gameObject);

            // Move 상태 종료 시 처리
            // 중복 실행 방지를 위한 스트림 종료
            model.AI.OnEndState(JellyModel.JellyState.Move).Subscribe(_ => Context.transform.DOKill()).AddTo(Context.gameObject);
        }
    }
}
