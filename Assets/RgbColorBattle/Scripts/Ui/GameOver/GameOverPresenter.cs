using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using DummyEgg.ProjectGK.Home;
using DummyEgg.ProjectGK.Model;
using unityroom.Api;

namespace DummyEgg.ProjectGK.GameOver
{
    public class GameOverPresenter : IDisposable
    {
        public virtual void Dispose()
        {
            //DebugLog.Log("Presenter dispose.....");
        }

        readonly GameOverView _view;


        [Inject]
        public GameOverPresenter(GameOverView view)
        {
            _view = view;
        }

        
        public async UniTask Prepare()
        {
            if (HeroModel.Instance.NOW_SCORE.Value > 0)
                UnityroomApiClient.Instance.SendScore(1, HeroModel.Instance.NOW_SCORE.Value, ScoreboardWriteMode.Always);

            HeroModel.Instance.NOW_SCORE.Subscribe(value => {
                _view.TxtScore.text = string.Format("{0}", value);
            }).AddTo(_view);

            _view.OnClickTitle.Subscribe(_ =>
                {
                    Time.timeScale = 0;
                    UiManagerRoot.Instance.UiStackMain.PopAll();
                    var homeView = _view.GoToHomeView();
                    Time.timeScale = 1;
                }).AddTo(_view);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
    }
}
