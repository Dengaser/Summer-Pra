using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [Header("Collision Settings")]
    public LayerMask collisionLayers; // Слой стен и окружения, сквозь которые нельзя проходить
    public float collisionRadius = 0.2f; // Радиус сферы коллизии камеры
    public float minDistance = 1f;

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
            if (target == null) target = transform.parent;
            transform.SetParent(null);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void LateUpdate()
    {
        if(target == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * sensitivity;
        rotationX -= mouseY * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        Vector3 targetPosition = target.position + Vector3.up * targetHeight;

        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        // 2. Направление от персонажа к идеальной позиции камеры
        Vector3 rayDirection = desiredPosition - targetPosition;
        float rayLength = rayDirection.magnitude;
        rayDirection.Normalize();

        // 3. Пускаем сферу от игрока к камере, чтобы проверить препятствия
        if (Physics.SphereCast(targetPosition, collisionRadius, rayDirection, out RaycastHit hit, rayLength, collisionLayers))
        {
            // Если на пути стена, плавно пододвигаем камеру в точку столкновения (но не ближе minDistance)
            float currentDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            transform.position = targetPosition + rayDirection * currentDistance;
        }
        else
        {
            // Если препятствий нет, ставим камеру в идеальную позицию
            transform.position = desiredPosition;
        }

        // Поворот остается прежним
        transform.rotation = rotation;

    }
}
