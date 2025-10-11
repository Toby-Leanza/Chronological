using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;

    private Rigidbody rb;
    private Collider col;

    private int frameIndex;
    private List<PlayerFrameData> localFrames = new List<PlayerFrameData>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.freezeRotation = true;
        frameIndex = playerRecorder.recordedFrames.Count - 1;

        // Copiamos las posiciones del jugador hasta ahora
        if (playerRecorder != null)
            localFrames = new List<PlayerFrameData>(playerRecorder.recordedFrames);
    }

    void Update()
    {
        bool frozen = TimeControls.Instance.isFrozen;
        bool rewinding = TimeControls.Instance.isRewinding;
        bool forwarding = TimeControls.Instance.isForwarding;

        if (frozen)
        {
            rb.isKinematic = true;
            col.isTrigger = true;

            if (rewinding)
                frameIndex = Mathf.Max(0, frameIndex - 1);
            else if (forwarding)
                frameIndex = Mathf.Min(localFrames.Count - 1, frameIndex + 1);

            ApplyFrame(localFrames, frameIndex);
        }
        else
        {
            rb.isKinematic = false;
            col.isTrigger = false;

            // Aplicar inputs del jugador actual
            if (frameIndex < playerRecorder.recordedFrames.Count)
            {
                var inputFrame = playerRecorder.recordedFrames[frameIndex];
                HandleMovement(inputFrame.moveInput, inputFrame.rotation);
                HandleJump(inputFrame.jumpPressed);
            }

            HandleGravity();

            // Guardar posición actual del clon
            localFrames.Add(new PlayerFrameData(transform.position, transform.rotation, Vector2.zero, false));

            frameIndex++;
        }
    }

    private void ApplyFrame(List<PlayerFrameData> frames, int index)
    {
        if (frames.Count == 0 || index < 0 || index >= frames.Count) return;
        transform.position = frames[index].position;
        transform.rotation = frames[index].rotation;
    }

    private void HandleMovement(Vector2 moveInput, Quaternion rotation)
    {
        if (moveInput.magnitude < 0.01f) return;
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 desiredVelocity = transform.TransformDirection(direction) * playerMovement.speed;
        Vector3 velocityChange = new Vector3(desiredVelocity.x - rb.linearVelocity.x, 0f, desiredVelocity.z - rb.linearVelocity.z);
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)) * playerMovement.speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        Quaternion targetRotation = rotation;
        rb.MoveRotation(rotation);
    }

    private void HandleJump(bool jumpPressed)
    {
        if (jumpPressed && IsGrounded())
        {
            float jumpForce = playerMovement?.jumpForce ?? 10f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleGravity()
    {
        if (!IsGrounded())
            rb.AddForce(Physics.gravity * 1.5f, ForceMode.Acceleration);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
