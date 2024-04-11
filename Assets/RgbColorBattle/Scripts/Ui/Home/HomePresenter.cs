using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using DummyEgg.ProjectGK.Model;
using static DummyEgg.ProjectGK.Model.HeroModel;

namespace DummyEgg.ProjectGK.Home
{
    public class HomePresenter : IDisposable
    {
        public virtual void Dispose()
        {
            //DebugLog.Log("Presenter dispose.....");
        }

        readonly HomeView _view;

        [Inject]
        public HomePresenter(HomeView view)
        {
            _view = view;
        }
        
        public async UniTask Prepare()
        {
            HeroModel.Instance.SetupExp(0);

            //_view.OnClickHeroIcon.Subscribe(_ => _view.OpenHeroStatus()).AddTo(_view);
            //_view.OnClickTile.Subscribe(_ => _view.BackToTitle()).AddTo(_view);

            //HeroModel.Instance.NowLevel.Subscribe(value => _view.TxtLevel.text = $"LV{value}").AddTo(_view);

            //HeroModel.Instance.NowExp.Subscribe(value => {
            //    _view.ExpBar.NowValue = (float)value;
            //    _view.ExpBar.LimitValue = (float)HeroModel.Instance.NextExp.Value;
            //    _view.ExpBar.UpdateNowValue();
            //}).AddTo(_view);

            HeroModel.Instance.NOW_SCORE.Subscribe(value => {
                _view.TxtScore.text = string.Format("{0}", value);
            }).AddTo(_view);

            HeroModel.Instance.Hp.Subscribe(value => {
                _view.HpBar.NowValue = (float)value;
                _view.HpBar.LimitValue = (float)HeroModel.Instance.GetBaseData().hp;
                _view.HpBar.UpdateNowValue();
            }).AddTo(_view);

            HeroModel.Instance.IS_GAME_OVER.Subscribe(value => {
                if (value)
                {
                    var u = UiManagerRoot.Instance;
                    u.PushUi(u.UiPrefabSo.GameOverViewSo);
                }
            }).AddTo(_view);
            

            HeroModel.Instance.Mp.Subscribe(value => {
                _view.EnBar.NowValue = (float)value;
                _view.EnBar.LimitValue = (float)HeroModel.Instance.GetBaseData().mp;
                _view.EnBar.UpdateNowValue();
            }).AddTo(_view);

            HeroModel.Instance.MpRed.Subscribe(value => {
                _view.ElementBars[(int)ELE_TYPE.RED].NowValue = (float)value;
                _view.ElementBars[(int)ELE_TYPE.RED].LimitValue = (float)HeroModel.Instance.GetBaseData().mp_red;
                _view.ElementBars[(int)ELE_TYPE.RED].UpdateNowValue();
            }).AddTo(_view);

            HeroModel.Instance.MpGreen.Subscribe(value => {
                _view.ElementBars[(int)ELE_TYPE.GREEN].NowValue = (float)value;
                _view.ElementBars[(int)ELE_TYPE.GREEN].LimitValue = (float)HeroModel.Instance.GetBaseData().mp_green;
                _view.ElementBars[(int)ELE_TYPE.GREEN].UpdateNowValue();
            }).AddTo(_view);

            HeroModel.Instance.MpBlue.Subscribe(value => {
                _view.ElementBars[(int)ELE_TYPE.BLUE].NowValue = (float)value;
                _view.ElementBars[(int)ELE_TYPE.BLUE].LimitValue = (float)HeroModel.Instance.GetBaseData().mp_blue;
                _view.ElementBars[(int)ELE_TYPE.BLUE].UpdateNowValue();
            }).AddTo(_view);


            HeroModel.Instance.NOW_ELETYPE.Subscribe(value => {
                _view.OnSelectElement((int)value);
            }).AddTo(_view);


            HeroModel.Instance.IS_PAUSE.Subscribe(value => {
                _view.ShowPauseUi(value);
            }).AddTo(_view);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
    }
}
