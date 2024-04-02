using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using DummyEgg.ProjectGK.Home;
using DummyEgg.ProjectGK.Model;

namespace DummyEgg.ProjectGK.Title
{
    public class TitlePresenter : IDisposable
    {
        public virtual void Dispose()
        {
            //DebugLog.Log("Presenter dispose.....");
        }

        readonly TitleView _view;


        [Inject]
        public TitlePresenter(TitleView view)
        {
            _view = view;
        }

        
        public async UniTask Prepare()
        {
            _view.OnClickLogin.Subscribe(_ =>
                {
                    Model.PlayerModel.Instance.JobId = 100 + (_view.Dp_job.value + 1);
                    Model.PlayerModel.Instance.PlayerName = _view.GetInputName();
                    var homeView = _view.GoToHomeView();
                    //var homtLifeTime = homeView.GetComponent<HomeLifetimeScope>();      
                }).AddTo(_view);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
    }
}
