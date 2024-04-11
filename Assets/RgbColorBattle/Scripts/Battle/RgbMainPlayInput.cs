using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DummyEgg.ProjectGK.Battle
{
    public class RgbMainPlayInput : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 Move;

        public bool Jump;

        public ReactiveProperty<bool> Fire = new();

        //public ReactiveProperty<int> ElementId = new();

        public void OnMove(InputValue value)
        {
            Vector2 newMoveDirection = value.Get<Vector2>();
            newMoveDirection = new Vector2(newMoveDirection.x, 0);
            Move = newMoveDirection;
        }

        public void OnJump(InputValue value)
        {
            //Debug.Log("debug jumpppppppppppppp!!!!!!:" + value.isPressed);
            Jump = value.isPressed;
            //JumpInput(value.isPressed);
        }

        public void OnFire(InputValue value)
        {
            //Debug.Log("debug attackkkkkkkkkkkkkkk!!!!!!" + value.isPressed);
            Fire.Value = value.isPressed;
        }

        public void OnPause(InputValue value)
        {

            Model.HeroModel.Instance.IS_PAUSE.Value = !Model.HeroModel.Instance.IS_PAUSE.Value;
            //Debug.Log("debug OnPauseOnPauseOnPauseOnPauseOnPause!!!!!!" + IsPause.Value);
        }

        public void OnChangeRed(InputValue value)
        {
            //Debug.Log("debug attackkkkkkkkkkkkkkk!!!!!!");

            // ElementId.Value = 1;
            //todo model分離
            if(!Model.HeroModel.Instance.IS_PAUSE.Value)
                Model.HeroModel.Instance.NOW_ELETYPE.Value = Model.HeroModel.ELE_TYPE.RED;
        }

        public void OnChangeGreen(InputValue value)
        {
            //Debug.Log("debug attackkkkkkkkkkkkkkk!!!!!!");
            // ElementId.Value = 2;
            //todo model分離
            if (!Model.HeroModel.Instance.IS_PAUSE.Value)
                Model.HeroModel.Instance.NOW_ELETYPE.Value = Model.HeroModel.ELE_TYPE.GREEN;
        }

        public void OnChangeBlue(InputValue value)
        {
            //Debug.Log("debug attackkkkkkkkkkkkkkk!!!!!!");
            // ElementId.Value = 3;
            //todo model分離
            if (!Model.HeroModel.Instance.IS_PAUSE.Value)
                Model.HeroModel.Instance.NOW_ELETYPE.Value = Model.HeroModel.ELE_TYPE.BLUE;
        }
    }
}