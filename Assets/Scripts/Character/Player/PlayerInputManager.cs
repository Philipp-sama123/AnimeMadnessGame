using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;

        public PlayerManager playerManager;
        private ThirdPersonControls playerControls;

        [Header("Camera Input")]
        public Vector2 cameraInput;
        [Header("Player Movement Input")]
        public Vector2 movementInput;
        public float moveAmount;
        #region Input Values
        public bool lightAttackInput;
        public bool heavyAttackInput;
        private float rollInputTimer;

        private bool dodgeAndSprintInput;
        #endregion
        [Header("Player Action Input")]
        public bool dodgeInput = false;
        public bool sprintInput = false;
        public bool jumpInput = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
            playerManager = FindObjectOfType<PlayerManager>();
            SceneManager.activeSceneChanged += OnSceneChange;
            // Instance.enabled = false;
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new ThirdPersonControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                // while you hold it --> true!
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
                // playerControls.PlayerActions.Dodge.canceled += i => dodgeInput = false;
                // when you press the button --> True
                playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

                playerControls.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
                playerControls.PlayerActions.LightAttack.canceled += i => lightAttackInput = false;

                playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;
                playerControls.PlayerActions.HeavyAttack.canceled += i => heavyAttackInput = false;
            }

            playerControls.Enable();
        }
        private void Update()
        {
            HandleAllInputs();
        }
        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        private void HandleAllInputs()
        {
            HandleDodgeInput();
        }
        private void OnApplicationFocus(bool hasFocus)
        {
            if (enabled)
            {
                if (hasFocus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }
        private void HandleDodgeInput()
        {
            // if (playerManager.isPerformingAction) return;

            if (dodgeInput)
            {
                dodgeInput = false;
                playerManager.playerLocomotionManager.HandleDodging();
            }
        }
        private static void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
            // {
            //     Instance.enabled = true;
            // }
            // else
            // {
            //     Instance.enabled = false;
            // }
        }
    }
}