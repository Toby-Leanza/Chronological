using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f; // Distancia máxima para agarrar
    public Transform holdPoint;    // Punto donde se sujetan objetos
    public float throwForce = 5f;  // Fuerza para lanzar
    public Transform cameraTransform; // Referencia a la cámara

    private GameObject heldObject;
    private Rigidbody heldRb;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                Drop();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward); // Usar la cámara
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * pickupRange, Color.red, 2f);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                Debug.Log("Objeto encontrado: " + hit.collider.gameObject.name);
                heldObject = hit.collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody>();

                if (heldRb == null)
                {
                    Debug.LogError("El objeto no tiene Rigidbody");
                    heldObject = null;
                    return;
                }

                heldObject.transform.position = holdPoint.position;
                heldObject.transform.SetParent(holdPoint);
                heldRb.isKinematic = true;
            }
            else
            {
                Debug.Log("Objeto no tiene la etiqueta Pickup: " + hit.collider.tag);
            }
        }
        else
        {
            Debug.Log("No se detectó nada en el rango del raycast");
        }
    }

    void Drop()
    {
        heldObject.transform.SetParent(null);
        heldRb.isKinematic = false;

        heldRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse); // Usar cameraTransform.forward
        heldObject = null;
        heldRb = null;
    }
}