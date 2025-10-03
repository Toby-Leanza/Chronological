using UnityEngine;

public class CylinderTrigger : MonoBehaviour
{
    public GameObject rectangle; // El rectángulo a ocultar

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != rectangle)
        {
            rectangle.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != rectangle)
        {
            rectangle.SetActive(true);
        }
    }
}
