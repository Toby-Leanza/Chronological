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
            rb.isKinematic = true;
        }

        if (playerRecorder != null && playerRecorder.recordedFrames.Count > 0)
        {
            // COPIAR localFrames (nuevas instancias para posición)
            foreach (var frame in playerRecorder.recordedFrames)
            {
                localFrames.Add(new PlayerFrameData(
                    frame.position,
                    frame.rotation,
                    frame.moveInput,
                    frame.jumpPressed
                ));
            }

            frameIndex = playerRecorder.recordedFrames.Count - 1;

            // APLICAR POSICIÓN INICIAL desde localFrames
            if (frameIndex >= 0 && frameIndex < localFrames.Count)
            {
                transform.position = localFrames[frameIndex].position;
                transform.rotation = localFrames[frameIndex].rotation;
            }

            Debug.Log($"Clone inicializado - Global: {playerRecorder.recordedFrames.Count}, Local: {localFrames.Count}, Frame: {frameIndex}");
        }

        TimeControls.OnUnfreeze += OnUnfreeze;
        TimeControls.OnFreeze += OnFreeze;
        hasInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isAlive || !hasInitialized) return;

        bool frozen = TimeControls.Instance.isFrozen;
        bool rewinding = TimeControls.Instance.isRewinding;
        bool forwarding = TimeControls.Instance.isForwarding;

        if (frozen)
        {
            // TIEMPO CONGELADO: Usar localFrames para posición/rotación (Kinematic)
            if (rb != null && !rb.isKinematic)
                rb.isKinematic = true;

            if (col != null && !col.isTrigger)
                col.isTrigger = true;

            // Navegar por la línea temporal
            if (rewinding)
                frameIndex = Mathf.Max(0, frameIndex - 1);
            else if (forwarding)
                frameIndex = Mathf.Min(playerRecorder.recordedFrames.Count - 1, frameIndex + 1);

            // Aplicar frame desde localFrames
            ApplyFrame(localFrames, frameIndex);
        }
        else
        {
            // TIEMPO DESCONGELADO: Física activa
            if (rb != null && rb.isKinematic)
                rb.isKinematic = false;

            if (col != null && col.isTrigger)
                col.isTrigger = false;

            // Usar playerRecorder.recordedFrames para inputs del jugador original
            if (frameIndex < playerRecorder.recordedFrames.Count)
            {
                var inputFrame = playerRecorder.recordedFrames[frameIndex];
                HandleMovement(inputFrame.moveInput, inputFrame.rotation);
                HandleJump(inputFrame.jumpPressed);
            }
            else
            {
                // Si no hay más frames globales, aplicar fricción
                Vector3 currentVel = rb.linearVelocity;
                rb.linearVelocity = new Vector3(currentVel.x * 0.8f, currentVel.y, currentVel.z * 0.8f);
            }

            HandleGravity();

            // Grabar nueva posición en localFrames (ramificación temporal)
            localFrames.Add(new PlayerFrameData(
                transform.position,
                transform.rotation,
                Vector2.zero, // Los nuevos frames locales no tienen input
                false
            ));

            frameIndex++;
        }
    }

    private void OnUnfreeze()
    {
        if (!isAlive) return;

        // CORRECCIÓN: Asegurar que frameIndex esté dentro de los límites
        frameIndex = Mathf.Min(frameIndex, localFrames.Count - 1);

        // Borrar frames futuros de localFrames (ramificación temporal)
        for (int i = localFrames.Count - 1; i > frameIndex; i--)
        {
            localFrames.RemoveAt(i);
        }

        Debug.Log($"Borrados frames futuros. Frames restantes: {localFrames.Count}");

        // Resetear física para transición suave
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        if (col != null)
            col.isTrigger = false;
    }

    private void OnFreeze()
    {
        if (!isAlive) return;

        // Resetear física antes de congelar
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (col != null)
            col.isTrigger = true;

        // Asegurar que frameIndex esté sincronizado
        frameIndex = Mathf.Min(frameIndex, localFrames.Count - 1);
    }

    private void ApplyFrame(List<PlayerFrameData> frames, int index)
    {
        if (frames.Count == 0 || index < 0 || index >= frames.Count) return;

        // Teleport directo (correcto para modo kinematic)
        transform.position = frames[index].position;
        transform.rotation = frames[index].rotation;
    }

    private void HandleMovement(Vector2 moveInput, Quaternion rotation)
    {
        if (moveInput.magnitude < 0.01f)
        {
            // Aplicar fricción cuando no hay input
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(currentVel.x * 0.9f, currentVel.y, currentVel.z * 0.9f);
            return;
        }

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 worldDirection = transform.TransformDirection(direction);

        // Aplicar fuerza para movimiento físico (en FixedUpdate)
        Vector3 force = worldDirection * (playerMovement.speed * 8f);
        rb.AddForce(force, ForceMode.Force);

        // Limitar velocidad máxima
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > playerMovement.speed)
        {
            flatVel = flatVel.normalized * playerMovement.speed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }

        // Rotación
        if (moveInput.magnitude > 0.1f)
        {
            rb.MoveRotation(rotation);
        }
    }

    private void HandleJump(bool jumpPressed)
    {
        if (jumpPressed && IsGrounded())
        {
            float jumpForce = playerMovement?.jumpForce ?? 10f;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleGravity()
    {
        if (!IsGrounded())
            rb.AddForce(Physics.gravity * 1.2f, ForceMode.Acceleration);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DestroyClone();
        }
    }

    private void DestroyClone()
    {
        isAlive = false;
        TimeControls.OnUnfreeze -= OnUnfreeze;
        TimeControls.OnFreeze -= OnFreeze;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        TimeControls.OnUnfreeze -= OnUnfreeze;
        TimeControls.OnFreeze -= OnFreeze;
    }
}