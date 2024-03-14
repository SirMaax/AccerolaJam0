 using System;
 using System.Collections;
 using System.Collections.Generic;
 using Unity.Mathematics;
 using UnityEngine;
 using UnityEngine.Animations;
 using UnityEngine.PlayerLoop;
 using UnityEngine.Timeline;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Test")] [SerializeField] private float test;
        
        [Header("Settings")] 
        [SerializeField] private bool playerIsAlwaysSprinting;
        [SerializeField] private bool canUseDoubleJump = false;
        [SerializeField] private bool canJumpHigherByHolding;
        [SerializeField] private bool canHoldDownSpaceForJumping;
        [SerializeField] bool printOutJumpCombo;


        [Header("Player")] 
        public float velocity;
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        public Vector3 velocityVector;
        private Vector3 lastPosition;
        private float timeStartedMovement;
        
        
        [Header("AccelSpeed")]
        [SerializeField] private AnimationCurve MovementSpeed;
        [SerializeField] public  float requiredAccelerationSpeed;
        [SerializeField] float currentAccelerationTime = 0;
        private bool hasFullyAccelerated = false;
        
        [Header("Audio")] 
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Jumping")]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.0f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("The additional multiplier speed the player has during the jump is only applied when under a certain speed")]
        [SerializeField] private float[] jumpSpeedMultiplier;
        private Vector3 jumpDirection;
        [SerializeField] private AnimationCurve jumpCurveOverTime;
        [Tooltip("The lerp value that changes the jump direction over time")]
        [SerializeField] private float lerpValueForAirControl;
        [SerializeField] private float jumpBuffer;
        [SerializeField] private float coyoteTime;
        [SerializeField] private float maxJumpButtonHolding;
        [Tooltip("-1 == None/ 0 == Normal Jump/  1 == WallJump/  2 == Double Jump / 3 == Continued Jump / 4 == Other")]
        public int lastJumpType = -1;
        
        //The last time when the player had his initial jump
        private float timeOffInitialJump = 0;
        private bool usedCoyoteTime = false;
        private bool canJump = false;
        private bool startedJump = false;

        [Header("Double Jump")] 
        [SerializeField] private float forceDoubleJump;
        private bool usedDoubleJump = false;
        

        [Header("Jump combo")]
        [SerializeField] private float timingForJumpCombo;
        [SerializeField] private List<float> jumpMultiplier;
        public int currentJumpIndex;
        
       
        [Header("Walljump")] 
        [SerializeField] private float wallJumpHeight;
        [Tooltip("Not really the angle just the distance direction the player is going up")]
        [SerializeField] public float wallJumpAngle;
        [SerializeField] private float overwriteOfNormalMovementPeriod;
        [SerializeField] private float wallJumpMultiplier;
        [HideInInspector] public GameObject touchedWall;
        [Tooltip("Lerp value used for making the walljump direction more like the user input direction")]
        [SerializeField] private float lerpValueWallJump;
        public bool onWall = false;
        [HideInInspector] public bool wallJump;
        [HideInInspector] public Vector3 entryVector;
        private float timeLeftWall;

        [Header("Diving")] 
        [SerializeField] private float divingSpeed;
        [SerializeField] private float diveGravity;
        private bool isDiving;
        private Vector3 divingDirection;

        [Header("BackFlip")] 
        [SerializeField] private float backFlipHeight;
        [SerializeField] private float backFlipDuration;
        [SerializeField] private float backFlipBackwardsMovement;
        [SerializeField] float backFlipRotateSpeed;
        private float startBackFlip;
        private bool isBackFlipping;
        
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        //Is continiously set if playerIsStill touching ground. When leaving like jumping from plattform this will be the time when player last touched ground
        private float lastTimeGrounded;
        //The time when the player ground itself again after leaving. Is only set on the first ground touch
        private float timeSinceGrounded;
        //If the player left the ground by jumping or falling.
        private bool leftGroundByJumping;
        
        [Header("CorruptSettings")] 
        private CorruptAbilities _corruptAbilities;
        
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

        [Header("References")]
        [Tooltip("Graphics of the player")]
        [SerializeField] private GameObject gx;
        
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
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        //Gx
        private Quaternion gxStartRotation;
        
        //sound
        private LocalSoundManager _soundManager; 

        
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        [HideInInspector]public CharacterController _controller;
        protected StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _soundManager = transform.parent.GetComponentInChildren<LocalSoundManager>();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            gxStartRotation = gx.transform.rotation;
            AssignAnimationIDs();
            _corruptAbilities = GetComponent<CorruptAbilities>();
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (_input.isUsingUi) return;
            
            Vector3 speed = _controller.velocity;
            speed.y = 0;
            velocity = speed.magnitude;
            
            velocityVector = transform.position - lastPosition;
            lastPosition = transform.position;
            _hasAnimator = TryGetComponent(out _animator);
            
            canJump = CheckCanJump();
            Dive();
            BackFlip();
            CheckWallJump();
            JumpAndGravity();
            ContinueJump();
            Accleration();
            Move();
        }

        private void FixedUpdate()
        {
            GroundedCheck();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            if (Time.time - timeOffInitialJump < 0.1f) return;
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            bool groundStatusBeforeCheck = Grounded;
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers);
            Collider[] colliders = Physics.OverlapSphere(spherePosition, GroundedRadius, GroundLayers);
            if (colliders.Length > 0) Grounded = true;
            if (Grounded)
            {
                lastTimeGrounded = Time.time;
                if (!groundStatusBeforeCheck) FirstGroundTouch();
            }


            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (isDiving) return;
            if (Grounded) _verticalVelocity = -2f;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            if (_input.movedOppositeDirection)
            {
                _input.movedOppositeDirection = false;
                if(Grounded) currentAccelerationTime = -0.25f;
            }
            // float targetSpeed = _input.sprint || playerIsAlwaysSprinting ? SprintSpeed : MoveSpeed;
            float targetSpeed = MovementSpeed.Evaluate(currentAccelerationTime);
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float localJumpSpeedMultiplier = leftGroundByJumping ? jumpSpeedMultiplier[currentJumpIndex] : 1;
            targetSpeed *= localJumpSpeedMultiplier;
            
            // a reference o the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (Grounded || lastJumpType == -1)
            {
                  if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                      currentHorizontalSpeed > targetSpeed + speedOffset)
                  {
                      float newspeedChangeRate = currentHorizontalSpeed > targetSpeed ? 6 : SpeedChangeRate;
                      // creates curved result rather than a linear one giving a more organic speed change
                      // note T in Lerp is clamped, so we don't need to clamp our speed
                      _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                          Time.deltaTime * newspeedChangeRate);
        
                      // round speed to 3 decimal places
                      _speed = Mathf.Round(_speed * 1000f) / 1000f;
                  }
                  else
                  {
                      _speed = targetSpeed;
                  }
                
            }
            // accelerate or decelerate to target speed

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Reduce control in air
            Vector3 inputDirection= new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                if (!isBackFlipping)
                {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    
                }
            }
            
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if(wallJump)
            {
                //Walljump
                if (_input.move != Vector2.zero)
                {
                    //If jumpDirection = Vector3.zero mehr lerpValue
                    entryVector.x  = Mathf.Lerp(entryVector.x, targetDirection.x, lerpValueWallJump);
                    entryVector.z  = Mathf.Lerp(entryVector.z, targetDirection.z, lerpValueWallJump);
                }
                entryVector.y = 0;
                _controller.Move(entryVector * (SprintSpeed * wallJumpMultiplier * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else if (isBackFlipping)
            {
                Vector3 backFlipDirection = new Vector3(0, 0, -1);
                backFlipDirection = transform.rotation * backFlipDirection;
                _controller.Move(backFlipDirection * backFlipBackwardsMovement * Time.deltaTime +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else if (!Grounded && (lastJumpType == 0 || lastJumpType == 2 ))
            {
                if (!usedDoubleJump || !_corruptAbilities.DoubleJump())
                {
                    if (_input.move != Vector2.zero)
                    {
                        //If jumpDirection = Vector3.zero mehr lerpValue
                        jumpDirection.x = Mathf.Lerp(jumpDirection.x, targetDirection.x, lerpValueForAirControl);
                        jumpDirection.z = Mathf.Lerp(jumpDirection.z, targetDirection.z, lerpValueForAirControl);
                }
                    
                }
                float jumpSpeedMulti = velocity < 6 ? jumpSpeedMultiplier[currentJumpIndex] : 1;
                //For slow jumps 
                if (_speed <= 2 && _input.move != Vector2.zero) _speed = 2;
                _controller.Move(jumpDirection * (_speed * Time.deltaTime * jumpSpeedMulti) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else
            {
                //If last jump was walljump momentum goes in other direction
                if (lastJumpType == 1 && _input.move == Vector2.zero) targetDirection = Vector3.zero;
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                // transform.Translate(outsideMovement);
            }
            if(_input.move != Vector2.zero) _corruptAbilities.CorruptedMovement(currentHorizontalSpeed,Grounded);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void Accleration()
        {
            // _input.move == Vector2.zero ||
            if ( velocity < requiredAccelerationSpeed && hasFullyAccelerated && !onWall )
            {
                // if(!Grounded)currentAccelerationTime -= Time.deltaTime;
                if(Grounded)
                // else
                {
                    currentAccelerationTime = 0;
                    hasFullyAccelerated = false;
                }
            }
            else if (velocity > requiredAccelerationSpeed && !hasFullyAccelerated) hasFullyAccelerated = true;
            else if(Grounded && _input.move !=Vector2.zero) currentAccelerationTime += Time.deltaTime;
        }
        private bool CheckCanJump()
        {
            //Released Jump Button after jumping
            if (!Grounded && startedJump && !_input.jump) return false;
            
            // 
            return canJump;
        }
        
        private void ContinueJump()
        {
            if(CanPlayerJump())
            if(currentJumpIndex >= 1 ) return;
            if (canJumpHigherByHolding && startedJump && _input.jump && Time.time - _input.timeOfLastJump < maxJumpButtonHolding && canJump)
            {
                Jump(jumpType: 2);
            }
            //Double Jump canJump is false after first jump button is released
             else  if (_input.jump && !canJump && !usedDoubleJump && lastJumpType!= -1 && canUseDoubleJump && !onWall && !wallJump && _input.HasReleasedJumpButtonSinceLastJump(lastTimeGrounded))
            {
                Debug.Log("used double jump");
                usedDoubleJump = true;
                Jump(jumpType:2,overwriteJumpCurve:forceDoubleJump);
            }
        }

        private void JumpAndGravity()
        {
            if (!CanPlayerJump()) return;
            if ((_input.jump || Time.time - _input.timeOfLastJump <= jumpBuffer) && onWall 
                && _input.HasReleasedJumpButtonSinceLastJump(lastTimeGrounded) && _input.timeHeldJumpButton < jumpBuffer)
            {
            //Walljump
                onWall = false;
                if (_corruptAbilities.WallJump(touchedWall)) return;
                timeLeftWall = Time.time;
                wallJump = true;
                Jump(true, jumpType: 1);
            }
            else if (Grounded)
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

                // Jump
                if (_input.jump || Time.time - _input.timeOfLastJump <= jumpBuffer &&!startedJump)
                {
                    
                    Jump(initialJump: true,jumpBufferWasUsed:Time.time - _input.timeOfLastJump <= jumpBuffer);
                }
            }
            // Coyote Time
            else if (!Grounded && _input.jump && Time.time - lastTimeGrounded < coyoteTime && !usedCoyoteTime && canJump)
            {
                Jump();
                usedCoyoteTime = true;
            }
            else 
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
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
                // _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        /// <summary>
        /// Handles the actual execution of the jump
        /// </summary>
        /// <param name="wallJump"> If this jump is a walljump. Because different forces are then applied</param>
        /// <param name="jumpType"> Which jump tpye = 0 normal // 1 = walljump // 2 = double jump // 3 = continued jump</param>
        /// <param name="overwriteJumpCurve"> if yes a custom jump value can be given for this jump</param>
        /// <param name="initialJump"> If this is the first jump of the ground. Is used for handling the jump combo</param>
        private void Jump(bool wallJump = false, int jumpType = 0, float overwriteJumpCurve = 0, 
            bool initialJump = false, bool jumpBufferWasUsed = false, bool applyCorrupt = true, bool useJumpHeightMulitplier = true)
        {
            if (_corruptAbilities.CorruptJump() && applyCorrupt) return;
            if (!canHoldDownSpaceForJumping 
                && (!_input.HasPlayerReleasedJumpButtonSinceLastPress(lastTimeGrounded) && !jumpBufferWasUsed)
                && initialJump) return;
            if(initialJump)HandleJumpCombo();
            
            if(jumpType == 0 || jumpType == 1)_soundManager.Play(SoundManager.EAudioClips.jump);
            lastJumpType = jumpType;
            startedJump = true;
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            float currentPosition = 0;
            currentPosition = Mathf.Clamp01(Time.time - _input.timeOfLastJump);
            float currentJumpHeight = jumpCurveOverTime.Evaluate(currentPosition);
            
            float usedJumpHeight = wallJump ? wallJumpHeight : currentJumpHeight;
            usedJumpHeight = overwriteJumpCurve != 0 ? overwriteJumpCurve : usedJumpHeight;
            
            
            float jumpComboMultiplier = jumpMultiplier[currentJumpIndex];
            //Corruption modifier
            jumpComboMultiplier *= currentJumpIndex > 1? _corruptAbilities.JumpCombo(currentJumpIndex) : 1;
            //Jumpcombo multiplier only on normal jump
            jumpComboMultiplier = jumpType == 0 || jumpType == 3 ? jumpComboMultiplier : 1;
            _verticalVelocity = Mathf.Sqrt(usedJumpHeight * jumpComboMultiplier * -2f * Gravity);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, true);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

        }
        /// <summary>
        /// Handles the jump combo aswell as checks which are down after leaving the ground by jumping the first time
        /// </summary>
        private void HandleJumpCombo()
        {
            jumpDirection.y = 0;
            jumpDirection = velocityVector.normalized;
            timeOffInitialJump = Time.time;
            Grounded = false;
            leftGroundByJumping = true;
            //After on complete combo reset
            if (currentJumpIndex == jumpMultiplier.Count - 1)
            {
                currentJumpIndex = 0;
                return;
            }
            float usedTime = _input.timeOfLastJump;
            if (usedTime < timeSinceGrounded + timingForJumpCombo && usedTime > timeSinceGrounded - timingForJumpCombo)
            {
                //Player got right timing
                currentJumpIndex += 1;
                currentJumpIndex = Math.Clamp(currentJumpIndex, 0, jumpMultiplier.Count - 1);
                if(printOutJumpCombo)Debug.Log("succeded to " + currentJumpIndex); 
                
            }
            else
            {
                currentJumpIndex = 0;
                if(printOutJumpCombo)Debug.Log("failed");
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
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center),
                        FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center),
                    FootstepAudioVolume);
            }
        }

        void FirstGroundTouch()
        {
            if(isDiving|| isBackFlipping)ResetGxRotation();
            // if(lastJumpType==1)
            timeSinceGrounded = Time.time;
            isDiving = false;
            usedCoyoteTime = false;
            canJump = true;
            startedJump = false;
            onWall = false;
            usedDoubleJump = false;
            wallJump = false;
            leftGroundByJumping = false;
        }
        


    //     private void CheckWall()
    //     {
    //         bool previousOnWall = onWall;
    //         onWall = Physics.CheckSphere(transform.position, wallRadiusCheck, wallLayerMask);
    //         if (!onWall && onWall)
    //         {
    //             
    //         }
    //     
    // }
    //     
        private void CheckWallJump()
        {
            if (!wallJump) return;
            if (Time.time - timeLeftWall > overwriteOfNormalMovementPeriod)
            {
                wallJump = false;
                Debug.LogError("wall jump over");
            }
        }

        private void Dive()
        {
            if (_input.diving && CanDive())
            {
                if (_corruptAbilities.CorruptDive())
                {
                    return;
                }
                _soundManager.Play(SoundManager.EAudioClips.dive);
                _input.diving = false;
                isDiving = true;
                divingDirection = _input.move;
                _verticalVelocity = 0;
                Vector3 rot = _mainCamera.transform.rotation.eulerAngles;
                rot.x = 0;
                Quaternion editedRot = Quaternion.Euler(rot);
                if (_input.move == Vector2.zero) divingDirection = transform.forward;
                else divingDirection = editedRot * new Vector3(_input.move.x, 0, _input.move.y);

                wallJump = false;

                //turn in that diretion
                LookInDiretion(divingDirection);
            }
            else _input.diving = false;

            if (!isDiving) return;
            _controller.Move(divingSpeed * Time.deltaTime * divingDirection
                             + new Vector3(0,_verticalVelocity,0) * Time.deltaTime);
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += diveGravity * Time.deltaTime;
            }
        }

        private bool CanDive()
        {
            bool backFlipping = !(_corruptAbilities.BackFlip() == 2 && isBackFlipping);
            if (backFlipping)_soundManager.Play(SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt);
            return !Grounded && !isDiving && backFlipping;
        }

        private void LookInDiretion(Vector3 lookDirection)
        {
            if (isBackFlipping) return;
            _targetRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            
            
            gx.transform.Rotate(Vector3.right,90);
        }

        private void BackFlip()
        {
            if (isBackFlipping)
            {
                if (Time.time - startBackFlip >= backFlipDuration)
                {
                    isBackFlipping = false;
                    ResetGxRotation();
                }
                //Rotate
                Vector3 direction = new Vector3(-1, 0,0 );
                direction = transform.rotation * direction;
                gx.transform.Rotate(direction,backFlipRotateSpeed * Time.deltaTime);
                return;
            }
            if (!_input.backFlip  || !Grounded || isDiving)
            {
                if(_input.backFlip)currentJumpIndex = 0;
                _input.backFlip = false;
                return;
            }
            float newTime = _corruptAbilities.BackFlip();
            backFlipDuration = newTime;
            isBackFlipping = true;
            _input.backFlip = false;
            startBackFlip = Time.time;
            Grounded = false;
            Jump(jumpType:4,overwriteJumpCurve:backFlipHeight,applyCorrupt:false,useJumpHeightMulitplier:false);
        }

        private bool CanPlayerJump()
        {
            return !isDiving || !isBackFlipping;
        }

        private void ResetGxRotation()
        {
            gx.transform.rotation = gxStartRotation;
        }

        public void Teleport(Vector3 position)
        {
            
            _controller.SimpleMove(Vector3.zero);
            _controller.enabled = false;
            transform.position = position;
            Debug.Log(position);
            _controller.enabled = true;
        }
    }
    
}