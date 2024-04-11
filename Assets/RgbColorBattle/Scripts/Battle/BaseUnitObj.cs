using DummyEgg.ProjectGK.Model;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DummyEgg.ProjectGK.Battle
{
    public class BaseUnitObj : PoolObject
    {
        //[SerializeField] MMFeedbacks _hurtEffect;
        //[SerializeField] MMFeedbacks _destoryEffect;

        [SerializeField] GameObject _hurtEffectObj;
        [SerializeField] GameObject _destoryEffectObj;
        [SerializeField] GameObject _model;
        [SerializeField] FlyObjShooter _shooter;

        //todo マスターデータ連携
        [SerializeField] public int HP = 50;
        [SerializeField] public int SCORE = 50;
        public bool IsShoot;

        BaseUnitPresenter _presenter;

        public Subject<GameObject> WhenTriggerEnter = new();

        public Subject<GameObject> WhenTriggerExit = new();


        public ElementShield GetShield()
        {
            return _elementShield;
        }
        [SerializeField] ElementShield _elementShield;
        // Start is called before the first frame update

        private void OnEnable()
        {
            if (_presenter == null)
                _presenter = new(this);
            this._model.SetActive(true);
            _presenter.Setup();
            if (_shooter != null)
                _shooter.gameObject.SetActive(true);
        }

        public void DoHurtEffect()
        {
            EasyAudioManager.Instance.HitEffect.PlayFeedbacks();
            //_hurtEffect.PlayFeedbacks();
            _hurtEffectObj.gameObject.SetActive(false);
            _hurtEffectObj.gameObject.SetActive(true);
        }

        public async UniTask DoDestory()
        {
            //await _destoryEffect.PlayFeedbacksTask();
            this._model.SetActive(false);
            //_destoryEffect.PlayFeedbacks();
            _destoryEffectObj.gameObject.SetActive(false);
            _destoryEffectObj.gameObject.SetActive(true);
            if (_shooter != null)
                _shooter.gameObject.SetActive(false);

            //削除エフェクトはTimeManagerに管理されなくでもok
            await UniTask.Delay(600);
            //await TimeManager.Instance.Delay(0.6f);


            _destoryEffectObj.gameObject.SetActive(false);
            _hurtEffectObj.gameObject.SetActive(false);

            DestoryPoolObj();
        }

        private void OnTriggerEnter(Collider other)
        {
            WhenTriggerEnter.OnNext(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            WhenTriggerExit.OnNext(other.gameObject);
        }
    }
}