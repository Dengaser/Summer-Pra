using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public float sensitivity = 2f;
    public float maxAngle = 80f;

    [Header("Third Person Settings")]
    public Transform target;          // Ссылка на персонажа, вокруг которого крутимся
    public float distance = 5f;       // Дистанция отхода от персонажа
    public float targetHeight = 1.5f; // Смещение камеры по высоте (чтобы смотреть не в ноги)

    private float rotationX = 0f;
    private float rotationY = 0f;
    void Start()
    {
        if (transform.parent != null)
        {
            if (target == null) target = transform.parent;
            transform.SetParent(null);
        }
    }

    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * sensitivity;
        rotationX -= mouseY * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        Vector3 targetPosition = target.position + Vector3.up * targetHeight;
        transform.position = targetPosition - (rotation * Vector3.forward * distance);
        transform.rotation = rotation;

    }
}
