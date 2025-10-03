using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpForce = 10f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder; // Referencia al CameraHolder
    public Camera playerCamera;    // Referencia a la Main Camera

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private float xRotation = 0f;

    public TimeControls timeControls;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Configurar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Buscar automáticamente si no está asignado
        if (cameraHolder == null)
        {
            cameraHolder = transform.Find("CameraHolder");
        }
        if (playerCamera == null && cameraHolder != null)
        {
            playerCamera = cameraHolder.GetComponentInChildren<Camera>();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            jumpPressed = true;
        }
    }

    private void Update()
    {
        if (playerCamera != null && cameraHolder != null)
        {
            HandleMouseLook();
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

        // Rotación vertical solo en el CameraHolder
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal solo en el jugador
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
            Vector3 movement = direction * speed * Time.fixedUnscaledDeltaTime;

            rb.MovePosition(rb.position + movement);
        }
    }

    private void HandleJump()
    {
        if (timeControls != null && timeControls.isFrozen) return;

        if (jumpPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    // Opcional: Permitir toggle del cursor con ESC
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Método para cambiar sensibilidad desde otros scripts
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}