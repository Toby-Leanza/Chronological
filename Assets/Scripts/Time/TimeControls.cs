using System;
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

    public static event Action OnFreeze;
    public static event Action OnUnfreeze;
    public static event Action OnStartRewind;
    public static event Action OnStopRewind;
    public static event Action OnStartForward;
    public static event Action OnStopForward;

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
            if (isFrozen) OnFreeze?.Invoke();
            else OnUnfreeze?.Invoke();
            globalClock.localTimeScale = isFrozen ? 0f : normalSpeed;
            Debug.Log(isFrozen ? "Congelado" : "Descongelado");

            var player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                var rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.linearVelocity = Vector3.zero;
            }

            if (isFrozen && cloneManager != null)
                cloneManager.SpawnClone();
        };

        controls.Player.Rewind.performed += ctx =>
        {
            OnStartRewind?.Invoke();
            if (isFrozen)
            {
                isRewinding = true;
                globalClock.localTimeScale = rewindSpeed;
            }
        };
        controls.Player.Rewind.canceled += ctx =>
        {
            OnStopRewind?.Invoke();
            if (isFrozen)
            {
                isRewinding = false;
                globalClock.localTimeScale = 0f;
            }
        };

        controls.Player.Forward.performed += ctx =>
        {
            OnStartForward?.Invoke();
            if (isFrozen)
            {
                isForwarding = true;
                globalClock.localTimeScale = forwardSpeed;
            }
        };
        controls.Player.Forward.canceled += ctx =>
        {
            OnStopForward?.Invoke();
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
