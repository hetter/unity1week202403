using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DummyEgg.ProjectGK.Battle
{
	public class PoolObject : MonoBehaviour
	{
		protected bool _isSetup = false;
		protected System.IDisposable _dispLifeDead;

		public float LifeTime = 0f;

		public Action OnPoolObjDestory;

		protected virtual void Update()
		{
			if (!_isSetup)
				_DoSetup();
		}

		public virtual void DestoryPoolObj()
		{
			if (!_isSetup)
				return;
			this.gameObject.SetActive(false);
			_isSetup = false;
			if (_dispLifeDead != null)
			{
				_dispLifeDead.Dispose();
				_dispLifeDead = null;
			}
			OnPoolObjDestory?.Invoke();
		}

		protected virtual void _DoSetup()
		{
			_isSetup = true;

			if (LifeTime >= 0)
			{
				_dispLifeDead = TimeManager.Instance.RunFixedUpdateAct(() => {
					//Debug.Log("-----------------DestoryPoolObj:" + this.name + ",id:" + this.gameObject.GetInstanceID());
					DestoryPoolObj();
				}, this.gameObject, -1, LifeTime);
			}
		}
	}
}