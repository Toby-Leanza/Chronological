using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;

    private Rigidbody rb;
    private Collider col;

    private int frameIndex;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;
    private bool isGrounded;
    private float gravityMultiplier = 2.5f;
    private float moveSpeed;

    private bool usingPhysics = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void SetInitialState()
    {
        // Inicializar velocidad igual al jugador
        if (playerMovement != null)
            moveSpeed = playerMovement.speed;

        // Spawnear ignorando colisiones mientras congelado
        Physics.IgnoreCollision(col, playerTransform.GetComponent<Collider>(), true);

        frameIndex = playerRecorder != null ? playerRecorder.recordedFrames.Count - 1 : 0;
        ApplyFrame(frameIndex);
    }

    void Update()
    {
        if (playerRecorder == null || playerRecorder.recordedFrames.Count == 0) return;

        var timeCtrl = TimeControls.Instance;

        if (timeCtrl.isFrozen)
        {
            // No usar física
            usingPhysics = false;

            // Ignorar colisiones con todo
            foreach (var worldCol in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
            {
                Physics.IgnoreCollision(col, worldCol, true);
            }

            // Rewind / Forward inputs
            if (timeCtrl.isRewinding)
            {
                frameIndex--;
            }
            else if (timeCtrl.isForwarding)
            {
                frameIndex++;
            }
            frameIndex = Mathf.Clamp(frameIndex, 0, playerRecorder.recordedFrames.Count - 1);
            ApplyFrame(frameIndex);
        }
        else
        {
            // Usar física
            if (!usingPhysics)
            {
                usingPhysics = true;

                // Reactivar colisiones con todo excepto jugador
                foreach (var worldCol in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
                {
                    if (worldCol != playerTransform.GetComponent<Collider>())
                        Physics.IgnoreCollision(col, worldCol, false);
                }

                // Mantener ignorando al jugador
                Physics.IgnoreCollision(col, playerTransform.GetComponent<Collider>(), true);

                // Posicionar en el último frame correcto
                ApplyFrameWithInputs(frameIndex);
            }

            // Avanzar frame
            if (frameIndex < playerRecorder.recordedFrames.Count - 1)
            {
                frameIndex++;
                ApplyFrameWithInputs(frameIndex);
            }

            // Destruir al tocar al jugador
            if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (usingPhysics)
        {
            HandleMovement();
            HandleJump();
            HandleGravity();
        }
    }

    private void ApplyFrame(int index)
    {
        if (index < 0 || index >= playerRecorder.recordedFrames.Count) return;
        PlayerFrameData frame = playerRecorder.recordedFrames[index];
        transform.position = frame.position;
        transform.rotation = frame.rotation;
    }

    private void ApplyFrameWithInputs(int index)
    {
        if (index < 0 || index >= playerRecorder.recordedFrames.Count) return;
        PlayerFrameData frame = playerRecorder.recordedFrames[index];

        transform.position = frame.position;
        transform.rotation = frame.rotation;

        currentMoveInput = frame.moveInput;
        currentJumpPressed = frame.jumpPressed;
    }

    private void HandleMovement()
    {
        if (currentMoveInput.magnitude < 0.1f) return;

        Vector3 direction = new Vector3(currentMoveInput.x, 0, currentMoveInput.y).normalized;
        Vector3 worldDir = transform.TransformDirection(direction);
        Vector3 desiredVelocity = worldDir * moveSpeed;

        Vector3 velocityChange = new Vector3(
            desiredVelocity.x - rb.linearVelocity.x,
            0,
            desiredVelocity.z - rb.linearVelocity.z
        );

        rb.linearVelocity += velocityChange * Time.fixedDeltaTime;
    }

    private void HandleJump()
    {
        if (currentJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * playerMovement.jumpForce, ForceMode.Impulse);
            isGrounded = false;
            currentJumpPressed = false;
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
