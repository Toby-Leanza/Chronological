using UnityEngine;
using Chronos;

public class TimeObject : MonoBehaviour
{
    private Timeline timeline;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        timeline = GetComponent<Timeline>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (TimeControls.Instance.isFrozen)
            return;

        var clock = Timekeeper.instance.Clock("World");

        if (TimeControls.Instance.isRewinding)
        {
            if (Vector3.Distance(transform.position, initialPosition) > 0.01f)
            {
                // Usar AddForce en lugar de Translate
                GetComponent<Rigidbody>().AddForce(Vector3.forward * clock.deltaTime * 10f);
            }
            else
            {
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
        }
        else
        {
            // Movimiento con física
            GetComponent<Rigidbody>().AddForce(Vector3.forward * clock.deltaTime * 10f);
        }
    }
}
