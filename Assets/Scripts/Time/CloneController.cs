using UnityEngine;

public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;
    public float moveSpeed = 8f;
    public float destroyDistance = 5f;

    private int frameIndex;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;
    private Rigidbody rb;
    private bool isGrounded;
    private float gravityMultiplier = 2.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Inicializamos en el último frame grabado si hay alguno
        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            frameIndex = playerRecorder.recordedFrames.Count - 1;
            ApplyFrame(frameIndex);
        }
    }

    void Update()
    {
        if (playerRecorder == null || playerRecorder.recordedFrames.Count == 0) return;

        // Cada frame verificamos si se debe destruir por cercanía al jugador
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (!TimeControls.Instance.isFrozen && distance <= destroyDistance)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Control de tiempo congelado
        if (TimeControls.Instance.isFrozen)
        {
            // Rewind
            if (TimeControls.Instance.isRewinding)
            {
                frameIndex--;
                frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
                ApplyFrameWithInput(frameIndex, invert: true);
            }
            // Forward
            else if (TimeControls.Instance.isForwarding)
            {
                frameIndex++;
                frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
                ApplyFrameWithInput(frameIndex, invert: false);
            }
        }
    }

    void FixedUpdate()
    {
        // Solo usar física cuando el tiempo no está congelado
        if (!TimeControls.Instance.isFrozen)
        {
            if (frameIndex < playerRecorder.recordedFrames.Count)
            {
                ApplyFrameWithInput(frameIndex, invert: false);
                HandleMovement();
                HandleJump();
                HandleGravity();
                frameIndex++;
            }
        }
    }

    private void ApplyFrame(int index)
    {
        if (index < 0 || index >= playerRecorder.recordedFrames.Count) return;

        PlayerFrameData frame = playerRecorder.recordedFrames[index];
        transform.position = frame.position;
        transform.rotation = frame.rotation;
    }

    private void ApplyFrameWithInput(int index, bool invert)
    {
        if (index < 0 || index >= playerRecorder.recordedFrames.Count) return;

        PlayerFrameData frame = playerRecorder.recordedFrames[index];
        transform.rotation = frame.rotation;

        // Capturar inputs
        currentMoveInput = invert ? -frame.moveInput : frame.moveInput;
        currentJumpPressed = frame.jumpPressed && !invert;

        // Mover el transform directamente mientras está congelado
        if (TimeControls.Instance.isFrozen)
        {
            Vector3 direction = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y).normalized;
            transform.position += transform.TransformDirection(direction) * moveSpeed * Time.fixedDeltaTime;

            if (currentJumpPressed)
            {
                transform.position += Vector3.up * (playerMovement?.jumpForce ?? 10f) * Time.fixedDeltaTime;
            }
        }
    }

    private void HandleMovement()
    {
        if (currentMoveInput.magnitude > 0.01f)
        {
            Vector3 direction = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y).normalized;
            Vector3 worldDir = transform.TransformDirection(direction);
            Vector3 desiredVel = worldDir * moveSpeed;
            Vector3 velChange = new Vector3(desiredVel.x - rb.linearVelocity.x, 0f, desiredVel.z - rb.linearVelocity.z);
            rb.linearVelocity += velChange;
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
}
