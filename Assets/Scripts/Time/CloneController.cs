using System.Collections.Generic;
using UnityEngine;

public class CloneController : Living
{
    [Header("Clone References")]
    public PlayerMovement playerMovement;

    [Header("Clone State")]
    private int frameIndex = 0;

    protected bool IsRewinding => TimeControls.Instance != null && TimeControls.Instance.isFrozen;
    protected bool IsForwarding => TimeControls.Instance != null && TimeControls.Instance.isFrozen;

    protected override void Start()
    {
        base.Start();
        posRecorder = GetComponent<PosRecorder>();

        if (posRecorder != null)
        {
            posRecorder.record = true;
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

        frameIndex = localFrames.Count - 1;
        localFrames = playerMovement.localFrames;
        FrozenMovement();
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
            FrozenMovement();
        }
        else
        {
            UnfrozenMovement(globalFrames[frameIndex]);
            frameIndex++;
        }

        ApplyFrame();
    }

    private void FrozenMovement()
    {
        transform.position = localFrames[frameIndex].position;
        transform.rotation = localFrames[frameIndex].rotation;


        if (IsRewinding)
        {
            frameIndex = Mathf.Max(0, frameIndex - 1);
        }
        else if (IsForwarding)
        {
            frameIndex = Mathf.Min(globalFrames.Count - 1, frameIndex + 1);
        }

        localFrames.Add(new PosFrameData(transform.position, transform.rotation));
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

        // Ramificar línea temporal
        if (localFrames.Count > frameIndex + 1)
        {
            localFrames.RemoveRange(frameIndex + 1, localFrames.Count - 1);
            Debug.Log($"✅ Línea temporal ramificada. Frames mantenidos: {localFrames.Count}");
        }
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