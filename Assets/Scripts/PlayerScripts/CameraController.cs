using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Collision Settings")]
    public LayerMask collisionLayers;
    public float collisionRadius = 0.2f;
    public float minDistance = 1f;

    [Header("Camera Settings")]
    public float sensitivity = 2f;
    public float maxAngle = 80f;

    [Header("Third Person Settings")]
    public Transform target;
    public float distance = 5f;
    public float targetHeight = 1.5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (transform.parent != null)
        {
            if (target == null)
                target = transform.parent;

            transform.SetParent(null);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (PauseMenuManager.IsGamePaused)
            return;

        if (target == null)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * sensitivity;
        rotationX -= mouseY * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        Vector3 targetPosition = target.position + Vector3.up * targetHeight;
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        Vector3 rayDirection = desiredPosition - targetPosition;
        float rayLength = rayDirection.magnitude;
        rayDirection.Normalize();

        if (Physics.SphereCast(
                targetPosition,
                collisionRadius,
                rayDirection,
                out RaycastHit hit,
                rayLength,
                collisionLayers))
        {
            float currentDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            transform.position = targetPosition + rayDirection * currentDistance;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.rotation = rotation;
    }
}