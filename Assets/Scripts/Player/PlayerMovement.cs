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
    public Transform cameraHolder; // Referencia al CameraHolder
    public Camera playerCamera;    // Referencia a la Main Camera

    [Header("Camera Advanced")]
    public float deadZone = 0.001f; // Zona muerta para evitar drift

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

        // Buscar autom�ticamente si no est� asignado
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
        // Obtener input del mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // SOLUCI�N 1: Aplicar dead zone para evitar drift
        if (Mathf.Abs(mouseX) < deadZone) mouseX = 0f;
        if (Mathf.Abs(mouseY) < deadZone) mouseY = 0f;

        // SOLUCI�N 2: Usar Time.deltaTime en lugar de unscaledDeltaTime
        // y reducir la sensibilidad recomendada
        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        // Rotaci�n vertical solo en el CameraHolder
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotaci�n horizontal solo en el jugador
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reinicia velocidad vertical
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
            isGrounded = false;
        }

        // Aplicar gravedad extra manualmente
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
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

    // M�todo para cambiar sensibilidad desde otros scripts
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}