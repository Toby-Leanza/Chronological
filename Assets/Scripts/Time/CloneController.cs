using UnityEngine;

public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;
    public float moveSpeed;
    public float destroyDistance = 5f;

    private int frameIndex;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;
    private Rigidbody rb;
    private bool isGrounded;
    private float gravityMultiplier = 2.5f;

    // Nuevas variables para control de tiempo
    private bool isUsingPhysics = false;
    private float rewindCooldown = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (playerMovement != null)
        {
            moveSpeed = playerMovement.speed;
            gravityMultiplier = 2.5f;
        }

        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            frameIndex = playerRecorder.recordedFrames.Count - 1; // Empezar desde el último frame
            ApplyFrame(frameIndex);
        }
    }

    void Update()
    {
        if (playerRecorder == null || playerRecorder.recordedFrames.Count == 0) return;

        // Control de tiempo congelado (rewind/forward)
        if (TimeControls.Instance.isFrozen)
        {
            isUsingPhysics = false;

            // Rebobinado
            if (TimeControls.Instance.isRewinding)
            {
                frameIndex--;
                frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
                ApplyFrame(frameIndex);
            }
            // Avance rápido
            else if (TimeControls.Instance.isForwarding)
            {
                frameIndex++;
                frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
                ApplyFrame(frameIndex);
            }
        }
        else
        {
            // Tiempo normal - usar física para movimiento suave
            if (!isUsingPhysics)
            {
                isUsingPhysics = true;
                // Posicionar en el frame correcto al salir del tiempo congelado
                if (frameIndex < playerRecorder.recordedFrames.Count)
                {
                    ApplyFrameWithInputs(frameIndex);
                }
            }

            // Avanzar en la grabación durante tiempo normal
            if (frameIndex < playerRecorder.recordedFrames.Count - 1)
            {
                frameIndex++;
                ApplyFrameWithInputs(frameIndex);
            }
        }

        // Destruir si está cerca del jugador
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= destroyDistance)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        // Solo usar física cuando NO está congelado el tiempo
        if (!TimeControls.Instance.isFrozen && isUsingPhysics)
        {
            HandleMovement();
            HandleJump();
            HandleGravity();
        }
    }

    private void ApplyFrame(int index)
    {
        if (index >= 0 && index < playerRecorder.recordedFrames.Count)
        {
            PlayerFrameData frame = playerRecorder.recordedFrames[index];
            transform.position = frame.position;
            transform.rotation = frame.rotation;
        }
    }

    private void ApplyFrameWithInputs(int index)
    {
        if (index >= 0 && index < playerRecorder.recordedFrames.Count)
        {
            PlayerFrameData frame = playerRecorder.recordedFrames[index];

            // Aplicar posición y rotación
            transform.position = frame.position;
            transform.rotation = frame.rotation;

            // Capturar inputs para usar en FixedUpdate
            currentMoveInput = frame.moveInput;
            currentJumpPressed = frame.jumpPressed;
        }
    }

    private void HandleMovement()
    {
        if (currentMoveInput.magnitude > 0.1f)
        {
            Vector3 direction = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y).normalized;
            Vector3 worldDirection = transform.TransformDirection(direction);

            Vector3 desiredVelocity = worldDirection * moveSpeed;
            Vector3 currentVelocity = rb.linearVelocity;

            Vector3 velocityChange = new Vector3(
                desiredVelocity.x - currentVelocity.x,
                0f,
                desiredVelocity.z - currentVelocity.z
            );

            rb.linearVelocity += velocityChange * Time.fixedDeltaTime;
        }
    }

    private void HandleJump()
    {
        if (currentJumpPressed && isGrounded)
        {
            float jumpForce = playerMovement?.jumpForce ?? 10f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
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
}