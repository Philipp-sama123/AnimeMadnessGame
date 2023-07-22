using UnityEngine;

namespace Character.Player
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        #region Player Movement
        [Range(0.0f, 0.3f)] [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;
        private const float MoveSpeed = 5.0f; // im m/s
        private const float SprintSpeed = 8.0f; // im m/s

        private const float JumpHeight = 1.2f;

        //Time required to pass before being able to jump again. Set to 0f to instantly jump again
        private const float JumpTimeout = 0.50f;

        //Time required to pass before entering the fall state. Useful for walking down stairs
        private const float FallTimeout = 0.15f;
        #endregion

        #region Player Runtime Variables
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        #endregion
        private float verticalMovement;
        private float horizontalMovement;
        private float moveAmount;
        private PlayerManager playerManager;
        protected Animator Animator;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            Animator = GetComponent<Animator>();
        }
        private void Start()
        {
            ResetTimeOuts();
        }
        // ToDO: LocomotionManager
        public void MovePlayer()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = PlayerInputManager.Instance.sprintInput ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (PlayerInputManager.Instance.movementInput == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            var velocity = playerManager.characterController.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = PlayerInputManager.Instance.movementInput.magnitude;

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
            Vector3 inputDirection = new Vector3(PlayerInputManager.Instance.movementInput.x, 0.0f,
                PlayerInputManager.Instance.movementInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (PlayerInputManager.Instance.movementInput != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                 playerManager.mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            // move the player
            if (!playerManager.isUsingRootMotion)
                playerManager.characterController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                                       new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            Animator.SetFloat(playerManager.AnimIDSpeed, animationBlend);
            // Animator.SetFloat(playerManager.AnimIDMotionSpeed, inputMagnitude);
        }
        public void HandleDodging()
        {
            Animator.SetBool(playerManager.AnimIDIsUsingRootMotion, true);
            Animator.CrossFade(PlayerInputManager.Instance.movementInput.magnitude == 0 ? "Avoid_Back" : "Avoid_Front",
                0.2f);
        }
        public void JumpAndGravity()
        {
            if (playerManager.Grounded)
            {
                // reset the fall timeout timer
                FallTimeoutDelta = FallTimeout;

                // update animator if using character

                Animator.SetBool(playerManager.AnimIDJump, false);
                Animator.SetBool(playerManager.AnimIDFreeFall, false);


                // stop our velocity dropping infinitely when grounded
                if (VerticalVelocity < 0.0f)
                {
                    VerticalVelocity = -2f;
                }

                // Jump
                if (PlayerInputManager.Instance.jumpInput && JumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character

                    Animator.SetBool(playerManager.AnimIDJump, true);
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

                    Animator.SetBool(playerManager.AnimIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                PlayerInputManager.Instance.jumpInput = false;
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
    }
}