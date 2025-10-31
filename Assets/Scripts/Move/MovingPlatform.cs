using UnityEngine;
using Chronos;

public class MovingPlatformChronos : MonoBehaviour
{
    [Header("PUNTOS DE MOVIMIENTO")]
    public Transform pointA;
    public Transform pointB;

    [Header("CONFIGURACIÓN")]
    public float speed = 3f;

    private float distance;   
    private float timeTracker = 0f; 

    private Clock clock;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip plataformaClip;

    [Header("Trigger para sonido")]
    [SerializeField] private float sonidoDistance = 5f;

    private Transform player;
    private bool sonidoActivo = false;

    void Start()
    {
        if (TimeControls.Instance != null)
            clock = TimeControls.Instance.globalClock;
        else
            Debug.LogWarning("No se encontró TimeControls.Instance. Se usará tiempo normal.");

        InitializePlatform();

        player = Camera.main.transform;

        if (audioSource != null && plataformaClip != null)
        {
            audioSource.clip = plataformaClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    void InitializePlatform()
    {
        if (pointA == null || pointB == null)
        {
            SearchForPointsInParent();
        }

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Faltan puntos A o B en " + gameObject.name);
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

        timeTracker += delta * speed;

        float t = Mathf.PingPong(timeTracker / distance, 1f);

        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        if (player != null && audioSource != null)
        {
            float distancia = Vector3.Distance(transform.position, player.position);

            if (distancia <= sonidoDistance && !sonidoActivo)
            {
                audioSource.Play();
                sonidoActivo = true;
            }
            else if (distancia > sonidoDistance && sonidoActivo)
            {
                audioSource.Stop();
                sonidoActivo = false;
            }
        }
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