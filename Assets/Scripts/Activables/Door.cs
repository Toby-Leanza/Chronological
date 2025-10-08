using UnityEngine;

public class Door : Activable
{
    private bool isActive = true;

    public override void SetActiveState(bool active)
    {
        if (isActive == active) return;
        isActive = active;
        gameObject.SetActive(active);
    }
}
