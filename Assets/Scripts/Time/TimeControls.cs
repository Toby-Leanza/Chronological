using Chronos;
using UnityEngine;

public class TimeControls : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip freezeSound;
    [SerializeField] private AudioClip unfreezeSound;
    [SerializeField] private AudioSource rewindSource;
    [SerializeField] private AudioSource forwardSource;
    [SerializeField] private AudioClip rewindClip;
    [SerializeField] private AudioClip forwardClip;



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
        if (rewindSource != null && rewindClip != null)
        rewindSource.clip = rewindClip;
        if (forwardSource != null && forwardClip != null)
            forwardSource.clip = forwardClip;

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

            if (audioSource != null)
            {
                audioSource.PlayOneShot(isFrozen ? freezeSound : unfreezeSound);
            }


            if (isFrozen && cloneManager != null)
            {
                cloneManager.SpawnClone();
            }
        };

        controls.Player.Rewind.performed += ctx =>
        {
            if (isFrozen)
            {
                isRewinding = true;
                globalClock.localTimeScale = rewindSpeed;

                if (rewindSource != null && !rewindSource.isPlaying)
                    rewindSource.Play();
            }
        };
        controls.Player.Rewind.canceled += ctx =>
        {
            if (isFrozen)
            {
                isRewinding = false;
                globalClock.localTimeScale = 0f;

                if (rewindSource != null && rewindSource.isPlaying)
                    rewindSource.Stop();
            }
        };

        controls.Player.Forward.performed += ctx =>
        {
            if (isFrozen)
            {
                isForwarding = true;
                globalClock.localTimeScale = forwardSpeed;

                if (forwardSource != null && !forwardSource.isPlaying)
                    forwardSource.Play();
            }
        };
        controls.Player.Forward.canceled += ctx =>
        {
            if (isFrozen)
            {
                isForwarding = false;
                globalClock.localTimeScale = 0f;

                if (forwardSource != null && forwardSource.isPlaying)
                    forwardSource.Stop();
            }
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
