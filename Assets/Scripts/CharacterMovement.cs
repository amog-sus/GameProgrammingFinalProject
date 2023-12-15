using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float mouseSensitivity = 5.0f;
    private float jumpHeight = 2f;
    private float gravityValue = -9.81f;
    private CharacterController controller;
    private float walkSpeed = 5;
    private float runSpeed = 8;

    public float rotationSpeed = 10f; 

    public Transform cameraTransform; // Assign your camera transform here in the inspector

    private float verticalSensitivity = 2.0f;
    private float cameraPitch = 0f;

    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        UpdateRotation();
        ProcessMovement();
    }

    void UpdateRotation()
    {
        // Yaw rotation (turning left and right)
        float yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * yaw);

        // Pitch rotation (looking up and down)
        cameraPitch -= Input.GetAxis("Mouse Y") * verticalSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f); // Clamp to prevent flipping

        // Apply the pitch rotation to the camera transform on the X axis
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraTransform.localEulerAngles.y, 0f);
        }
    }

    void ProcessMovement()
     {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 targetDirection = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
        targetDirection.y = 0;
        targetDirection.Normalize();

        float speed = GetMovementSpeed();
        Vector3 movement = targetDirection * speed * Time.deltaTime;
        controller.Move(movement);

        bool isMovementDetected = moveDirection.magnitude > 0;
        animator.SetBool("isWalking", isMovementDetected && !Input.GetButton("Fire3"));
        animator.SetBool("isRunning", isMovementDetected && Input.GetButton("Fire3"));
        animator.SetBool("isIdle", !isMovementDetected);

        // Jump logic
        if (groundedPlayer && Input.GetButtonDown("Jump"))
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(Vector3.up * playerVelocity.y * Time.deltaTime); 
    }

    float GetMovementSpeed()
    {
        return Input.GetButton("Fire3") ? runSpeed : walkSpeed;
    }
}
