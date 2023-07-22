using Character.Player;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;
    public PlayerManager playerManager;
    public Camera cameraObject;

    [Header("Camera Settings")] public float cameraSmoothSpeed = 1;
    [SerializeField] private float upAndDownRotationSpeed = 220;
    [SerializeField] private float leftAndRightRotationSpeed = 220;
    [SerializeField] private float minimumPivot = -30;
    [SerializeField] private float maximumPivot = 60;

    [Header("Camera Values")] public Vector3 cameraVelocity;
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    [SerializeField] private Transform cameraPivotTransform;

    [Header("Camera Collision")] [SerializeField]
    private float cameraCollisionRadius = 0.2f;

    [SerializeField] private LayerMask collisionLayers;

    private float cameraZPosition;
    private float targetCameraZPosition;
    private Vector3 cameraObjectPosition;

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
        // DontDestroyOnLoad(this);

        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (playerManager)
        {
            FollowTarget();
            HandleRotations();
            HandleCollision();
        }
    }

    private void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position,
            ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);

        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        // if locked on, force Rotation towards target 
        // else rotate usally 

        // normal rotations
        // rotate left and right based on horizontal movement on the right joystick
        leftAndRightLookAngle += (PlayerInputManager.Instance.cameraInput.x * leftAndRightRotationSpeed) *
                                 Time.deltaTime;
        // rotate up and down based on the vertical movement on the right joystick 
        upAndDownLookAngle -= (PlayerInputManager.Instance.cameraInput.y * upAndDownRotationSpeed) *
                              Time.deltaTime;
        // clamp up-down between min and max 
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollision()
    {
        targetCameraZPosition = cameraZPosition;

        RaycastHit hit;
        // direction for the collision Check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // Check if there is an Object infront of the Camera 
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetCameraZPosition), collisionLayers))
        {
            // if there is, we get our distance from it 
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            // then equate our target z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // if target position is less then collision radius --> substract the radius (snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}