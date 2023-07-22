using HoaxGames;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character
{
    public class CharacterManager : MonoBehaviour, IDamageable
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool isJumping = false;
        public bool isSprinting = false;
        public bool isGrounded = true;

        public bool applyRootMotion;
        public bool canRotate = true;
        public bool canMove = true;

        public bool Grounded = true;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        protected float TerminalVelocity = 53.0f;
        protected float VerticalVelocity = 0f;

        protected float FallTimeoutDelta;
        protected float JumpTimeoutDelta;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [SerializeField] private float GroundedOffset;
        [SerializeField] private LayerMask GroundLayers;
        [SerializeField] private float GroundedRadius;


        // animation IDs
        public int AnimIDSpeed;
        private int animIDGrounded;
        public int AnimIDJump;
        public int AnimIDFreeFall;
        protected int AnimIDMotionSpeed;
        public int AnimIDIsUsingRootMotion;

        public bool isUsingRootMotion;

        protected Animator Animator;
        protected FootIK FootIK;
        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
            FootIK = GetComponent<FootIK>();
            characterController = GetComponent<CharacterController>();
            AssignAnimationIDs();
        }
        protected virtual void Update()
        {
            GroundedCheck();
            isUsingRootMotion = Animator.GetBool(AnimIDIsUsingRootMotion);
        }
        public virtual void TakeDamage(float amount)
        {
            Animator.CrossFade("Hit_Front", .2f);
            Debug.Log("Base Take Damage called: Damage Amount: " + amount);
        }
        protected virtual void LateUpdate()
        {
        }
        protected virtual void GroundedCheck()
        {
            if (FootIK && FootIK.enabled)
            {
                GroundedResult groundedResult = FootIK.getGroundedResult();
                if (groundedResult != null)
                    Grounded = groundedResult.isGrounded;
            }
            else
            {
                var position = transform.position;
                Vector3 spherePosition = new Vector3(position.x, position.y - GroundedOffset,
                    position.z);
                Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                    QueryTriggerInteraction.Ignore);
            }

            Animator.SetBool(animIDGrounded, Grounded);
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index],
                        transform.TransformPoint(characterController.center),
                        FootstepAudioVolume);
                }
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterController.center),
                    FootstepAudioVolume);
            }
        }
        protected virtual void AssignAnimationIDs()
        {
            AnimIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            AnimIDJump = Animator.StringToHash("Jump");
            AnimIDFreeFall = Animator.StringToHash("FreeFall");
            AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            AnimIDIsUsingRootMotion = Animator.StringToHash("IsUsingRootMotion");
        }
        protected static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        protected virtual void OnAnimatorMove()
        {
            if (isUsingRootMotion)
            {
                characterController.Move(Animator.deltaPosition);
            }
        }
    }
}