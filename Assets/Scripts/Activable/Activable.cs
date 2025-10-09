using UnityEngine;

public class Activable : MonoBehaviour
{
    public int activableID;

    public virtual void SetActiveState(bool state) { }
}