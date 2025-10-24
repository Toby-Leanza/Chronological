using UnityEngine;

public class ActivableByKeyDirect : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.E;
    public GameObject targetDoor;
    public float activationDistance = 3f;

    private bool isActive = true;
    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = Camera.main.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= activationDistance;
        
        if (Input.GetKeyDown(activationKey) && playerInRange && targetDoor != null)
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isActive = !isActive;

        MeshRenderer mr = targetDoor.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = isActive;

        Collider col = targetDoor.GetComponent<Collider>();
        if (col != null) col.enabled = isActive;
    }

    void OnGUI()
    {
        if (playerInRange)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 200, 30), "Presiona E para activar");
        }
    }
}