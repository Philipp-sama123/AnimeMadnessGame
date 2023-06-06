using HoaxGames;
using UnityEngine;
using UnityEngine.Serialization;

public class CatPlayerManager : CharacterManager
{
    #region Player Movement

    [Range(0.0f, 0.3f)] [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float speedChangeRate = 10.0f;
    private const float MoveSpeed = 2.0f; // im m/s
    private const float SprintSpeed = 5.335f; // im m/s

    private const float JumpHeight = 1.2f;

    //Time required to pass before being able to jump again. Set to 0f to instantly jump again
    private const float JumpTimeout = 0.5F;

    //Time required to pass before entering the fall state. Useful for walking down stairs
    private const float FallTimeout = 0.15f;

    #endregion

    #region Camera Variables

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField]
    private GameObject cinemachineCameraTarget;

    [SerializeField] private bool lockCameraPosition = false;

    // private cinemachine variables
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private readonly float topClamp = 70.0f;
    private readonly float bottomClamp = -30.0f;
    private readonly float cameraAngleOverride = 0.0f;

    #endregion

    #region Player Runtime Variables

    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;

    #endregion

    #region Input Values

    private bool lightAttackInput;
    private bool heavyAttackInput;
    private float rollInputTimer;

    private Vector2 moveInput;
    private Vector2 cameraInput;
    private bool jumpInput;
    private bool dodgeAndSprintInput;

    #endregion

    private ThirdPersonControls playerControls;
    private GameObject mainCamera;

    private void Awake()
    {
        // get a reference to our main camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        Animator = GetComponent<Animator>();
        FootIK = GetComponent<FootIK>();

        CharacterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new ThirdPersonControls();

            playerControls.PlayerMovement.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            // while you hold it --> true!
            playerControls.PlayerActions.Sprint.performed += i => dodgeAndSprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => dodgeAndSprintInput = false;
            // when you press the button --> True
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;

            playerControls.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
            playerControls.PlayerActions.LightAttack.canceled += i => lightAttackInput = false;

            playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;
            playerControls.PlayerActions.HeavyAttack.canceled += i => heavyAttackInput = false;
        }

        playerControls.Enable();
    }

    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        ResetTimeOuts();
        AssignAnimationIDs();
    }

    private void Update()
    {
        Move();
        JumpAndGravity();
        HandleAttackInput();
        GroundedCheck();

        isUsingRootMotion = Animator.GetBool(AnimIDIsUsingRootMotion);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void HandleAttackInput()
    {
        if (lightAttackInput)
        {
            HandleLightAttack(); 
            lightAttackInput = false;
        }
        else if (heavyAttackInput)
        {
            heavyAttackInput = false;
            HandleHeavyAttack();
        }
    }

    private void HandleHeavyAttack()
    {
        throw new System.NotImplementedException();
    }

    private void HandleLightAttack()
    {
        throw new System.NotImplementedException();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if ( /*_input.look.sqrMagnitude >= _threshold &&*/ !lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f; //: Time.deltaTime;

            cinemachineTargetYaw += cameraInput.x * deltaTimeMultiplier;
            cinemachineTargetPitch += cameraInput.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride,
            cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        bool sprintFlag = false;
        bool dodgeFlag = false;
        if (dodgeAndSprintInput)
        {
            rollInputTimer += Time.deltaTime;
            if (moveInput.magnitude > 0.5f /*&& playerStats.currentStamina > 0*/)
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                dodgeFlag = true;
            }

            rollInputTimer = 0;
        }


        if (dodgeFlag)
        {
            HandleDodging();
        }

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = sprintFlag ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (moveInput == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        var velocity = CharacterController.velocity;
        float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = moveInput.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (moveInput != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                             mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                rotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // move the player
        if (!isUsingRootMotion)
            CharacterController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                     new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        Animator.SetFloat(AnimIDSpeed, animationBlend);
        Animator.SetFloat(AnimIDMotionSpeed, inputMagnitude);
    }

    private void HandleDodging()
    {
        Animator.SetBool(AnimIDIsUsingRootMotion, true);
        Animator.CrossFade(moveInput.magnitude == 0 ? "Avoid_Back" : "Avoid_Front", 0.2f);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            FallTimeoutDelta = FallTimeout;

            // update animator if using character

            Animator.SetBool(AnimIDJump, false);
            Animator.SetBool(AnimIDFreeFall, false);


            // stop our velocity dropping infinitely when grounded
            if (VerticalVelocity < 0.0f)
            {
                VerticalVelocity = -2f;
            }

            // Jump
            if (jumpInput && JumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character

                Animator.SetBool(AnimIDJump, true);
            }

            // jump timeout
            if (JumpTimeoutDelta >= 0.0f)
            {
                JumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            JumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (FallTimeoutDelta >= 0.0f)
            {
                FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character

                Animator.SetBool(AnimIDFreeFall, true);
            }

            // if we are not grounded, do not jump
            jumpInput = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (VerticalVelocity < TerminalVelocity)
        {
            VerticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void ResetTimeOuts()
    {
        JumpTimeoutDelta = JumpTimeout;
        FallTimeoutDelta = FallTimeout;
    }

    private void OnAnimatorMove()
    {
        if (isUsingRootMotion)
        {
            CharacterController.Move(Animator.deltaPosition);
        }
    }
}