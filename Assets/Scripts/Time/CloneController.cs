using System.Collections.Generic;
using UnityEngine;

public class CloneController : Living
{
    [Header("Clone Settings")]
    public CloneManager cloneManager;
    private PlayerMovement playerMovement; // Referencia al jugador para copiar frames

    [Header("Clone State")]
    public int frameIndex = 0;
    private bool isActive = false;
    private bool wasRewinding = false;
    private bool wasForwarding = false;
    protected override void Start()
    {
        base.Start();

        if (cloneManager == null)
            cloneManager = FindAnyObjectByType<CloneManager>();

        // ✅ Buscar al jugador para copiar sus frames locales
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            // Copiar los frames locales actuales del jugador
            localFrames = playerMovement.localFrames;
        }

        // Empezar desde el frame actual del jugador
        frameIndex = playerMovement?.localFrames.Count - 1 ?? 0;
        isActive = false;

        // Configurar física inicial (tiempo congelado)
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Suscribirse a eventos
        TimeControls.OnFreeze += OnTimeFrozen;
        TimeControls.OnUnfreeze += OnTimeUnfrozen;

        Debug.Log($"🔹 Clon creado - Frames locales heredados: {localFrames.Count}, Frame inicial: {frameIndex}");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // CheckGrounded + Grabación automática de frames

        if (!isActive)
        {
            // === TIEMPO CONGELADO ===
            FrozenBehavior();
        }
        else
        {
            // === TIEMPO DESCONGELADO ===
            UnfrozenBehavior();
        }
    }

    private void FrozenBehavior()
    {
        // 1. ACTUALIZAR frameIndex según Rewind/Forward
        if (TimeControls.Instance != null)
        {
            if (TimeControls.Instance.isRewinding && !wasRewinding)
            {
                // Rewind: disminuir frameIndex
                frameIndex = Mathf.Max(0, frameIndex - 1);
                wasRewinding = true;
                Debug.Log($"⏪ Clon rewinding to frame: {frameIndex}");
            }
            else if (TimeControls.Instance.isForwarding && !wasForwarding)
            {
                // Forward: aumentar frameIndex  
                int maxFrames = localFrames.Count - 1;
                frameIndex = Mathf.Min(maxFrames, frameIndex + 1);
                wasForwarding = true;
                Debug.Log($"⏩ Clon forwarding to frame: {frameIndex}");
            }

            // Reset flags cuando se sueltan las teclas
            if (!TimeControls.Instance.isRewinding) wasRewinding = false;
            if (!TimeControls.Instance.isForwarding) wasForwarding = false;
        }

        // 2. APLICAR frame de POSICIÓN desde localFrames propios
        if (localFrames != null && frameIndex < localFrames.Count && frameIndex >= 0)
        {
            transform.position = localFrames[frameIndex].position;
            transform.rotation = localFrames[frameIndex].rotation;
        }
    }

    private void UnfrozenBehavior()
    {
        // 1. AUMENTAR frameIndex automáticamente
        int maxKeyFrames = Living.keyRecorder?.recordedKeyFrames.Count - 1 ?? 0;
        if (frameIndex < maxKeyFrames)
        {
            frameIndex++;
        }

        // 2. EJECUTAR movimiento con física (UnfrozenMovement)
        if (Living.keyRecorder != null && frameIndex < Living.keyRecorder.recordedKeyFrames.Count)
        {
            KeyFrameData currentFrame = Living.keyRecorder.recordedKeyFrames[frameIndex];
            UnfrozenMovement(currentFrame);
        }
        else
        {
            // No hay más frames - quedarse quieto
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }

        // ✅ La grabación de frames locales se hace automáticamente en el FixedUpdate del Living
    }

    private void OnTimeFrozen()
    {
        // Congelar: cambiar a modo posición + trigger
        isActive = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
        if (col != null)
        {
            col.isTrigger = true;
        }

        Debug.Log($"❄️ Clon congelado en frame: {frameIndex}, Frames locales: {localFrames.Count}");
    }

    private void OnTimeUnfrozen()
    {
        // Descongelar: cambiar a modo física
        isActive = true;

        if (rb != null)
        {
            rb.isKinematic = false;
        }
        if (col != null)
        {
            col.isTrigger = false;
        }

        Debug.Log($"🔄 Clon descongelado - Continuando desde frame: {frameIndex}");
    }

    void OnDestroy()
    {
        TimeControls.OnFreeze -= OnTimeFrozen;
        TimeControls.OnUnfreeze -= OnTimeUnfrozen;

        if (cloneManager != null)
            cloneManager.CloneDestroyed();
    }

    // Método para debug
    public void PrintCloneState()
    {
        string state = isActive ? "ACTIVO" : "CONGELADO";
        Debug.Log($"🔹 Clon [{state}] - Frame: {frameIndex}, LocalFrames: {localFrames?.Count ?? 0}");
    }
}