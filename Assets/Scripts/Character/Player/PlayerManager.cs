using UnityEngine;

namespace Character.Player
{
    public class PlayerManager : CharacterManager
    {
        private ThirdPersonControls playerControls;
        private PlayerAttackManager playerAttackManager;
        public GameObject mainCamera;
        public PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();

            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            playerAttackManager = GetComponent<PlayerAttackManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }
        protected override void Update()
        {
            base.Update();
            // TODO: Put in Input Manager
            playerLocomotionManager.MovePlayer();
            playerLocomotionManager.JumpAndGravity();
            HandleAttackInput();
        }
        private void HandleAttackInput()
        {
            // TODO: Put in Input Manager
            if (PlayerInputManager.Instance.lightAttackInput)
            {
                PlayerInputManager.Instance.lightAttackInput = false;
                playerAttackManager.HandleLightAttack();
            }
            else if (PlayerInputManager.Instance.heavyAttackInput)
            {
                PlayerInputManager.Instance.heavyAttackInput = false;
                playerAttackManager.HandleHeavyAttack();
            }
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();
            PlayerCamera.Instance.HandleAllCameraActions();
        }
        protected override void OnAnimatorMove()
        {
            base.OnAnimatorMove();
        }
    }
}