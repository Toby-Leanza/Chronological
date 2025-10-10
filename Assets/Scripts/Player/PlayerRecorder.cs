using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRecorder : MonoBehaviour
{
    public List<PlayerFrameData> recordedFrames = new List<PlayerFrameData>();
    public bool record = true;

    private PlayerMovement playerMovement;
    private PlayerControls playerControls;
    private Vector2 currentMoveInput;
    private bool currentJumpPressed;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

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

        // No grabar si el tiempo está congelado
        if (TimeControls.Instance != null && TimeControls.Instance.isFrozen) return;

        recordedFrames.Add(new PlayerFrameData(
            transform.rotation,
            currentMoveInput,
            currentJumpPressed
        ));

        if (currentJumpPressed) currentJumpPressed = false;
    }

    public PlayerFrameData GetLastFrame()
    {
        if (recordedFrames.Count == 0)
            return new PlayerFrameData(transform.rotation, Vector2.zero, false);

        return recordedFrames[recordedFrames.Count - 1];
    }
}

[System.Serializable]
public class PlayerFrameData
{
    public Quaternion rotation;
    public Vector2 moveInput;
    public bool jumpPressed;

    public PlayerFrameData(Quaternion rot, Vector2 input, bool jump)
    {
        rotation = rot;
        moveInput = input;
        jumpPressed = jump;
    }
}
