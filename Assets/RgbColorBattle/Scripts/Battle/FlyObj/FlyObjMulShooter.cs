using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

namespace DummyEgg.ProjectGK.Battle
{

    public class FlyObjMulShooter : MonoBehaviour
    {
        public int DefaultPoolCount = 2; // プールのデフォルト容量
        public int MaxPoolCount = 3;     // プールの最大容量


        public PoolObject[] PoolObjObjPrefabs;
        public Transform[]  ShootPosition; //must same count withPoolObjObjPrefabs
        List<ObjectPool<PoolObject>> _ItemPools = new();

        public GameObject ActiveField;

        public float ShootCdTime;
        public float ShootStartTime;

        public float WaveCDTimeRandomMin = 2;
        public float WaveCDTimeRandomMax = 3;
        private float _ShootWaveCDTime;

        public int ShootTimesOneWave = 1;

        CancellationTokenSource _thisCts = new CancellationTokenSource();

        // Start is called before the first frame update
        void OnEnable()
        {
            if (_ItemPools.Count == 0)
            {
                foreach (var fb in PoolObjObjPrefabs)
                    _ItemPools.Add(new(
                                createFunc: () =>
                                {
                                    var flyobj = GameObject.Instantiate(fb, this.transform);         // プールが空のときに新しいインスタンスを生成する処理
                                return flyobj;
                                },
                                actionOnGet: target => target.gameObject.SetActive(true),                            // プールから取り出されたときの処理 
                                actionOnRelease: target => target.gameObject.SetActive(false),                       // プールに戻したときの処理
                                actionOnDestroy: target => Destroy(target),                               // プールがmaxSizeを超えたときの処理
                                collectionCheck: true,                                                    // 同一インスタンスが登録されていないかチェックするかどうか
                                defaultCapacity: DefaultPoolCount,
                                maxSize: MaxPoolCount));
            }
            Setup().Forget();
        }


        async UniTask _setupOneWaveShoot()
        {
            try
            {
                while (true)
                {
                    if (!gameObject.activeSelf)
                        break;
                    _ShootWaveCDTime = UnityEngine.Random.Range(WaveCDTimeRandomMin, WaveCDTimeRandomMax);
                    int doTimes = 0;
                    IDisposable waveShootDisp = null;
                    waveShootDisp = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(ShootCdTime)).Take(ShootTimesOneWave)
                        .Subscribe(x =>
                        {
                            if (!gameObject.activeSelf)
                            {
                                doTimes = ShootTimesOneWave;
                                waveShootDisp.Dispose();
                            }
                            else
                            {
                                var inx = UnityEngine.Random.Range(0, _ItemPools.Count);
                                SpawnFlyObjects(inx);
                                doTimes++;
                            }
                        }
                        ).AddTo(this);

                    await UniTask.WaitUntil(() => { return doTimes == ShootTimesOneWave; }, PlayerLoopTiming.Update, _thisCts.Token);
                    waveShootDisp.Dispose();
                    await UniTask.Delay(TimeSpan.FromSeconds(_ShootWaveCDTime), false, PlayerLoopTiming.Update, _thisCts.Token);
                }
            }
            catch (OperationCanceledException e)
            {
                // キャンセル時に呼ばれる例外  
                Debug.Log($"FlyObjMulShooter {gameObject.name} キャンセル......");
            }
        }

        public virtual async UniTask Setup()
        {
            Model.HeroModel.Instance.IS_GAME_OVER.Subscribe(value => {
                if (value)
                {
                    //todo dead show
                    this.gameObject.SetActive(false);
                    _thisCts.Cancel();
                }
            }).AddTo(this);

            await UniTask.Delay(TimeSpan.FromSeconds(ShootStartTime));
            _setupOneWaveShoot().Forget();
        }

        public virtual PoolObject SpawnFlyObjects(int inx)
        {
            if(inx >= _ItemPools.Count)
            {
                Debug.LogWarning("----------------SpawnFlyObjects inx over:" + this.name);
                return null;
            }
            var nextObj = _ItemPools[inx].Get();
            if (ActiveField == null)
                ActiveField = GameObject.FindGameObjectWithTag("FlyObjField");
            nextObj.transform.SetParent(ActiveField.transform);
            nextObj.transform.position = ShootPosition[inx].position;//this.transform.position;
            nextObj.gameObject.SetActive(true);


            nextObj.OnPoolObjDestory = () => {
                //Debug.Log($"{nextObj.name} is return to pool!");
                _ItemPools[inx].Release(nextObj);
            };

            //// プールに戻す
            //_bulletPool.Release(nextGameObject);

            // mandatory checks
            if (nextObj == null)
            {
                Debug.LogWarning($"_LifeItemPool pool [{this.name}] is bomb!!!!!!:");
                return null;
            }
            return nextObj;
        }
    }
}