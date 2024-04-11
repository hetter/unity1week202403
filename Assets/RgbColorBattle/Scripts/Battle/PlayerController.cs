using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UniRx;
using Cysharp.Threading.Tasks;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace DummyEgg.ProjectGK.Battle
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : Singleton<PlayerController>
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;



        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Space(10)]
        [Tooltip("fly speed")]
        public float flySpdY = 3.0f;

        public Subject<Unit> OnJumpIntend = new();

        public Subject<Unit> OnFlyIntend = new();

        public Subject<Unit> DoFlying = new();

        public Subject<GameObject> WhenTriggerEnter = new();

        public Subject<GameObject> WhenTriggerExit = new();

        public enum ACTION_STATE
        {
            ONLAND,
            IN_AIR,
            FLYING
        }

        ACTION_STATE _nowActState = ACTION_STATE.IN_AIR;
        public ACTION_STATE GetActState() { return _nowActState; }
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDShoot;
        private int _animIDHit;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;

        private RgbMainPlayInput _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;


        public void SetAnimatorSpeed(float v)
        {
            if (_animator != null)
                _animator.speed = v;
        }

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return true;
#else
				return false;
#endif
            }
        }

        PlayerGun _playerGun;
        public PlayerGun GetPlayerGun
        {
            get
            {
                if(_playerGun == null)
                    _playerGun = GetComponent<PlayerGun>();
                return _playerGun;
            }
        }

        //private void Awake()
        //{
        //    // get a reference to our main camera
        //    if (_mainCamera == null)
        //    {
        //        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //    }
        //}

        PlayerPresenter _presenter;

        private void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            // _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            if (_animator == null)
                _hasAnimator = TryGetComponent(out _animator);
            else
                _hasAnimator = true;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<RgbMainPlayInput>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _presenter = new(this);
            _presenter.Setup();


        }

        private void OnTriggerEnter(Collider other)
        {
            WhenTriggerEnter.OnNext(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            WhenTriggerExit.OnNext(other.gameObject);
            //Debug.Log("otherrrrr:" + other.gameObject.layer);
        }

        private void Update()
        {
            if (Model.HeroModel.Instance.IS_PAUSE.Value)
            {
                return;
            }

            if (_animator == null)
                _hasAnimator = TryGetComponent(out _animator);
            else
                _hasAnimator = true;

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDShoot = Animator.StringToHash("Shoot");
            _animIDHit = Animator.StringToHash("Damage");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }

           // if (Grounded)
           //     _nowActState = ACTION_STATE.ONLAND;
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = MoveSpeed;

            //FLYING減速
            if (ACTION_STATE.FLYING == _nowActState)
                targetSpeed = flySpdY;// flySpdY / 10.0f;
            //

            //Mp0挙動
            if (_isLockMpAction)
                targetSpeed = 0;
            //

                // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is no input, set the target speed to 0
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    TimeManager.Instance.DeltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

           
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, TimeManager.Instance.DeltaTime * SpeedChangeRate);
            float setAnimationBlend = _animationBlend;
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            else if (_animationBlend >= 5f) setAnimationBlend = 5f;
            else if (_animationBlend >= 1f) setAnimationBlend = 1f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _lastMoveFlag = _controller.Move(targetDirection.normalized * (_speed * TimeManager.Instance.DeltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * TimeManager.Instance.DeltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, setAnimationBlend);// _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        //todo behaviour tree?
        public int checkss = 0;
        private CollisionFlags _lastMoveFlag;

        public async UniTask DoShootAction()
        {
            EasyAudioManager.Instance.ShootEffect.PlayFeedbacks();
            _animator.SetBool(_animIDShoot, true);
            GetPlayerGun.SpawnBullets();
            await UniTask.DelayFrame(1);
            _animator.SetBool(_animIDShoot, false);
        }

        public async UniTask DoOnHitAction()
        {
            _animator.SetBool(_animIDHit, true);
            await UniTask.DelayFrame(1);
            _animator.SetBool(_animIDHit, false);
        }

        public void DoJumpAction()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, true);
            }

            _nowActState = ACTION_STATE.IN_AIR;
        }

        bool _isLockMpAction = false;
        public void LockMpAction(bool v)
        {
            _isLockMpAction = v;
        }

        public void DoStartFlyAction()
        {
            //Debug.Log("-------------DoStartFlyAction ssssssssssssssss");
            _nowActState = ACTION_STATE.FLYING;
            _verticalVelocity = flySpdY;
        }

        public void DoStopFlyAction()
        {
            //Debug.Log("-------------change FLYING enddddddd");
            if (Grounded)
                _nowActState = ACTION_STATE.ONLAND;
            else
                _nowActState = ACTION_STATE.IN_AIR;
        }


        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }


                // Jump指令実行される瞬間、_verticalVelocityの数値は正数になる
                if (_input.Jump && _jumpTimeoutDelta <= 0.0f && _verticalVelocity <= 0)
                {
                    //Debug.Log("intend to jump!!!!");
                    OnJumpIntend.OnNext(Unit.Default);
                   //DoJumpAction()
                }
                else if(!_input.Jump && _verticalVelocity < 0)
                {
                    _nowActState = ACTION_STATE.ONLAND;
                }
                
                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= TimeManager.Instance.DeltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= TimeManager.Instance.DeltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                //_input.Jump = false;
            }

            switch (_nowActState)
            {
                case ACTION_STATE.IN_AIR:
                    {
                        //when head hit wall
                        if (_lastMoveFlag == CollisionFlags.Above)
                        {
                            if(_verticalVelocity > 0)
                                _verticalVelocity = Gravity * TimeManager.Instance.DeltaTime;
                            _verticalVelocity += Gravity * TimeManager.Instance.DeltaTime;
                        }
                        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                        else if (_verticalVelocity < _terminalVelocity )
                        {
                            _verticalVelocity += Gravity * TimeManager.Instance.DeltaTime;

                            if (_verticalVelocity < 0 && _input.Jump && !Grounded)
                            {
                                OnFlyIntend.OnNext(Unit.Default);
                                //doFly
                            }
                        }
                    }
                    break;
                case ACTION_STATE.FLYING:
                    {
                        if (!_input.Jump)
                        {
                            DoStopFlyAction();
                        }
                    }
                    break;
                case ACTION_STATE.ONLAND:
                    {
                        if (!Grounded)
                            _nowActState = ACTION_STATE.IN_AIR;
                    }
                    break;
                default:
                    break;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}