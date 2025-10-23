using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRecorder : MonoBehaviour
{
    public List<PlayerFrameData> recordedFrames = new List<PlayerFrameData>();
    public bool record = true;

    private PlayerControls playerControls;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;

    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += ctx => currentMoveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => currentMoveInput = Vector2.zero;

        playerControls.Player.Jump.performed += ctx => currentJumpPressed = true;
        playerControls.Player.Jump.canceled += ctx => currentJumpPressed = false;

        playerControls.Enable();
    }

    void Update()
    {
        if (!record) return;
        if (TimeControls.Instance != null && TimeControls.Instance.isFrozen) return;

        // Grabar posición, rotación e inputs
        recordedFrames.Add(new PlayerFrameData(
            transform.position,
            transform.rotation,
            currentMoveInput,
            currentJumpPressed
        ));

        // Reiniciar salto para que solo se registre un frame
        if (currentJumpPressed) currentJumpPressed = false;
    }

    void OnDestroy()
    {
        playerControls?.Disable();
    }

    public PlayerFrameData GetLastFrame()
    {
        if (recordedFrames.Count == 0)
            return new PlayerFrameData(transform.position, transform.rotation, Vector2.zero, false);

        return recordedFrames[recordedFrames.Count - 1];
    }
}

[System.Serializable]
public class PlayerFrameData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 moveInput;
    public bool jumpPressed;

    public PlayerFrameData(Vector3 pos, Quaternion rot, Vector2 input, bool jump)
    {
        position = pos;
        rotation = rot;
        moveInput = input;
        jumpPressed = jump;
    }
}
