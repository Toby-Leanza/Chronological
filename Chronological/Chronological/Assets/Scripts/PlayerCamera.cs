using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody; // referencia al jugador
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Oculta y bloquea el mouse en pantalla
    }

    void Update()
    {
        // Movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación en eje vertical (mirar arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitar para no dar vueltas completas
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación en eje horizontal (gira el cuerpo del jugador)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
