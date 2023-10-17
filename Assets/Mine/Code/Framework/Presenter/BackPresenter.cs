#if UNIRX_SUPPORT && UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.UINavigator.Runtime;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Mine.Code.Framework.Presenter
{
    public class BackPresenter : IStartable
    {
        void IStartable.Start() => Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(_ => UIContainer.BackAsync().Forget());
    }
}

#endif