using UnityEngine;
using Chronos;

public class MovingPlatformChronos : MonoBehaviour
{
    [Header("PUNTOS DE MOVIMIENTO")]
    public Transform pointA;
    public Transform pointB;

    [Header("CONFIGURACIÓN")]
    public float speed = 3f;

    private float distance;       // distancia total A-B
    private float timeTracker = 0f; // tiempo acumulado (puede ir hacia adelante o atrás)

    private Clock clock;

    void Start()
    {
        if (TimeControls.Instance != null)
            clock = TimeControls.Instance.globalClock;
        else
            Debug.LogWarning("⚠️ No se encontró TimeControls.Instance. Se usará tiempo normal.");

        InitializePlatform();
    }

    void InitializePlatform()
    {
        if (pointA == null || pointB == null)
        {
            SearchForPointsInParent();
        }

        if (pointA == null || pointB == null)
        {
            Debug.LogError("❌ Faltan puntos A o B en " + gameObject.name);
            return;
        }

        distance = Vector3.Distance(pointA.position, pointB.position);
        transform.position = pointA.position;
    }

    void SearchForPointsInParent()
    {
        if (transform.parent != null)
        {
            foreach (Transform child in transform.parent)
            {
                if (child.name.Contains("Point_A") && pointA == null)
                    pointA = child;
                else if (child.name.Contains("Point_B") && pointB == null)
                    pointB = child;
            }
        }
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        float delta = clock != null ? clock.deltaTime : Time.deltaTime;

        // acumulamos el tiempo (puede ser positivo o negativo)
        timeTracker += delta * speed;

        // ping-pong matemático infinito: se repite y refleja
        float t = Mathf.PingPong(timeTracker / distance, 1f);

        // interpolación
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
