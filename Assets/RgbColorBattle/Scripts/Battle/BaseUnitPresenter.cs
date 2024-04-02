using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using DummyEgg.ProjectGK.Battle;

namespace DummyEgg.ProjectGK.Battle
{
    public class BaseUnitPresenter
    {
        BaseUnitObj _unitObj;
        Model.EasyUnitDataModel _model;
        private List<IDisposable> _disps = new();

        public BaseUnitPresenter(BaseUnitObj baseUnit)
        {
            _unitObj = baseUnit;
            _model = new();
        }

        public void Setup()
        {
            _model.BaseHp = _unitObj.HP;
            _model.Setup();

            _model.Now_ShieldType.Value = (Model.HeroModel.ELE_TYPE)UnityEngine.Random.Range(0, (int)Model.HeroModel.ELE_TYPE.TOTAL);

            foreach (var _ in _disps)
                _.Dispose();
            _disps.Clear();

            _disps.Add(_model.Now_ShieldType.Subscribe(value => {
                _unitObj.GetShield().ChangeShield(value);
            }).AddTo(_unitObj));

            _disps.Add(Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(_unitObj.GetShield().ChangeTime))
                .Subscribe(x =>
                {
                    if (_model.Hp.Value <= 0)
                        return;
                    var setval = (int)_model.Now_ShieldType.Value;
                    ++setval;
                    if (setval >= (int)Model.HeroModel.ELE_TYPE.TOTAL)
                        setval = 0;
                    _model.Now_ShieldType.Value = (Model.HeroModel.ELE_TYPE)setval;
                }).AddTo(_unitObj));

            _disps.Add(_model.Hp.Subscribe(value => {
                if (_model.Hp.Value <= 0)
                {
                   _unitObj.DoDestory().Forget();
                }
            }).AddTo(_unitObj));

            _disps.Add(_unitObj.WhenTriggerEnter.Subscribe((go) => {
                switch (go.gameObject.layer)
                {
                    case (int)PlayerPresenter.GK_LAYER.PLAYER_BULLET:
                        //todo HeroModelÇÃêîílòAåg
                        var bullet = go.GetComponent<BaseBullet>();
                        if (bullet.ETYPE == _model.Now_ShieldType.Value)
                        {
                            _model.ChangeHp(-20);
                            _unitObj.DoHurtEffect();
                            if (_model.Hp.Value <= 0)
                                Model.HeroModel.Instance.NOW_SCORE.Value += _unitObj.SCORE;
                        }
                        //GameObject.Destroy(_unitObj.gameObject);
                        break;
                    default:
                        break;
                }
            }));
        }
    }
}