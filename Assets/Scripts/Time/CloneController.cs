using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{

    [HideInInspector] public PlayerRecorder playerRecorder;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public Transform playerTransform;

    private Rigidbody rb;
    private Collider col;
    private bool isAlive = true;
    private bool hasInitialized = false;

    private int frameIndex;
    private List<PlayerFrameData> localFrames = new List<PlayerFrameData>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.isKinematic = true; // ← IMPORTANTE: Kinematic al inicio
        }

        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            localFrames = new List<PlayerFrameData>(playerRecorder.recordedFrames);
            frameIndex = localFrames.Count - 1;

            // APLICAR POSICIÓN INICIAL INMEDIATAMENTE
            if (frameIndex >= 0 && frameIndex < localFrames.Count)
            {
                transform.position = localFrames[frameIndex].position;
                transform.rotation = localFrames[frameIndex].rotation;
            }

            Debug.Log($"Clone inicializado en frame {frameIndex} - Pos: {transform.position}");
        }

        TimeControls.OnUnfreeze += OnUnfreeze;
        TimeControls.OnFreeze += OnFreeze;

        hasInitialized = true;
    }

    void Update()
    {
        if (!isAlive || !hasInitialized) return;

        bool frozen = TimeControls.Instance.isFrozen;
        bool rewinding = TimeControls.Instance.isRewinding;
        bool forwarding = TimeControls.Instance.isForwarding;

        if (frozen)
        {
            // MANTENER kinematic durante tiempo congelado
            if (rb != null && !rb.isKinematic)
                rb.isKinematic = true;

            if (col != null && !col.isTrigger)
                col.isTrigger = true;

            if (rewinding)
                frameIndex = Mathf.Max(0, frameIndex - 1);
            else if (forwarding)
                frameIndex = Mathf.Min(localFrames.Count - 1, frameIndex + 1);

            ApplyFrame(localFrames, frameIndex);
        }
        else
        {
            // SOLO activar física cuando el tiempo no está congelado
            if (rb != null && rb.isKinematic)
                rb.isKinematic = false;

            if (col != null && col.isTrigger)
                col.isTrigger = false;

            // ... resto del código para tiempo normal
            if (playerRecorder != null && frameIndex < playerRecorder.recordedFrames.Count)
            {
                var inputFrame = playerRecorder.recordedFrames[frameIndex];
                HandleMovement(inputFrame.moveInput, inputFrame.rotation);
                HandleJump(inputFrame.jumpPressed);
            }

            HandleGravity();

            localFrames.Add(new PlayerFrameData(transform.position, transform.rotation, Vector2.zero, false));
            frameIndex++;
        }
    }
    private void OnUnfreeze()
    {
        for (int i = localFrames.Count - 1; i > frameIndex; i--)
        {
            localFrames.RemoveAt(i);
        }

        Debug.Log($"Borrados frames futuros. Frames restantes: {localFrames.Count}");

        rb.isKinematic = false;
        col.isTrigger = false;
    }

    private void OnFreeze()
    {
        rb.isKinematic = true;
        col.isTrigger = true;
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
            TimeControls.OnUnfreeze -= OnUnfreeze;
            TimeControls.OnFreeze -= OnFreeze;

            Destroy(gameObject);
        }
    }
}
