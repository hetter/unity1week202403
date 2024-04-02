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

    public class FlyObjShooter : MonoBehaviour
    {
        public int DefaultPoolCount = 10; // プールのデフォルト容量
        public int MaxPoolCount = 20;     // プールの最大容量


        public FlyObject[] FlyObjPrefabs;
        List<ObjectPool<FlyObject>> _flyPools = new();

        public GameObject ActiveField;

        public float ShootCdTime;
        public float ShootStartTime;
        public float ShootWaveCDTime;

        public Vector3 Direction = Vector3.left;

        public int ShootTimesOneWave = 4;

        public bool IsTargetPlayer = false;

        CancellationTokenSource _thisCts = new CancellationTokenSource();

        // Start is called before the first frame update
        void OnEnable()
        {
            if (_flyPools.Count == 0)
            {
                foreach (var fb in FlyObjPrefabs)
                {
                    _flyPools.Add(new ObjectPool<FlyObject>(
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
            }
            Setup().Forget();
        }


        //how to stop?
        async UniTask _setupOneWaveShoot()
        {
            try
            {
                while (true)
                {
                    if (!gameObject.activeSelf)
                        break;
                    int thisWavePoolInx = UnityEngine.Random.Range(0, FlyObjPrefabs.Length);
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
                            SpawnFlyObjects(thisWavePoolInx);
                            doTimes++;
                        }
                    }
                    ).AddTo(this);

                    await UniTask.WaitUntil(() => { return doTimes == ShootTimesOneWave; }, PlayerLoopTiming.Update, _thisCts.Token);
                    waveShootDisp.Dispose();
                    await UniTask.Delay(TimeSpan.FromSeconds(ShootWaveCDTime), false, PlayerLoopTiming.Update, _thisCts.Token);
                }
            }
            catch (OperationCanceledException e)
            {
                // キャンセル時に呼ばれる例外  
                Debug.Log($"FlyObjShooter  {gameObject.name} キャンセル......");
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

            await UniTask.Delay(TimeSpan.FromSeconds(ShootStartTime), false, PlayerLoopTiming.Update, _thisCts.Token);
            _setupOneWaveShoot().Forget();
        }

        public virtual FlyObject SpawnFlyObjects(int inx)
        {
            var nextObj = _flyPools[inx].Get();
            nextObj.gameObject.SetActive(true);
            if (ActiveField == null)
                ActiveField = GameObject.FindGameObjectWithTag("FlyObjField");
            nextObj.transform.SetParent(ActiveField.transform);
            nextObj.transform.position = this.transform.position;

            if (IsTargetPlayer)
                nextObj.Direction = (PlayerController.Instance.transform.position - this.transform.position).normalized;
            else
                nextObj.Direction = Direction;

            nextObj.OnPoolObjDestory = () => {
                //Debug.Log($"{nextObj.name} is return to pool!");
                _flyPools[inx].Release(nextObj);
            };

            //// プールに戻す
            //_bulletPool.Release(nextGameObject);

            // mandatory checks
            if (nextObj == null)
            {
                Debug.LogWarning($"FlyObjPool pool [{this.name}] is bomb!!!!!!:");
                return null;
            }
            return nextObj;
        }
    }
}