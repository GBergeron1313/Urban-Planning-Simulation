using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float smoothMoveTime = 0.125f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Rotation Settings")]
    public float sensitivity = 2f;
    public float minPitch = -90f;
    public float maxPitch = 90f;
    public bool invertY = false;
    public float rotationSmoothness = 0.1f;

    [Header("Boundaries")]
    public float boundarySize = 100f;
    public bool useBoundaries = true;
    public bool showBoundaryGizmos = true;

    // Private variables for internal state
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Vector3 smoothVelocity;
    private float currentYaw;
    private float currentPitch;
    private float targetYaw;
    private float targetPitch;
    private bool isRotating = false;
    private Vector3 lastMousePosition;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetPosition = transform.position;
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
        targetYaw = currentYaw;
        targetPitch = currentPitch;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        HandleInput();
        UpdateMovement();
        UpdateRotation();
    }

    private void HandleInput()
    {
        // Movement input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Calculate target velocity based on input
        Vector3 targetVelocity = input.normalized;

        // We want to prevent the camera from moving 
        // diagonally, because:
        // 1. It conflicts with dedicated vertical 
        //    movement, through Space and LeftShift,
        // 
        // 2. Often, one will want to repeat an action,
        //    perhaps while placing a line of road tiles,
        //    and if they don't want to move vertically
        //    while doing so.
        //    This is one thing I've found frustrating
        //    since day 1. -Reid
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        targetVelocity = (targetVelocity.x * transform.right + targetVelocity.z * forward) * moveSpeed;

        // Apply acceleration/deceleration
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            input.magnitude > 0
            ? (acceleration * Time.unscaledDeltaTime)
            : (deceleration * Time.unscaledDeltaTime)
        );

        // Update target position
        targetPosition += currentVelocity * Time.unscaledDeltaTime;

        // Manual adjustment to targetPosition for 
        // vertical movement.
        //
        // FIXME: Add Axes for Vertical movement,
        //        such that Input.GetAxisRaw(.., VerticalKey, ..)
        //        let's us do this in a more elegant way.
        if (Input.GetKey(KeyCode.Space))
        {
            targetPosition[1] += moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetPosition[1] -= moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.DrawRay(transform.position, transform.forward * 10, Color.black, 10f);
        }

        // Handle rotation toggle
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = !isRotating;
            /*Cursor.visible = !isRotating;*/
            Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.Confined;
        }

        // Handle rotation input
        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * (invertY ? 1 : -1);

            targetYaw += mouseX;
            targetPitch = Mathf.Clamp(targetPitch + mouseY, minPitch, maxPitch);
        }
        else
        {
            Rect res = Screen.safeArea;
            lastMousePosition.x = res.width / 2;
            lastMousePosition.y = res.height / 2;
        }

        /*// Reset camera*/
        /*if (Input.GetKeyDown(KeyCode.P))*/
        /*{*/
        /*    ResetCamera();*/
        /*}*/

    }

    private void UpdateMovement()
    {
        if (useBoundaries)
        {
            targetPosition = ClampPositionToBoundaries(targetPosition);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref smoothVelocity,
            smoothMoveTime
        );
    }

    private void UpdateRotation()
    {
        float dt_rotationSmoothness = (rotationSmoothness * Time.unscaledDeltaTime) * 100f;

        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, dt_rotationSmoothness);
        currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, dt_rotationSmoothness);
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
    }

    private Vector3 ClampPositionToBoundaries(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, -boundarySize, boundarySize),
            position.y,
            Mathf.Clamp(position.z, -boundarySize, boundarySize)
        );
    }

    /*private void ResetCamera()*/
    /*{*/
    /*    targetPosition = new Vector3(0, minHeight, -10);*/
    /*    targetYaw = 0;*/
    /*    targetPitch = 0;*/
    /*    currentVelocity = Vector3.zero;*/
    /*    smoothVelocity = Vector3.zero;*/
    /*    isRotating = false;*/
    /*    Cursor.visible = true;*/
    /*    Cursor.lockState = CursorLockMode.Confined;*/
    /*}*/

    private void OnDrawGizmos()
    {
        if (!showBoundaryGizmos || !useBoundaries) return;

        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(0, transform.position.y, 0);
        Vector3 size = new Vector3(boundarySize * 2, 0.1f, boundarySize * 2);
        Gizmos.DrawWireCube(center, size);
    }

    // Public methods for external control
    public void SetRotationEnabled(bool enabled)
    {
        isRotating = enabled;
        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.Confined;
    }

    public void SetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void SetRotation(float yaw, float pitch)
    {
        targetYaw = yaw;
        targetPitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}
