using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("PUNTOS DE MOVIMIENTO")]
    public Transform pointA;
    public Transform pointB;
    
    [Header("CONFIGURACIÓN")]
    public float speed = 3f;
    public float waitTime = 0.5f;
    
    private Vector3 currentTarget;
    private bool isMoving = true;
    private bool movingToB = true;

    void Start()
    {
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
            Debug.LogError("FALTAN PUNTOS A o B en: " + gameObject.name + ". Buscando en hermanos...");
            SearchForPointsInSiblings();
        }

        if (pointA != null && pointB != null)
        {
            transform.position = pointA.position;
            currentTarget = pointB.position;
        }
        else
        {
            Debug.LogError("NO SE ENCONTRARON PUNTOS para: " + gameObject.name);
        }
    }

    void SearchForPointsInParent()
    {
        if (transform.parent != null)
        {
            Transform parent = transform.parent;
            foreach (Transform child in parent)
            {
                if (child.name == "Point_A" && pointA == null)
                    pointA = child;
                else if (child.name == "Point_B" && pointB == null)
                    pointB = child;
            }
        }
    }

    void SearchForPointsInSiblings()
    {
        if (transform.parent != null)
        {
            Transform parent = transform.parent;
            foreach (Transform child in parent)
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
        if (!isMoving || pointA == null || pointB == null) return;
        MovePlatform();
    }

    void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            currentTarget, 
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            StartCoroutine(ChangeDirection());
        }
    }

    IEnumerator ChangeDirection()
    {
        isMoving = false;
        yield return new WaitForSeconds(waitTime);
        movingToB = !movingToB;
        currentTarget = movingToB ? pointB.position : pointA.position;
        isMoving = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}