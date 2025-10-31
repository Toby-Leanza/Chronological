using UnityEngine;

public class WatchPickup : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.E;
    public float activationDistance = 3f;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = Camera.main.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= activationDistance;

        if (playerInRange && Input.GetKeyDown(activationKey))
        {
            RecogerWatch();
        }
    }

    void RecogerWatch()
    {
        Debug.Log("¡Reloj recogido!");
        gameObject.SetActive(false);
    }

    void OnGUI()
    {
        if (playerInRange)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 50),
                "Presiona E para tomar el reloj");
        }
    }

}