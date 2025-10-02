using UnityEngine;
using Chronos;

public class TimeControls : MonoBehaviour
{
    public Clock globalClock;

    public float rewindSpeed = -1f;
    public float forwardSpeed = 2f;
    public float normalSpeed = 1f;

    private bool isFrozen = false;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        // Freeze toggle
        controls.Player.Freeze.performed += ctx =>
        {
            isFrozen = !isFrozen;
            globalClock.localTimeScale = isFrozen ? 0f : normalSpeed;
            Debug.Log(isFrozen ? "¡Congelado!" : "¡Descongelado!");
        };

        // Rewind
        controls.Player.Rewind.performed += ctx =>
        {
            if (isFrozen) globalClock.localTimeScale = rewindSpeed;
        };
        controls.Player.Rewind.canceled += ctx =>
        {
            if (isFrozen) globalClock.localTimeScale = 0f;
        };

        // Forward
        controls.Player.Forward.performed += ctx =>
        {
            if (isFrozen) globalClock.localTimeScale = forwardSpeed;
        };
        controls.Player.Forward.canceled += ctx =>
        {
            if (isFrozen) globalClock.localTimeScale = 0f;
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
