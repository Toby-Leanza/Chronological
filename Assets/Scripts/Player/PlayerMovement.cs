using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : Living
{
    [Header("Camera")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;
    public Camera playerCamera;

    [Header("Time Control")]
    public TimeControls timeControls;

    private float xRotation = 0f;

    protected override void Start()
    {
        base.Start();
        keyRecorder = GetComponent<KeyRecorder>();

        // Inicializar localFrames vacío
        if (localFrames == null)
        {
            localFrames = new List<PosFrameData>();
        }

        if (rb != null) rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        // ✅ Jugador empieza con localFrames vacío (solo frame inicial)
        Debug.Log($"✅ Player iniciado - Frames locales: {localFrames.Count}");

        Debug.Log("✅ PlayerMovement iniciado con grabación de inputs");
    }

    void Update()
    {
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // CheckGrounded + Grabación automática de PosFrames

        // ✅ PLAYER SOLO REPRODUCE - NO GRABA INPUTS

        if (timeControls != null && IsFrozen)
        {
            // Tiempo congelado - aplicar fricción
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(currentVel.x * 0.1f, currentVel.y, currentVel.z * 0.1f);
        }
        else
        {
            // Tiempo normal - reproducir KeyFrame grabado correspondiente
            if (Living.keyRecorder != null)
            {
                KeyFrameData frame = Living.keyRecorder.recordedKeyFrames.Last();
                UnfrozenMovement(frame);
            }
            else
            {
                // Si no hay frames grabados aún, quedarse quieto
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
}