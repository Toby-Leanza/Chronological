using UnityEngine;

public class Door : Activable
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip abrirClip;
    [SerializeField] private AudioClip cerrarClip;
    private bool lastState;

    public GameObject doorOpen;    // Referencia al modelo abierto
    public GameObject doorClosed;  // Referencia al modelo cerrado

    private void Start()
    {
        // Estado inicial
        doorOpen.SetActive(false);
        doorClosed.SetActive(true);

        lastState = false;
    }

    public override void SetActiveState(bool state)
    {
        if (state == lastState) return;

        doorOpen.SetActive(state);
        doorClosed.SetActive(!state);

        if (audioSource != null)
        {
            audioSource.PlayOneShot(state ? abrirClip : cerrarClip);
        }

        lastState = state;
    }
}