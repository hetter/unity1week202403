using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DummyEgg.ProjectGK
{
    public class TimeManager : Singleton<TimeManager>
    {
        public class TimeActData : IDisposable
        {
            private static int AddId = 0;

            private Action _updateCall;

            TimeManager _manger;

            public TimeActData(TimeManager manager, Action updateCall, float pIntervert, float pStartTime, int pTakeTimes, bool isUpdate)
            {
                _id = ++AddId;
                _manger = manager;
                _updateCall = updateCall;
                _startTime = pStartTime;
                _takeTime = pTakeTimes;
                _intervert = pIntervert;
                _isWaitStart = (_startTime > 0);
                _isUpdateMode = isUpdate;
            }

            private float _runCalcTime = 0;
            public bool DoUpdate()
            {
                var deltaTime = _isUpdateMode ? _manger.DeltaTime : _manger.FixedDeltaTime;

                //need calc
                if (_isWaitStart || Intervert > 0)
                {
                    _runCalcTime += deltaTime;
                }
                
                if (_isWaitStart)
                {
                    if (_runCalcTime >= StartTime)
                    {
                        _isWaitStart = false;
                        _runCalcTime -= StartTime;
                    }
                }

                if (!_isWaitStart)
                {
                    if (Intervert < 0)
                    {
                        _updateCall?.Invoke();
                        if (_takeTime > 0)
                        {
                            --_takeTime;
                            if (TakeTime == 0)
                                return false;
                        }
                    }
                    else if (_runCalcTime >= Intervert)
                    {
                        var invocationCount = (int)(_runCalcTime / Intervert);
                        _runCalcTime %= Intervert;
                        for (var i = 0; i < invocationCount; i++)
                        {
                            _updateCall?.Invoke();

                            if (_takeTime > 0)
                            {
                                --_takeTime;
                                if (TakeTime == 0)
                                    return false;
                            }
                        }
                    }
                }
                return true;    
            }

            public void Dispose()
            {
                if(_isUpdateMode)
                    _manger.RemoveUpdateAct(this.ID);
                else
                    _manger.RemoveFixedUpdateAct(this.ID);
            }

            public int ID { get => _id;}
            private int _id = 0;
            public float StartTime { get => _startTime; }
            private float _startTime = 0;

            public int TakeTime { get => _takeTime; }
            public int _takeTime = -1;

            public float Intervert { get => _intervert; }
            private float _intervert = -1;
            private bool _isWaitStart;
            private bool _isUpdateMode; //update or fixedUpdate
        }

        public void RemoveUpdateAct(int id)
        {
            _removeUpdateTimeActs.Add(id);
        }

        public void RemoveFixedUpdateAct(int id)
        {
            _removeFixedUpdateTimeActs.Add(id);
        }
        public HashSet<TimeActData> _readyUpdateTimeActs = new();
        public Dictionary<int, TimeActData> _updateTimeActs = new();
        public HashSet<TimeActData> _readyFixedUpdateTimeActs = new();
        public Dictionary<int, TimeActData> _fixedUpdateTimeActs = new();

        public IObservable<Unit> UpdateAsObservable => _updateSubject.AsObservable();
        public IObservable<Unit> FixedUpdateAsObservable => _fixedUpdateSubject.AsObservable();

        private Subject<Unit> _updateSubject = new();
        private Subject<Unit> _fixedUpdateSubject = new();

        private float _timeScale = 1.0f;

        // ŽžŠÔŒo‰ß‘¬“x‚Ì”{—¦
        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = Mathf.Max(value, 0.0f);
        }

        private float _deltaTime;
        private float _fixedDeltaTime;
        public float DeltaTime
        {
            get => _deltaTime;
        }

        public float FixedDeltaTime
        {
            get => _fixedDeltaTime;
        }

        public IDisposable RunUpdateAct(Action act, GameObject attachGo = null, float pIntervert = -1, float pStartTime = 0, int pTakeTimes = -1)
        {
            TimeActData ta = new(this, act, pIntervert, pStartTime, pTakeTimes, true);
            if(attachGo != null)
                ta.AddTo(attachGo);
            _readyUpdateTimeActs.Add(ta);
            return ta;
        }

        public IDisposable RunFixedUpdateAct(Action act, GameObject attachGo = null, float pIntervert = -1, float pStartTime = 0, int pTakeTimes = -1)
        {
            TimeActData ta = new(this, act, pIntervert, pStartTime, pTakeTimes, false);
            if (attachGo != null)
                ta.AddTo(attachGo);
            _readyFixedUpdateTimeActs.Add(ta);
            return ta;
        }

        HashSet<int> _removeUpdateTimeActs = new();
        void Update()
        {
            _deltaTime = Time.deltaTime * _timeScale;
            _deltaTime *= _timeScale;

            foreach (var _ in _updateTimeActs)
            {
                var ta = _.Value;
                if (_removeUpdateTimeActs.Contains(ta.ID))
                    continue;
                if (!ta.DoUpdate())
                    _removeUpdateTimeActs.Add(ta.ID);
            }

            _updateSubject.OnNext(Unit.Default);
        }

        private void LateUpdate()
        {
            foreach (var rk in _removeUpdateTimeActs)
            {
                _updateTimeActs.Remove(rk);
            }
            _removeUpdateTimeActs.Clear();

            foreach (var _ in _readyUpdateTimeActs)
            {
                _updateTimeActs.Add(_.ID, _);
            }
            _readyUpdateTimeActs.Clear();
        }

        HashSet<int> _removeFixedUpdateTimeActs = new();
        void FixedUpdate()
        {
            _fixedDeltaTime = Time.fixedDeltaTime * _timeScale;
            _fixedDeltaTime *= _timeScale;

            foreach (var _ in _fixedUpdateTimeActs)
            {
                var ta = _.Value;
                if (_removeFixedUpdateTimeActs.Contains(ta.ID))
                    continue;
                if (!ta.DoUpdate())
                    _removeFixedUpdateTimeActs.Add(ta.ID);
            }

            _fixedUpdateSubject.OnNext(Unit.Default);

            LateFixedUpdate();
        }

        private void LateFixedUpdate()
        {
            foreach (var rk in _removeFixedUpdateTimeActs)
                _fixedUpdateTimeActs.Remove(rk);
            _removeFixedUpdateTimeActs.Clear();

            foreach (var _ in _readyFixedUpdateTimeActs)
                _fixedUpdateTimeActs.Add(_.ID, _);
            _readyFixedUpdateTimeActs.Clear();
        }

        private void OnDestroy()
        {
            _updateSubject.Dispose();
            _fixedUpdateSubject.Dispose();
        }

        public async UniTask Delay(float ms, CancellationToken cancellationToken = default)
        {
            TimeActData ta = new(this, null, -1, ms, 1, true);
            _readyUpdateTimeActs.Add(ta);
            await UniTask.WaitUntil(() => { return ta.TakeTime == 0; }, PlayerLoopTiming.Update, cancellationToken);
        }
    }
}