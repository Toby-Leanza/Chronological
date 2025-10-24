using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Living
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;
    public Camera playerCamera;

    [Header("References")]
    public TimeControls timeControls;
    private float xRotation = 0f;

    protected override void Start()
    {
        base.Start();
        keyRecorder = GetComponent<KeyRecorder>();
        SetPlayerKeyRecorder(keyRecorder);
        if (rb != null) rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        TimeControls.OnFreeze += OnTimeFrozen;
        TimeControls.OnUnfreeze += OnTimeUnfrozen;
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
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
        base.FixedUpdate(); // CheckGrounded

        if (timeControls != null && IsFrozen)
        {
            // Tiempo congelado - máxima fricción
            Vector3 currentVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(currentVel.x * 0.1f, currentVel.y, currentVel.z * 0.1f);
            return;
        }

        // Leer inputs directamente en FixedUpdate
        bool up = Input.GetKey(KeyCode.W) || Keyboard.current?.wKey.isPressed == true;
        bool down = Input.GetKey(KeyCode.S) || Keyboard.current?.sKey.isPressed == true;
        bool left = Input.GetKey(KeyCode.A) || Keyboard.current?.aKey.isPressed == true;
        bool right = Input.GetKey(KeyCode.D) || Keyboard.current?.dKey.isPressed == true;
        bool jump = Input.GetKey(KeyCode.Space) || Keyboard.current?.spaceKey.isPressed == true;

        Quaternion cameraRotation = playerCamera.transform.rotation;

        // Crear y procesar frame
        KeyFrameData frame = new KeyFrameData(up, down, left, right, jump, false, cameraRotation);

        // Grabar
        if (keyRecorder != null && keyRecorder.record)
        {
            keyRecorder.recordedKeyFrames.Add(frame);
        }

        // Procesar movimiento
        UnfrozenMovement(frame);
    }

    // INPUT METHODS (pueden quedar vacíos o eliminarse si no se usan)
    public void OnMove(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }

    private void OnTimeFrozen()
    {
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }

    private void OnTimeUnfrozen()
    {
        // Reset adicional si es necesario
    }

    private void OnDestroy()
    {
        TimeControls.OnFreeze -= OnTimeFrozen;
        TimeControls.OnUnfreeze -= OnTimeUnfrozen;
    }
}