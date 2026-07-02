using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    [Header("Settings")]
    public float Speed = 7f;
    public float rotationSpeed = 10f;
    public float Gravity = -19.81f;
    public float JumpHeight = 2f;

    [Header("Настройка лестницы")]
    public float climbSpeed = 4f;
    private bool isClimbing = false;
    private Vector3 ladderForward;

    private CharacterController controller;
    private Transform cameraTransfom;
    private float verticalVelocity;

    void Start()
    {

        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        if (Camera.main != null) { cameraTransfom = Camera.main.transform; }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isClimbing)
        {
            Debug.Log("СЕЙЧАС ДЕЙСТВУЕТ РЕЖИМ КАРАБКАНЬЯ!");
            HandleClimbing();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    private void HandleNormalMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput);
        Vector3 move = Vector3.zero;

        if (inputDirection.magnitude > 0.1f && cameraTransfom != null)
        {
            Vector3 camForward = cameraTransfom.forward;
            Vector3 camRight = cameraTransfom.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            move = camForward * verticalInput + camRight * horizontalInput;
            move.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            animator.SetBool("IsGrounded", controller.isGrounded);
            if (verticalVelocity < 0) verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                animator.SetTrigger("Jump");
                animator.SetBool("IsGrounded", false);

            }
        }
        else
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }

        Vector3 finalMove = move * Speed;
        finalMove.y = verticalVelocity;

        bool isRunning = move.magnitude > 0.1f;
        animator.SetBool("IsRunning", isRunning);

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleClimbing()
    {
        animator.SetBool("IsRunning", false);
        float verticalInput = Input.GetAxis("Vertical");

       
        Vector3 climbDirection = new Vector3(0f, verticalInput, 0f);
        controller.Move(climbDirection * climbSpeed * Time.deltaTime);

        
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Игрок спрыгнул с лестницы!");

          
            Vector3 pushDirection = ladderForward; 
            ToggleClimbing(false, Vector3.zero);

            
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            animator.SetTrigger("Jump");
            animator.SetBool("IsGrounded", false);


            controller.Move((pushDirection * 2f + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        
        if (verticalInput < 0 && controller.isGrounded)
        {
            ToggleClimbing(false, Vector3.zero);
        }
    }

    public void ToggleClimbing(bool enable, Vector3 ladderForwardDirection)
    {
        isClimbing = enable;

        if (isClimbing)
        {
            verticalVelocity = 0f;
            ladderForward = ladderForwardDirection;

            if (ladderForward != Vector3.zero)
            {
                // Поворачиваем игрока лицом к лестнице
                Vector3 lookDir = -ladderForward;
                lookDir.y = 0f;

                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // Мягко позиционируем игрока, отключая коллизию на 1 кадр
                controller.enabled = false;
                // Чуть-чуть сдвигаем к центру куба для стабильности
                transform.position += ladderForward * 0.1f;
                controller.enabled = true;
            }
        }
    }

    public void OnEnable()
    {
        if (controller != null)
        {
            controller.Move(Vector3.zero);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetPhysicsActive(bool active)
    {
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = active;

        this.enabled = active;
    }

    public void ToggleControl(bool active)
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        controller.enabled = active;
        this.enabled = active;
    }
}