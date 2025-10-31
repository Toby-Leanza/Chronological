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
    protected bool IsFrozen => TimeControls.Instance != null && TimeControls.Instance.isFrozen;

    [Header("Recordings")]
    public List<PosFrameData> localFrames = new List<PosFrameData>(); // ✅ Cada instancia tiene su propia lista
    protected PosRecorder posRecorder;
    protected static KeyRecorder keyRecorder; // ✅ KeyFrames compartidos entre todos

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        posRecorder = GetComponent<PosRecorder>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Agregar frame inicial
        localFrames.Add(new PosFrameData(transform.position, transform.rotation));
    }

    public virtual void UnfrozenMovement(KeyFrameData keyFrame)
    {
        Vector2 moveInput = keyFrame.GetMovementVector();
        bool jumpInput = keyFrame.jump;

        // MOVIMIENTO con dirección relativa a la rotación de la cámara
        Vector3 direction = keyFrame.movementRotation * new Vector3(moveInput.x, 0, moveInput.y);
        direction.y = 0;
        direction.Normalize();

        Vector3 targetVelocity = direction * speed;

        // Aplicar movimiento manteniendo velocidad Y
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // SALTO
        if (jumpInput && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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

        // Grabar posición actual en localFrames (si no está congelado)
        if (!IsFrozen && posRecorder != null && posRecorder.record)
        {
            // Solo grabar si la posición cambió significativamente
            if (localFrames.Count == 0 ||
                Vector3.Distance(localFrames[localFrames.Count - 1].position, transform.position) > 0.01f)
            {
                localFrames.Add(new PosFrameData(transform.position, transform.rotation));
            }
        }
    }

}