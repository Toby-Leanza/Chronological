using UnityEngine;

public class Door : Activable
{
    public GameObject doorOpen;    // Referencia al modelo abierto
    public GameObject doorClosed;  // Referencia al modelo cerrado

    private void Start()
    {
        // Estado inicial
        doorOpen.SetActive(false);
        doorClosed.SetActive(true);
    }

    public override void SetActiveState(bool state)
    {
        doorOpen.SetActive(state);
        doorClosed.SetActive(!state);
    }
}