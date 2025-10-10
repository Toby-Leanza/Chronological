using Chronos;
using UnityEngine;

public class TimeControls : MonoBehaviour
{
    public static TimeControls Instance { get; private set; }

    public Clock globalClock;
    public CloneManager cloneManager;
    public float normalSpeed = 1f;
    public float rewindSpeed = -3f;
    public float forwardSpeed = 3f;

    private PlayerControls controls;

    public bool isFrozen = false;
    public bool isRewinding { get; private set; }
    public bool isForwarding { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        controls = new PlayerControls();

        controls.Player.Freeze.performed += ctx =>
        {
            isFrozen = !isFrozen;
            globalClock.localTimeScale = isFrozen ? 0f : normalSpeed;
            Debug.Log(isFrozen ? "Congelado" : "Descongelado");

            var player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                var rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.linearVelocity = Vector3.zero; // resetea velocidad
            }

            if (isFrozen && cloneManager != null)
                cloneManager.SpawnClone();
        };

        controls.Player.Rewind.performed += ctx =>
        {
            if (isFrozen)
            {
                isRewinding = true;
                globalClock.localTimeScale = rewindSpeed;
            }
        };
        controls.Player.Rewind.canceled += ctx =>
        {
            if (isFrozen)
            {
                isRewinding = false;
                globalClock.localTimeScale = 0f;
            }
        };

        controls.Player.Forward.performed += ctx =>
        {
            if (isFrozen)
            {
                isForwarding = true;
                globalClock.localTimeScale = forwardSpeed;
            }
        };
        controls.Player.Forward.canceled += ctx =>
        {
            if (isFrozen)
            {
                isForwarding = false;
                globalClock.localTimeScale = 0f;
            }
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
