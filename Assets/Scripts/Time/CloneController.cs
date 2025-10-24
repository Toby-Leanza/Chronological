using System.Collections.Generic;
using UnityEngine;

public class CloneController : Living
{
    [Header("Clone References")]
    public PlayerMovement playerMovement;

    [Header("Clone State")]
    private int frameIndex = 0;

    protected void Start()
    {
        posRecorder = GetComponent<PosRecorder>();

        if (posRecorder != null)
        {
            posRecorder.record = true;
        }

        // Copiar KeyFrames del jugador
        if (keyRecorder != null)
        {
            globalFrames = new List<KeyFrameData>(keyRecorder.recordedKeyFrames);
        }

        // Configurar física específica del clon
        if (rb != null)
        {
            rb.isKinematic = true;
            if (col != null) col.isTrigger = true;
        }

        speed = playerMovement.speed;
        jumpForce = playerMovement.jumpForce;

        TimeControls.OnFreeze += OnFreeze;
        TimeControls.OnUnfreeze += OnUnfreeze;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // CheckGrounded del Living

        // Actualizar frames locales
        if (posRecorder != null)
        {
            localFrames = new List<PosFrameData>(posRecorder.recordedPosFrames);
        }

        if (IsFrozen)
        {
            HandleFrozenTime();
        }
        else
        {
            UnfrozenMovement(globalFrames[frameIndex]);
            frameIndex++;
        }

        ApplyFrame();
    }

    private void HandleFrozenTime()
    {
        if (TimeControls.Instance.isRewinding)
        {
            frameIndex = Mathf.Max(0, frameIndex - 1);
        }
        else if (TimeControls.Instance.isForwarding)
        {
            frameIndex = Mathf.Min(globalFrames.Count - 1, frameIndex + 1);
        }
    }

    private void ApplyFrame()
    {
        if (IsFrozen)
        {
            // Tiempo congelado: usar posiciones grabadas
            if (localFrames.Count > 0 && frameIndex < localFrames.Count)
            {
                transform.position = localFrames[frameIndex].position;
                transform.rotation = localFrames[frameIndex].rotation;
            }
        }
        // Tiempo normal: la física maneja la posición automáticamente
    }

    private void OnFreeze()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            if (col != null) col.isTrigger = true;
        }
    }

    private void OnUnfreeze()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            if (col != null) col.isTrigger = false;
        }
    }

    private void OnDestroy()
    {
        TimeControls.OnFreeze -= OnFreeze;
        TimeControls.OnUnfreeze -= OnUnfreeze;
    }
}