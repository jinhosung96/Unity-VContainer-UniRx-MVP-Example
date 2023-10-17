using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mine.Code.Framework.Manager.SceneLoader
{
    #region Enum

    public enum LoadState
    {
        Preprocessing,
        Loading,
        Loaded
    }

    #endregion
    
    public class SceneLoader
    {
        #region Fields

        Subject<(string prev, string next)> receiveOrderSubject = new();
        Subject<(string prev, string next)> preLoadSubject = new();
        Subject<(string prev, string next, AsyncOperation loadSceneAsync)> loadingSubject = new();
        Subject<(string prev, string next)> postLoadSubject = new();

        #endregion

        #region Properties

        public IObservable<(string prev, string next)> OnReceiveOrder => receiveOrderSubject.FirstOrDefault().Share();
        public IObservable<(string prev, string next)> OnPreLoad => preLoadSubject.FirstOrDefault().Share();
        public IObservable<(string prev, string next, AsyncOperation loadSceneAsync)> OnLoading => loadingSubject.FirstOrDefault().Share();
        public IObservable<(string prev, string next)> OnPostLoad => postLoadSubject.FirstOrDefault().Share();

        public string CurrentScene => SceneManager.GetActiveScene().name;
        public string PrevScene { get; private set; }

        public LoadState LoadState { get; private set; } = LoadState.Loaded;

        #endregion

        #region Public Methods
        
        /// <summary>
        /// 호출 시 지정된 씬으로 전환한다.<br/>
        /// 씬 전환 과정에 따라 OnReceiveOrder -> OnPreLoad -> OnLoading -> OnPostLoad 순으로 이벤트를 호출한다.<br/>
        /// 주의 사항으로는 위 이벤트에 대한 구독 처리는 본 메소드 호출 전에 이루어져야 한다는 점이다.<br/>
        /// 추가로 onPreprocess를 지정해주면 해당 처리가 끝나는 것을 대기하고 씬 전환이 이루어 진다.<br/>
        /// <br/>
        /// OnReceiveOrder : 씬 전환 명령을 받았을 때<br/>
        /// OnPreLoad : 전처리 직 후, 씬 전환 전<br/>
        /// OnLoading : 씬 전환 중 매 프레임, 인자로 loadSceneAsync 정보를 넘겨준다.<br/>
        /// OnPostLoad : 씬 전환 직후 호출된다.<br/>
        /// </summary>
        /// <param name="targetScene"> 전환될 씬 </param>
        /// <param name="onPreprocess"> 씬 전환 이전에 호출할 전처리 </param>
        public async UniTask LoadSceneAsync(string targetSceneName, Func<UniTask> onPreprocess = null)
        {
            string prev = CurrentScene;
            string next = targetSceneName;

            LoadState = LoadState.Preprocessing;
            receiveOrderSubject.OnNext((prev, next));
            
            if (onPreprocess != null) await onPreprocess();
            
            LoadState = LoadState.Loading;
            preLoadSubject.OnNext((prev, next));

            var loadSceneAsync = SceneManager.LoadSceneAsync(next);

            while (!loadSceneAsync.isDone)
            {
                loadingSubject.OnNext((prev, next, loadSceneAsync));
                await UniTask.Yield();
            }

            PrevScene = prev;
            LoadState = LoadState.Loaded;
            postLoadSubject.OnNext((prev, next));
        }

        public async UniTask LoadSceneAsync(int targetBuildIndex, Func<UniTask> onPreprocess = null)
        {
            string targetScenePath = SceneUtility.GetScenePathByBuildIndex(targetBuildIndex);
            string targetSceneName = System.IO.Path.GetFileNameWithoutExtension(targetScenePath);
            await LoadSceneAsync(targetSceneName, onPreprocess);
        }

        #endregion
    }
}