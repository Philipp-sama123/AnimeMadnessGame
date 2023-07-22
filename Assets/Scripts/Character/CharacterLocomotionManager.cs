using UnityEngine;

namespace Character
{
    public class CharacterLocomotionManager: MonoBehaviour
    {
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        protected float TerminalVelocity = 53.0f;
        protected float VerticalVelocity = 0f;

        protected float FallTimeoutDelta;
        protected float JumpTimeoutDelta;

        public AudioClip landingAudioClip;
        public AudioClip[] footstepAudioClips;
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        [SerializeField] private float groundedOffset;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundedRadius;
    }
}