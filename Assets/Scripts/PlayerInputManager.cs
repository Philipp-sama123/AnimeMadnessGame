using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    public PlayerManager playerManager;
    private ThirdPersonControls playerControls;

    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 cameraInput;

    [Header("Camera Input")]
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Player Movement Input")]
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;
    #region Input Values
    private bool lightAttackInput;
    private bool heavyAttackInput;
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

        SceneManager.activeSceneChanged += OnSceneChange;
        Instance.enabled = false;
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new ThirdPersonControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
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
        HandleMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
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
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount >= 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        // Not locked on (!) so character moves always forward
        // playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters(
        //     0,
        //     moveAmount,
        //     playerManager.playerNetworkManager.isSprinting.Value
        // );
        // if locked on 
    }
    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
    private void HandleDodgeInput()
    {
        // if (playerManager.isPerformingAction) return;

        if (dodgeInput)
        {
            // if in menu do nothing 
            // perform Dodge 
            dodgeInput = false;
            // playerManager.playerLocomotionManager.AttemptToPerformDodge();
        }
    }
    private void HandleJumpInput()
    {
        // if (playerManager.isPerformingAction) return;

        if (jumpInput)
        {
            // if in menu do nothing 
            // perform Dodge 
            jumpInput = false;
            // playerManager.playerLocomotionManager.AttemptToPerformJump();
        }
    }
    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            // playerManager.HandleSprinting();
        }
        else
        {
            // playerManager.playerNetworkManager.isSprinting.Value = false;
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