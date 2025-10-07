using UnityEngine;
using Chronos;

public class TimeObject : MonoBehaviour
{
    private Timeline timeline;

    void Awake()
    {
        timeline = GetComponent<Timeline>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Timekeeper.instance.Clock("World").deltaTime);
    }
}
