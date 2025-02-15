using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float smoothMoveTime = 0.125f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minHeight = 2f;
    public float maxHeight = 20f;
    public float zoomSmoothness = 0.2f;
    public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotation Settings")]
    public float rotationSpeed = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
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
    private float currentZoomVelocity;
    private float targetZoom;
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
        targetZoom = transform.position.y;
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
        UpdateZoom();
    }

    private void HandleInput()
    {
        // Movement input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Calculate target velocity based on input
        Vector3 targetVelocity = input.normalized * moveSpeed;

        // Apply acceleration/deceleration
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            input.magnitude > 0 ? acceleration : deceleration * Time.deltaTime
        );

        // Update target position
        targetPosition += transform.TransformDirection(currentVelocity) * Time.deltaTime;

        // Handle rotation toggle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRotating = !isRotating;
            Cursor.visible = !isRotating;
            Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.Confined;
        }

        // Handle zoom input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0)
        {
            targetZoom = Mathf.Clamp(targetZoom - scrollInput * zoomSpeed, minHeight, maxHeight);
        }

        // Handle rotation input
        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * (invertY ? 1 : -1);

            targetYaw += mouseX;
            targetPitch = Mathf.Clamp(targetPitch + mouseY, minPitch, maxPitch);
        }

        // Reset camera
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetCamera();
        }
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
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotationSmoothness);
        currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, rotationSmoothness);
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
    }

    private void UpdateZoom()
    {
        float currentHeight = transform.position.y;
        float newHeight = Mathf.SmoothDamp(
            currentHeight,
            targetZoom,
            ref currentZoomVelocity,
            zoomSmoothness
        );

        transform.position = new Vector3(
            transform.position.x,
            newHeight,
            transform.position.z
        );
    }

    private Vector3 ClampPositionToBoundaries(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, -boundarySize, boundarySize),
            position.y,
            Mathf.Clamp(position.z, -boundarySize, boundarySize)
        );
    }

    private void ResetCamera()
    {
        targetPosition = new Vector3(0, minHeight, -10);
        targetZoom = minHeight;
        targetYaw = 0;
        targetPitch = 0;
        currentVelocity = Vector3.zero;
        smoothVelocity = Vector3.zero;
        currentZoomVelocity = 0;
        isRotating = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

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

    public void SetZoom(float height)
    {
        targetZoom = Mathf.Clamp(height, minHeight, maxHeight);
    }
}
