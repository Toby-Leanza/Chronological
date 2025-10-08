using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int targetID;
    public bool inverted = false;
    private bool state;
    private List<Activable> targets = new List<Activable>();

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
    }

    void OnTriggerExit(Collider other)
    {
        state = inverted ? true : false;
    }
}
