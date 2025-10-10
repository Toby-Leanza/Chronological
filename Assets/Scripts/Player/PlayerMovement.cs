using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpForce = 10f;
    private float gravityMultiplier = 2.5f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;
    public Camera playerCamera;

    [Header("Camera Advanced")]
    public float deadZone = 0.001f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private float xRotation = 0f;

    public TimeControls timeControls;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraHolder == null)
            cameraHolder = transform.Find("CameraHolder");
        if (playerCamera == null && cameraHolder != null)
            playerCamera = cameraHolder.GetComponentInChildren<Camera>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
            jumpPressed = true;
    }

    private void Update()
    {
        if (playerCamera != null && cameraHolder != null)
            HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) < deadZone) mouseX = 0f;
        if (Mathf.Abs(mouseY) < deadZone) mouseY = 0f;

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (timeControls != null && timeControls.isFrozen) return;

        if (playerCamera != null)
        {
            Vector3 forward = playerCamera.transform.forward;
            Vector3 right = playerCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * moveInput.y + right * moveInput.x;
            Vector3 desiredVelocity = direction * speed;

            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            rb.linearVelocity += velocityChange;
        }
    }

    private void HandleJump()
    {
        if (timeControls != null && timeControls.isFrozen) return;

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
            isGrounded = false;
        }

        if (!isGrounded)
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            isGrounded = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            isGrounded = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
