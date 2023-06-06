using System;
using HoaxGames;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterManager : MonoBehaviour, IDamageable
{
    // What does a Character have? 
    // Animator
    // Character Controller? 
    // Gravity
    // isGrounded 
    // movementSpeed

    // What can a Character do? 

    // Take Damage
    // Attack 
    //
    // Move
    // Rotate
    // GroundCheck
    // Falling

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
    protected int AnimIDSpeed;
    private int animIDGrounded;
    protected int AnimIDJump;
    protected int AnimIDFreeFall;
    protected int AnimIDMotionSpeed;
    protected int AnimIDIsUsingRootMotion;

    public bool isUsingRootMotion;
    protected CharacterController CharacterController;

    protected Animator Animator;
    protected FootIK FootIK;

    // TODO: investigate usage of this: 
    // protected virtual void Update()
    // {
    //     // Code that should be executed for all derived classes in their Update method.
    //     // For example, you can call a custom method called CustomUpdate.
    //     CustomUpdate();
    // }
    public virtual void TakeDamage(float amount)
    {
        Animator.CrossFade("Hit_Front", .2f);
        Debug.Log("Base Take Damage called: Damage Amount: " + amount);
    }

    protected void GroundedCheck()
    {
        if (FootIK && FootIK.enabled)
        {
            GroundedResult groundedResult = FootIK.getGroundedResult();
            if (groundedResult != null)
                Grounded = groundedResult.isGrounded;
        }
        else
        {
            // set sphere position, with offset
            var position = transform.position;
            Vector3 spherePosition = new Vector3(position.x, position.y - GroundedOffset,
                position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }
        // update animator if using character

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
                    transform.TransformPoint(CharacterController.center),
                    FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(CharacterController.center),
                FootstepAudioVolume);
        }
    }

    protected void AssignAnimationIDs()
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
}