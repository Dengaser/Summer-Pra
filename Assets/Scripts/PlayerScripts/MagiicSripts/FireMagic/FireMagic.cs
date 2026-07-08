using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FireMagic : MonoBehaviour
{

    public AudioSource AudioSource;
    public AudioClip fireSound;

    [Header("Настройки дистанции")]
    public float distance = 5f;

    [Header("Ссылка на игрока")]
    public Transform playerTransform;
    public Camera playerCamera;


   

    [Header("Интерфейс")]
    public TextMeshProUGUI hintText;
    [Header("Настройки луча от игрока")]
    public float eyeHeight = 0.5f;
    public float boxWidth = 1f;
    public float boxHeight = 2f;
    public ParticleSystem fireParticle;

    private Vector3 BoxHalfExtents => new Vector3(boxWidth / 2f, boxHeight / 2f, 0.1f);

    void Start()
    {
        
    }

    
    void Update()
    {

        if (playerCamera == null) playerCamera = Camera.main;

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        UpdateUI();

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            if (fireParticle != null)
            {
                fireParticle.transform.forward = playerTransform.forward;
                fireParticle.Play();
            }

            if (AudioSource != null && fireSound != null)
                AudioSource.PlayOneShot(fireSound);
            CastFire();
        }
    }



    private bool CastBox(float customDistance, out RaycastHit hit)
    {
        Vector3 rayDirection = playerTransform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();
        float spawnOffset = 0.6f;
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight + rayDirection * spawnOffset;
        float castDistance = Mathf.Max(0.1f, customDistance - spawnOffset);

        return Physics.BoxCast(rayStartPoint, BoxHalfExtents, rayDirection, out hit, playerTransform.rotation, castDistance);

    }

    private void   CastFire()
    {
        if (playerTransform == null) return;

        
        bool hitFire = CastBox(distance, out RaycastHit hit);


        if (!hitFire)
            return;


        

        if (hitFire && hit.collider.transform != playerTransform && hit.collider.TryGetComponent(out IFireInteractable target))
        {
            target.OnFire();
        }

    }

    private void UpdateUI()
    {
        if (hintText == null || playerTransform == null)
            return;
      
        if (CastBox(distance, out RaycastHit hit))
        {
            //if (hit.collider.CompareTag("Fire"))
            //{
            //    hintText.text = "R|ЛКМ: Поджечь";
            //    hintText.gameObject.SetActive(true);
            //    return;
            //}
            if (hit.collider.CompareTag("Enemy"))
            {
                hintText.text = "R|ЛКМ: Поджечь зомби";
                hintText.gameObject.SetActive(true);
                return;
            }
            if(hit.collider.CompareTag("Burn"))
            {
                hintText.text = "R|ЛКМ: Поджечь";
                hintText.gameObject.SetActive(true);
                return;
            }
        }

        hintText.gameObject.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        Vector3 forwardXZ = playerTransform.forward;
        forwardXZ.y = 0;
        forwardXZ.Normalize();

        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;
        Vector3 rayDirection_two = playerTransform.forward;
        rayDirection_two.y = 0;
        rayDirection_two.Normalize();

        // Центр коробки находится на половине дистанции луча впереди игрока
        Vector3 boxCenter = rayStartPoint + rayDirection_two * (distance / 2f);

        // Устанавливаем матрицу Gizmos, чтобы коробка крутилась вслед за персонажем
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerTransform.rotation, Vector3.one);

        // Рисуем объемную коробку захвата (размеры: ширина, высота, длина луча)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f); // Полупрозрачный желтый
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

        Gizmos.color = Color.yellow; // Контур коробки
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

    }
}
