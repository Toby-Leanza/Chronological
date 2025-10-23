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
        var clock = Timekeeper.instance.Clock("World");

        if (TimeControls.Instance.isFrozen)
            return;

        // Si está rebobinando
        if (TimeControls.Instance.isRewinding)
        {
            // Chequea si ya llegó a su posición inicial
            if (Vector3.Distance(transform.position, initialPosition) > 0.01f)
            {
                transform.Translate(Vector3.forward * clock.deltaTime);
            }
            else
            {
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
        }
        else
        {
            // Movimiento normal hacia adelante
            transform.Translate(Vector3.forward * clock.deltaTime);
        }
    }
}
