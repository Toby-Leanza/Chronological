using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int targetID;
    public bool inverted = false;
    private bool state;
    private List<Activable> targets = new List<Activable>();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip botonClip;

    void Awake()
    {
        state = inverted;
        foreach (var target in Object.FindObjectsByType<Activable>(FindObjectsSortMode.None))
        {
            if (target.activableID == targetID)
                targets.Add(target);
        }
    }

    private void Update()
    {
        foreach (var target in targets)
            target.SetActiveState(state);
    }

    void OnTriggerEnter(Collider other)
    {
        state = inverted ? false : true;
        ReproducirBoton();
    }

    void OnTriggerExit(Collider other)
    {
        state = inverted ? true : false;
        ReproducirBoton();
    }

    private void ReproducirBoton()
    {
        if (audioSource != null && botonClip != null)
        {
            audioSource.PlayOneShot(botonClip);
        }
    }
}
