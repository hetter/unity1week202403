using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DummyEgg.ProjectGK.Battle
{
	public class RgbMulLineMove : MonoBehaviour
	{
		public Space MovementSpace = Space.World;

		public Vector3 Movement { get { return _movement; } }

		public float Acceleration = 0;

		protected Vector3 _movement;

		public float Speed = 0;

		protected float _initialSpeed;

		public Vector3[] TargetPos;

		private int _nowMoveInx = 0;
		private bool _isEnd;
		private Vector3 _nowDrection;
		private Vector3 _nowTargetPosition;

		[SerializeField] bool _bDoRandomPosition;
		[SerializeField] Vector3 _randomMinV3;
		[SerializeField] Vector3 _randomMaxV3;

		public bool isLoop;


		protected virtual void Awake()
		{
			_initialSpeed = Speed;
		}

		protected virtual void OnEnable()
		{
			Speed = _initialSpeed;
			_isEnd = false;
			_nowMoveInx = 0;
			_nowDrection = Vector3.zero;
		}

		private void _setupToOnePoint()
		{
			var targetPosition = TargetPos[_nowMoveInx];

			if (MovementSpace == Space.Self)
				_nowTargetPosition = targetPosition + transform.position;
			else
				_nowTargetPosition = targetPosition;

			if (_bDoRandomPosition)
			{
				Vector3 rdv3 = new(UnityEngine.Random.Range(_randomMinV3.x, _randomMaxV3.x),
					UnityEngine.Random.Range(_randomMinV3.y, _randomMaxV3.y),
					UnityEngine.Random.Range(_randomMinV3.z, _randomMaxV3.z));
				_nowTargetPosition += rdv3;
			}

			_nowDrection = (_nowTargetPosition - transform.position).normalized;
		}

		public virtual void Move(float dtime)
		{
			if (TargetPos == null)
				return;

			if (_nowDrection == Vector3.zero)
				_setupToOnePoint();

			transform.position = Vector3.MoveTowards(transform.position, _nowTargetPosition, Speed * dtime);

			//_movement = _nowDrection * Speed * dtime;
			//transform.Translate(_movement, MovementSpace);

			Speed += Acceleration * dtime;

			if (Vector3.Distance(transform.position, _nowTargetPosition) < 0.05f)
			{
				_nowDrection = Vector3.zero;
				transform.position = _nowTargetPosition;
				Speed = _initialSpeed;
				++_nowMoveInx;
				if (_nowMoveInx >= TargetPos.Length)
				{
					if (isLoop && TargetPos.Length > 2)
						_nowMoveInx = 1;
					else
						_isEnd = true;
				}
			}

		}

		protected virtual void FixedUpdate()
		{
			if (_isEnd)
				return;
			//Move(Time.fixedDeltaTime);
			Move(TimeManager.Instance.FixedDeltaTime);
		}
	}
}