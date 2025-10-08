using UnityEngine;

public abstract class Activable : MonoBehaviour
{
    public string activableID;
    public abstract void SetActiveState(bool active);
}
