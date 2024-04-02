using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DummyEgg.ProjectGK.Model;
using UniRx.Triggers;
using System;

namespace DummyEgg.ProjectGK.Battle
{
    public class PlayerPresenter
    {
       //layer
       public enum GK_LAYER
        {
            LAND = 3,
            FIELD_BOX = 6,
            ENEMY = 7,
            PLAYER = 8,
            ENEMY_BULLET = 9,
            PLAYER_BULLET = 10,
            ELEMENT_ITEM = 11,
            LIFE_ITEM = 12,
        }
       //

        PlayerController _controller;
        public PlayerPresenter(PlayerController pc)
        {
            _controller = pc;
        }

        public void Setup()
        {
            _controller.OnJumpIntend.Subscribe((_) => {
                if (HeroModel.Instance.Mp.Value > 0)
                {
                    HeroModel.Instance.WasteJump();
                    _controller.DoJumpAction();
                }
            }).AddTo(_controller);

            _controller.OnFlyIntend.Subscribe((_) => {
                if (HeroModel.Instance.Mp.Value > 0)
                {
                    HeroModel.Instance.WasteStartFly();
                    _controller.DoStartFlyAction();
                }
            }).AddTo(_controller);

            _controller.GetPlayerGun.OnShootIntend.Subscribe((_) => {
                if (HeroModel.Instance.CanShoot())
                {
                    HeroModel.Instance.WasteShoot();
                    _controller.DoShootAction().Forget();
                }
            }).AddTo(_controller);

            HeroModel.Instance.IS_GAME_OVER.Subscribe(value => {
                if (value)
                {
                    //todo dead show
                    _controller.gameObject.SetActive(false);
                }

            }).AddTo(_controller);


            HeroModel.Instance.Mp.Subscribe(value => {
                if (value <= 0)
                    _controller.LockMpAction(true);
            }).AddTo(_controller);

            HeroModel.Instance.HpChangeSubject.Subscribe(value => {
                if(value < 0)
                    _controller.DoOnHitAction().Forget();
            }).AddTo(_controller);

            _controller.UpdateAsObservable().Subscribe((_) => {
                switch (_controller.GetActState())
                {
                    case PlayerController.ACTION_STATE.ONLAND:
                        if (HeroModel.Instance.RecvMpOnLand(Time.deltaTime))
                            _controller.LockMpAction(false);
                        break;
                    case PlayerController.ACTION_STATE.FLYING:
                        {
                            if (!HeroModel.Instance.WasteMpFlying(Time.deltaTime))
                                _controller.DoStopFlyAction();
                        }
                        break;
                    default:
                        break;
                }
            }).AddTo(_controller);

            _controller.WhenTriggerEnter.Subscribe((go) => {
                switch (go.gameObject.layer)
                { 
                    case (int)GK_LAYER.ENEMY:
                        HeroModel.Instance.ChangeHp(-10);
                        break;
                    case (int)GK_LAYER.ENEMY_BULLET:
                        HeroModel.Instance.ChangeHp(-20);
                        break;
                    case (int)GK_LAYER.ELEMENT_ITEM:
                        {
                            var eleItem = go.GetComponent<ElementCoin>();
                            eleItem.DestoryPoolObj();
                            EasyAudioManager.Instance.CoinEffect.PlayFeedbacks();
                            HeroModel.Instance.RecorverElmMp(eleItem.ELETYPE, 10);
                        }
                        break;
                    case (int)GK_LAYER.LIFE_ITEM:
                        {
                            var lifeItem = go.GetComponent<LifeItem>();
                            lifeItem.DestoryPoolObj();
                            HeroModel.Instance.ChangeHp(UnityEngine.Random.Range(5, 16));
                        }
                        break;
                    default:
                        break;
                }
            });

            _controller.WhenTriggerExit.Subscribe((go) => {
                switch (go.gameObject.layer)
                {
                    case (int)GK_LAYER.FIELD_BOX:
                        HeroModel.Instance.ChangeHp(-9999);
                        break;
                    default:
                        break;
                }
            });
            // const float dltTime = 0.02f;
            //Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(dltTime))
            //     .Subscribe(x =>
            //     {
            //         if(_controller.Grounded)
            //             HeroModel.Instance.RecvMp(dltTime);
            //     }
            //     ).AddTo(_controller);


            //// OnDestoryを受けてログに出す
            //_controller.OnDestroyAsObservable()
            //    .Subscribe(_ => Debug.Log("Destroy!")).AddTo(_controller);
        }

    }
}