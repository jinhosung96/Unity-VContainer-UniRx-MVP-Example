using Mine.Code.Framework.Extension;
using Mine.Code.Main.Setting;
using Mine.Code.Main.System;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Jelly
{
    public class JellyDragPresenter : VObject<JellyContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyModel model;
        [Inject] readonly ShopSystem shopSystem;
        [Inject] readonly MainSetting mainSetting;

        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            InitializeView();
        }

        void InitializeView()
        {
            Vector3 delta = Vector2.zero;

            // 젤리 클릭 시 마우스 포인터와 젤리 사이의 delta 값 캐싱 및 해당 젤리의 렌더링 순서를 UI보다 앞으로 전환
            Context.OnMouseDownAsObservable().Subscribe(_ =>
            {
                delta = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - Context.transform.position.DropZ();
                Context.GetComponent<SpriteRenderer>().sortingOrder = 11;
            }).AddTo(Context.gameObject);

            // 젤리 드래그 시 마우스 포인터를 delta값을 유지한 체 따라오게 하고 Idle 상태로 강제 전환
            Context.OnMouseDragAsObservable().Subscribe(_ =>
            {
                Context.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - delta;
                model.AI.Transition(JellyModel.JellyState.Idle);
            }).AddTo(Context.gameObject);

            // 판매 버튼 위에서 마우스 클릭을 멈출 시 판매
            // 필드 밖일 경우 랜덤 위치에 반납
            // 필드 안일 경우 해당 위치에 반납
            Context.OnMouseUpAsObservable()
                .DelayFrame(1) // 판매 가능 여부를 판단하기 위한 시간을 번다.
                .Subscribe(_ =>
                {
                    if (shopSystem.IsActiveSell) shopSystem.Sell((JellyContext)Context);
                    else if (!Context.transform.position.IsInRange(mainSetting.MinRange, mainSetting.MaxRange))
                        Context.transform.position = mainSetting.RandomPositionInField;
                    Context.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }).AddTo(Context.gameObject);
        }

        #endregion
    }
}