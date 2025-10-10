using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;

    public float destroyDistance = 5f;

    private int frameIndex;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;
    private Rigidbody rb;
    private Collider col;
    private bool isGrounded;
    private float gravityMultiplier = 2.5f;
    private float moveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.freezeRotation = true;

        if (playerMovement != null)
            moveSpeed = playerMovement.speed;
    }

    void Update()
    {
        if (playerRecorder == null) return;

        bool frozen = TimeControls.Instance.isFrozen;
        bool rewinding = TimeControls.Instance.isRewinding;
        bool forwarding = TimeControls.Instance.isForwarding;

        if (frozen)
        {
            // Seguir posición y rotación del jugador directamente
            rb.linearVelocity = Vector3.zero;
            rb.position = playerTransform.position;
            rb.rotation = playerTransform.rotation;

            // No colisiona con nada
            col.isTrigger = true;

            // Rewind / Forward afectan inputs internos del clon (para cuando se descongele)
            if (rewinding)
            {
                frameIndex = Mathf.Max(0, frameIndex - 1);
                ApplyInputs(frameIndex, invertInput: true);
            }
            else if (forwarding)
            {
                frameIndex = Mathf.Min(playerRecorder.recordedFrames.Count - 1, frameIndex + 1);
                ApplyInputs(frameIndex);
            }
        }
        else
        {
            col.isTrigger = false;

            if (frameIndex < playerRecorder.recordedFrames.Count)
            {
                ApplyInputs(frameIndex);
                frameIndex++;
            }
        }

        // Destruir si toca al jugador
        if (!frozen && playerTransform != null)
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
        if (TimeControls.Instance != null && TimeControls.Instance.isFrozen) return;

        HandleMovement();
        HandleJump();
        HandleGravity();
    }

    private void ApplyInputs(int index, bool invertInput = false)
    {
        if (index < 0 || index >= playerRecorder.recordedFrames.Count) return;
        PlayerFrameData frame = playerRecorder.recordedFrames[index];

        currentMoveInput = invertInput ? -frame.moveInput : frame.moveInput;
        currentJumpPressed = frame.jumpPressed && !invertInput;

        // Rotación del clon igual a la grabada
        rb.rotation = frame.rotation;
    }

    private void HandleMovement()
    {
        if (currentMoveInput.magnitude < 0.01f) return;

        Vector3 direction = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y).normalized;
        Vector3 worldDirection = transform.TransformDirection(direction);

        Vector3 desiredVelocity = worldDirection * moveSpeed;
        Vector3 velocityChange = new Vector3(
            desiredVelocity.x - rb.linearVelocity.x,
            0f,
            desiredVelocity.z - rb.linearVelocity.z
        );

        rb.linearVelocity += velocityChange;
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
        currentJumpPressed = false;
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
