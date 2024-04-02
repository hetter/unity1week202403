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
        public int DefaultPoolCount = 10; // �v�[���̃f�t�H���g�e��
        public int MaxPoolCount = 20;     // �v�[���̍ő�e��


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
                                var flyobj = GameObject.Instantiate(fb, this.transform);         // �v�[������̂Ƃ��ɐV�����C���X�^���X�𐶐����鏈��
                            return flyobj;
                            },
                            actionOnGet: target => target.gameObject.SetActive(true),                            // �v�[��������o���ꂽ�Ƃ��̏��� 
                            actionOnRelease: target => target.gameObject.SetActive(false),                       // �v�[���ɖ߂����Ƃ��̏���
                            actionOnDestroy: target => Destroy(target),                               // �v�[����maxSize�𒴂����Ƃ��̏���
                            collectionCheck: true,                                                    // ����C���X�^���X���o�^����Ă��Ȃ����`�F�b�N���邩�ǂ���
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
                // �L�����Z�����ɌĂ΂���O  
                Debug.Log($"FlyObjShooter  {gameObject.name} �L�����Z��......");
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

            //// �v�[���ɖ߂�
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