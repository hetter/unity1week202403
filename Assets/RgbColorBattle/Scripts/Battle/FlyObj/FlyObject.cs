using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DummyEgg.ProjectGK.Battle
{
	public class FlyObject : PoolObject
	{
		public float Speed = 200;

		public float Acceleration = 0;

		public Vector3 Direction = Vector3.right;

		public LayerMask SpawnSecurityCheckLayerMask;

		protected Vector3 _movement;



		protected override void Update()
		{
			base.Update();

			var dtime = TimeManager.Instance.DeltaTime; //Time.deltaTime;
			_movement = Direction * (Speed / 10) * dtime;
			transform.Translate(_movement, Space.World);
			// We apply the acceleration to increase the speed
			Speed += Acceleration * dtime;
		}

		protected override void _DoSetup()
		{
			base._DoSetup();

			var ang = -Vector3.Angle(Direction, Vector3.right);
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, ang);
		}
	}
}