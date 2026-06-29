using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Telekinesis : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip pickup;
    public AudioClip drop;

    [Header("Настройки дистанции")]
    public float distance = 5f;
   

    [Header("Ссылка на игрока")]
    public Transform playerTransform; 

    [Header("Интерфейс")]
    public Image cursorImage;
    public TextMeshProUGUI hintText;
    public Color baseColor = Color.white;
    public Color highlightColor = Color.green; 

    [Header("Точки удержания")]
    public Transform itemHoldParent;
    public Camera playerCamera;

    [Header("Настройки телекинеза")]
    public float throwForce = 15f;

    [Header("Настройки луча от игрока")]
    public float eyeHeight = 0.5f;
    private GameObject _heldObject;

    void Update()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        
        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (playerTransform != null)
        {
            Debug.DrawRay(playerTransform.position + Vector3.up * eyeHeight, playerTransform.forward * distance, Color.red);
        }

        HandleInput();
        UpdateUI();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_heldObject == null) TryPickUp();
            else DropObject();
        }
    }

    private void TryPickUp()
    {
        if (playerTransform == null) return;

        RaycastHit hit;
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 rayDirection = playerTransform.forward;

 
        if (Physics.Raycast(rayStartPoint, rayDirection, out hit, distance))
        {
            if (hit.collider.CompareTag("Moveable"))
            {
                PickUpObject(hit.collider.gameObject);
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        _heldObject = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        obj.transform.SetParent(itemHoldParent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (AudioSource != null && pickup != null) AudioSource.PlayOneShot(pickup);
    }

    private void DropObject()
    {
        if (_heldObject == null) return;

        Rigidbody rb = _heldObject.GetComponent<Rigidbody>();

        _heldObject.transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

           
            Vector3 throwDirection = playerTransform != null ? playerTransform.forward : playerCamera.transform.forward;

           
            throwDirection.y += 0.1f;
            throwDirection.Normalize();

            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        _heldObject = null;
        if (AudioSource != null && drop != null) AudioSource.PlayOneShot(drop);
    }

    private void UpdateUI()
    {
        if (cursorImage == null || hintText == null || playerTransform == null) return;

        if (_heldObject != null)
        {
            SetUI(highlightColor, "E: Толкнуть объект");
            return;
        }

        RaycastHit hit;
     
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 rayDirection = playerTransform.forward;

        if (Physics.Raycast(rayStartPoint, rayDirection, out hit, distance))
        {
            if (hit.collider.CompareTag("Moveable"))
            {
                SetUI(highlightColor, "E: Захватить телекинезом");
                return;
            }
        }

        cursorImage.color = baseColor;
        hintText.gameObject.SetActive(false);
    }

    private void SetUI(Color c, string t)
    {
        cursorImage.color = c;
        hintText.text = t;
        hintText.gameObject.SetActive(true);
    }
}