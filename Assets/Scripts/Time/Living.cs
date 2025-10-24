using System.Collections.Generic;
using UnityEngine;

public abstract class Living : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpForce = 10f;

    [Header("Components")]
    protected Rigidbody rb;
    protected Collider col;

    [Header("State")]
    protected bool isGrounded = false;
    protected bool IsFrozen => TimeControls.Instance.isFrozen;

    [Header("Recordings")]
    protected static List<KeyFrameData> globalFrames = new List<KeyFrameData>();
    protected List<PosFrameData> localFrames = new List<PosFrameData>();
    public static KeyRecorder keyRecorder = new KeyRecorder();
    protected PosRecorder posRecorder = new PosRecorder();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }
    public virtual void UnfrozenMovement(KeyFrameData keyFrame)
    {
        Vector2 moveInput = keyFrame.GetMovementVector();
        bool jumpInput = keyFrame.jump;

        // MOVIMIENTO HORIZONTAL
        if (moveInput.magnitude > 0.01f)
        {
            Quaternion baseRotation = keyFrame.movementRotation;

            Vector3 forward = baseRotation * Vector3.forward;
            Vector3 right = baseRotation * Vector3.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();

            Vector3 direction = forward * moveInput.y + right * moveInput.x;
            Vector3 targetVelocity = direction * speed;

            // ✅ CORRECTO: Modificar rb.linearVelocity directamente
            rb.linearVelocity = new Vector3(
                targetVelocity.x,
                rb.linearVelocity.y,  // Mantener velocidad Y (gravedad)
                targetVelocity.z
            );
        }
        else
        {
            // ✅ FRICCIÓN directa en rb.linearVelocity
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(
                currentVel.x * 0.7f,
                currentVel.y,        // Mantener velocidad Y
                currentVel.z * 0.7f
            );
        }

        // SALTO
        if (jumpInput && isGrounded)
        {
            // ✅ Salto directo en rb.linearVelocity
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(
                currentVel.x,
                jumpForce,           // Aplicar salto en Y
                currentVel.z
            );
            isGrounded = false;
        }
    }
    protected virtual void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    protected virtual void FixedUpdate()
    {
        CheckGrounded();
    }
}