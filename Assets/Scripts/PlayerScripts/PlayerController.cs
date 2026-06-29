using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float Speed = 7f;
    public float Gravity = -19.81f;
    public float JumpHeight = 2f;

    private CharacterController controller;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * verticalInput + transform.right * horizontalInput;

        
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        else
        {
            
            verticalVelocity += Gravity * Time.deltaTime;
        }

        Vector3 finalMove = move * Speed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
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

    
    public void ResetMovement()
    {
        if (controller != null)
        {
            verticalVelocity = 0; 
            controller.enabled = false;
            controller.enabled = true;
        }
    }
}
