using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx.Triggers;

namespace DummyEgg.ProjectGK.Battle
{
    public class PlayerGun : MonoBehaviour
    {
        const int DefaultPoolCount = 5; // プールのデフォルト容量
        const int MaxPoolCount = 10;     // プールの最大容量
        public BaseBullet[] BulletPrefab;
        List<ObjectPool<BaseBullet>> _bulletPools = new();

        private RgbMainPlayInput _input;

       // private IDisposable _shootingDisp = null;


       
        private Vector3 _inputDirection = Vector2.right;


        int _delayBetweenUsesCounter = 200; //発射間隔(millisecond)
        bool _isWaitShootCd = false;

        public Subject<Unit> OnShootIntend = new();

        void Start()
        {
            foreach(var bPrefab in BulletPrefab)
            {
                var bPool = new ObjectPool<BaseBullet>(
                        createFunc: () =>
                        {
                             var bullet = GameObject.Instantiate(bPrefab, this.transform);         // プールが空のときに新しいインスタンスを生成する処理
                             return bullet;
                        },
                        actionOnGet: target => target.gameObject.SetActive(true),                            // プールから取り出されたときの処理 
                        actionOnRelease: target => target.gameObject.SetActive(false),                       // プールに戻したときの処理
                        actionOnDestroy: target => Destroy(target),                               // プールがmaxSizeを超えたときの処理
                        collectionCheck: true,                                                    // 同一インスタンスが登録されていないかチェックするかどうか
                        defaultCapacity: DefaultPoolCount,
                        maxSize: MaxPoolCount);

                _bulletPools.Add(bPool);
            }

            _input = GetComponent<RgbMainPlayInput>();

            _input.Fire.Subscribe(value => {
                if (value && !_isWaitShootCd)
                {
                    _FireShootEvent().Forget();
                }
            }).AddTo(this);
        }

        private async UniTask _FireShootEvent()
        {
            OnShootIntend.OnNext(Unit.Default);
            _isWaitShootCd = true;
            //Debug.Log("wait start.......");
            //await UniTask.Delay(_delayBetweenUsesCounter);
            await TimeManager.Instance.Delay(_delayBetweenUsesCounter / 1000.0f);
            _isWaitShootCd = false;
            //Debug.Log("wait end.......");
        }

        public virtual BaseBullet SpawnBullets()
        {
            //todo model分離
            var nowInx = (int)(Model.HeroModel.Instance.NOW_ELETYPE.Value);
            var nowPool = _bulletPools[nowInx];

            var nextBullet = nowPool.Get();
            nextBullet.gameObject.SetActive(true);

            nextBullet.transform.SetParent(this.transform.parent);
            nextBullet.transform.position = this.transform.position;
            nextBullet.LifeTime = 2.0f;
            nextBullet.Direction = _getGunDirection();

            nextBullet.OnPoolObjDestory = ()=>{
                //// プールに戻す
                nowPool.Release(nextBullet);
            };

            // mandatory checks
            if (nextBullet == null)
            {
                Debug.LogWarning($"bulletPool pool [{this.name}] is bomb!!!!!!:");
                return null;
            }
            return nextBullet;
        }

        private Vector3 _getGunDirection()
        {
            return _inputDirection;
        }

        public void FixedUpdate()
        {
            if(_input.Move != Vector2.zero)
                _inputDirection = new Vector3(_input.Move.x, 0.0f, 0.0f).normalized;//_input.Move.y).normalized;
        }
    }
}