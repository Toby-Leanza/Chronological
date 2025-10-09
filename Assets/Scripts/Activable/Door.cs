public class Door : Activable
{
    public override void SetActiveState(bool state)
    {
        gameObject.SetActive(!state);
    }
}
