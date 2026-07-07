using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class IceMagic : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip iceSound;

    [Header("��������� ���������")]
    public float distance = 5f;
    
    [Header("������ �� ������")]
    public Transform playerTransform;
    public Camera playerCamera;


    [Header("��������� ������� ����")]
    public int iceBlocksCount = 5; // ������� ������ ���� ����� � �����
    public float stepDistance = 1f; // ���������� ����� ������� (����� ������� ������ ����)

    [Header("���������")]
    public TextMeshProUGUI hintText;
    [Header("��������� ���� �� ������")]
    public float eyeHeight = 0.5f; 
    [Range(0f, 85f)]
    public float dipAngle = 35f;
    public ParticleSystem iceParticle;
    public float boxWidth = 1f;
    public float boxHeight = 2f;


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
            CastIce();
        }
    }

    private void CastIce()
    {
        if (playerTransform == null) return;

        bool hitWater = CastRayDown(distance, out RaycastHit hit);
        bool hitFire = CastBox(distance, out RaycastHit hit_two);

        
        if (!hitWater && !hitFire)
            return;

        
        if (iceParticle != null)
        {
            iceParticle.transform.forward = playerTransform.forward;
            iceParticle.Play();
        }

        if (AudioSource != null && iceSound != null)
            AudioSource.PlayOneShot(iceSound);

        
        if (hitWater && hit.collider.TryGetComponent(out Water water))
        {
            Vector3 forwardXZ = playerTransform.forward;
            forwardXZ.y = 0;
            forwardXZ.Normalize();

            Vector3 startPoint = hit.point;

            for (int i = 0; i < iceBlocksCount; i++)
            {
                Vector3 currentPoint = startPoint + (forwardXZ * (i * stepDistance));
                water.FreezeAtPoint(currentPoint);
            }
        }

       
       
        if (hitFire && hit_two.collider.transform != playerTransform && hit_two.collider.TryGetComponent(out IIceInteractable target))
        {
            target.OnFreeze();
        }
    }



    private bool CastRayDown(float customDistance, out RaycastHit hit)
    {
      
        Vector3 forwardXZ = playerTransform.forward;
        forwardXZ.y = 0;
        forwardXZ.Normalize();

       
        Vector3 rayStartPoint = playerTransform.position + Vector3.up * eyeHeight;

        
        Vector3 rayDirection = Quaternion.AngleAxis(dipAngle, playerTransform.right) * forwardXZ;

        
        return Physics.Raycast(rayStartPoint, rayDirection, out hit, customDistance);
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

    private void UpdateUI()
    {
        if (hintText == null || playerTransform == null) 
            return; 
        if (CastRayDown(distance, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Water"))
            {
                hintText.text = "R|���: ���������� ���� "; 
                hintText.gameObject.SetActive(true); 
                return; 
            }
        }
        if (CastBox(distance, out RaycastHit hit_two))
        {
            if (hit_two.collider.CompareTag("Fire")) 
            {
                hintText.text = "R|���: �������� �����"; 
                hintText.gameObject.SetActive(true); 
                return; 
            }
            if (hit_two.collider.CompareTag("Enemy"))
            {
                hintText.text = "R|���: ���������� �����";
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
        Vector3 rayDirection = Quaternion.AngleAxis(dipAngle, playerTransform.right) * forwardXZ;
        

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rayStartPoint, rayDirection * distance);


        // ���������� ����� ������ ����
        
        Vector3 rayDirection_two = playerTransform.forward;
        rayDirection_two.y = 0;
        rayDirection_two.Normalize();

        // ����� ������� ��������� �� �������� ��������� ���� ������� ������
        Vector3 boxCenter = rayStartPoint + rayDirection_two * (distance / 2f);

        // ������������� ������� Gizmos, ����� ������� ��������� ����� �� ����������
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerTransform.rotation, Vector3.one);

        // ������ �������� ������� ������� (�������: ������, ������, ����� ����)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f); // �������������� ������
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

        Gizmos.color = Color.yellow; // ������ �������
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxWidth, boxHeight, distance));

    }
}
