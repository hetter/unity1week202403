using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace DummyEgg.ProjectGK
{
    public class TransitionManager : SingletonClass<TransitionManager>
    {
        string _currentSceneName;
       // AsyncOperationHandle<SceneInstance> _sceneLoadHandle;

        public string GetCurrentSceneName() => Instance._currentSceneName;

        public async UniTask LoadSceneWithFade(string sceneName)
        {
            UiManagerRoot.Instance.ShowWaitView();

            await LoadSceneAsync(sceneName);

            await UniTask.Delay(250); //Load エミュレーター

            UiManagerRoot.Instance.CloseWaitView();
        }


        public async UniTask UnLoadCurrentSceneAsync()
        {
            if (!string.IsNullOrEmpty(Instance._currentSceneName))
            {
                //await Addressables.UnloadSceneAsync(Instance._sceneLoadHandle);
                //SceneManager.UnloadScene(sceneName);
                Instance._currentSceneName = null;
                await Resources.UnloadUnusedAssets();
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            await UnLoadCurrentSceneAsync();

            //Instance._sceneLoadHandle = Addressables.LoadSceneAsync(sceneName, mode);

            await SceneManager.LoadSceneAsync(sceneName);

          //  await Instance._sceneLoadHandle;
           // var scene = Instance._sceneLoadHandle.Result;
            Instance._currentSceneName = sceneName;
        }
    }
}


