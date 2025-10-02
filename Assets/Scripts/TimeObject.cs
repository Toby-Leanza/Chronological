using UnityEngine;
using Chronos;

public class TimeAffectedObject : MonoBehaviour
{
    private Timeline timeline;

    void Awake()
    {
        timeline = GetComponent<Timeline>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * timeline.deltaTime);
    }
}
