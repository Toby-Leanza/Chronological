using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public Transform holdPoint;
    public float throwForce = 5f;
    public Transform cameraTransform;
    public float followSpeed = 20f; // mayor velocidad

    private Rigidbody heldRb;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldRb == null)
        {
            TryPickup();
        }
        else
        {
            Drop();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * pickupRange, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange) && hit.collider.CompareTag("Pickup"))
        {
            heldRb = hit.collider.GetComponent<Rigidbody>();
            if (heldRb == null)
            {
                Debug.LogError("El objeto no tiene Rigidbody");
                return;
            }

            heldRb.useGravity = false;
            heldRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Drop()
    {
        if (heldRb == null) return;

        heldRb.useGravity = true;
        heldRb.constraints = RigidbodyConstraints.None;
        heldRb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
        heldRb = null;
    }

    void FixedUpdate()
    {
        if (TimeControls.Instance.isFrozen)
        {
            Drop();
        }
        else if (heldRb != null)
        {
            Vector3 targetPos = holdPoint.position;
            Vector3 moveDir = targetPos - heldRb.position;

            // Mover usando Rigidbody y suavizado
            heldRb.linearVelocity = moveDir / Time.fixedDeltaTime;

            // Rotación igual a la cámara
            heldRb.rotation = cameraTransform.rotation;
        }
    }

}
