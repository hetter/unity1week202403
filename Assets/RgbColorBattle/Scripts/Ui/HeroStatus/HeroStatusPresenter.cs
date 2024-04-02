using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using DummyEgg.ProjectGK.Model;
using DummyEgg.ProjectGK.MasterData;

namespace DummyEgg.ProjectGK.HeroUi
{
    public class HeroStatusPresenter : IDisposable
    {
        public virtual void Dispose()
        {
            //DebugLog.Log("Presenter dispose.....");
        }

        readonly HeroStatusView _view;

        [Inject]
        public HeroStatusPresenter(HeroStatusView view)
        {
            _view = view;
        }
        
        public async UniTask Prepare()
        {
            _view.OnClickClose.Subscribe(_ => _view.DoClose()).AddTo(_view);
            _view.OnClickAddExp.Subscribe(_ => HeroModel.Instance.AddExp(_view.GetAddExp())).AddTo(_view);
            HeroModel.Instance.Hp.Subscribe(value => _view.Txt_hp.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Mp.Subscribe(value => _view.Txt_mp.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Phy_atk.Subscribe(value => _view.Txt_phyAtk.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Phy_def.Subscribe(value => _view.Txt_phyDef.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Mag_atk.Subscribe(value => _view.Txt_magAtk.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Mag_def.Subscribe(value => _view.Txt_magDef.text = $"{value}").AddTo(_view);
            HeroModel.Instance.Dex.Subscribe(value => _view.Txt_dex.text = $"{value}").AddTo(_view);

            _view.Txt_name.text = PlayerModel.Instance.PlayerName;
            _view.Txt_job.text = MstJobDescModel.GetRecordBy(PlayerModel.Instance.JobId).name;

            HeroModel.Instance.NowExp.Subscribe(value => _view.Txt_exp.text = $"{value}/{HeroModel.Instance.NextExp.Value}").AddTo(_view);
            //HeroModel.Instance.NextExp.Subscribe(value => _view.Txt_nextExp.text = $"{value}").AddTo(_view);
            HeroModel.Instance.NowLevel.Subscribe(value => _view.Txt_level.text = $"{value}").AddTo(_view);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }

        public void foo()
        {

        }
    }
}
