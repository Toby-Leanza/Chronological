using UnityEngine;
using UnityEngine.UI;

public class NotaInteractiva : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.E;
    public float activationDistance = 3f;
    public GameObject notaUI;

    private Transform player;
    private PlayerMovement playerMovement;
    private bool playerInRange = false;
    private bool notaAbierta = false;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip abrirClip;

    void Start()
    {
        player = Camera.main.transform;

        playerMovement = player.GetComponentInParent<PlayerMovement>();

        if (notaUI != null)
            notaUI.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool estabaEnRango = playerInRange;
        playerInRange = distance <= activationDistance;

        if (!playerInRange && estabaEnRango && notaAbierta)
            CerrarNota();

        if (playerInRange && Input.GetKeyDown(activationKey))
        {
            if (!notaAbierta)
                AbrirNota();
            else
                CerrarNota();
        }

        if (notaAbierta && Input.GetKeyDown(KeyCode.Escape))
            CerrarNota();
    }

    void AbrirNota()
    {
        if (notaUI != null)
        {
            notaUI.SetActive(true);
            notaAbierta = true;
    
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
    
            if (playerMovement != null)
                playerMovement.enableCameraLook = false;
    
            if (audioSource != null && abrirClip != null)
                audioSource.PlayOneShot(abrirClip);
        }
    }
    
    void CerrarNota()
    {
        if (notaUI != null)
        {
            notaUI.SetActive(false);
            notaAbierta = false;
    
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    
            if (playerMovement != null)
                playerMovement.enableCameraLook = true;
        }
    }

    void OnGUI()
    {
        if (playerInRange && !notaAbierta)
        {
            GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 + 50, 200, 30),
                "Presiona E para leer la nota");
        }
    }
}
