using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public string targetID;
    private Activable target;
    private bool isActive = false;

    void Start()
    {
        target = FindTarget();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsValidActivator(other) || isActive) return;

        isActive = true;
        if (target != null) target.SetActiveState(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsValidActivator(other)) return;

        // verificamos si ya no hay nada tocando el trigger
        if (!IsAnythingTouching())
        {
            isActive = false;
            if (target != null) target.SetActiveState(false);
        }
    }

    bool IsValidActivator(Collider col)
    {
        return col.CompareTag("Player") || col.CompareTag("Clone") || col.CompareTag("Box");
    }

    bool IsAnythingTouching()
    {
        // Physics.OverlapBox / Sphere según la forma del trigger
        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2);
        foreach (var hit in hits)
        {
            if (IsValidActivator(hit))
                return true;
        }
        return false;
    }

    private Activable FindTarget()
    {
        foreach (var activable in FindObjectsOfType<Activable>())
        {
            if (activable.activableID == targetID)
                return activable;
        }
        return null;
    }
}
