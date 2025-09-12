using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeController : MonoBehaviour
{
    public PlayerMovement player;
    public float cloneSpeedMultiplier = 1.0f; // Variable pública para ajustar en Inspector
    private bool isTimeStopped = false;
    private List<Vector3> playerPositions = new List<Vector3>();
    private List<Quaternion> playerRotations = new List<Quaternion>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTime();
        }

        if (isTimeStopped)
        {
            RecordPlayerMovement();
        }
    }

    void ToggleTime()
    {
        isTimeStopped = !isTimeStopped;

        if (isTimeStopped)
        {
            playerPositions.Clear();
            playerRotations.Clear();
            Time.timeScale = 0.05f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Opcional: bloquear y ocultar cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            CreatePlayerClone();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    void RecordPlayerMovement()
    {
        playerPositions.Add(player.transform.position);
        playerRotations.Add(player.transform.rotation);
    }

    void CreatePlayerClone()
    {
        if (playerPositions.Count == 0) return;

        GameObject clone = Instantiate(player.gameObject, playerPositions[0], playerRotations[0]);
        SetupClone(clone);
        StartCoroutine(PlayCloneMovement(clone));
    }

    void SetupClone(GameObject clone)
    {
        // Remover AudioListener si existe
        AudioListener audioListener = clone.GetComponentInChildren<AudioListener>();
        if (audioListener != null)
        {
            Destroy(audioListener);
        }

        // Remover la cámara completa si existe
        Camera cloneCamera = clone.GetComponentInChildren<Camera>();
        if (cloneCamera != null)
        {
            Destroy(cloneCamera.gameObject);
        }

        // Resto de la configuración del clon...
        Destroy(clone.GetComponent<PlayerMovement>());
        Destroy(clone.GetComponent<PlayerInput>());

        Rigidbody cloneRb = clone.GetComponent<Rigidbody>();
        if (cloneRb != null) Destroy(cloneRb);

        Collider cloneCol = clone.GetComponent<Collider>();
        if (cloneCol != null) cloneCol.enabled = false;

        Renderer renderer = clone.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.3f, 0.3f, 1f, 0.3f);
        }
    }

    IEnumerator PlayCloneMovement(GameObject clone)
    {
        float baseDelay = 0.01f;
        float actualDelay = baseDelay / cloneSpeedMultiplier;

        for (int i = 0; i < playerPositions.Count; i++)
        {
            if (clone != null)
            {
                clone.transform.position = playerPositions[i];
                clone.transform.rotation = playerRotations[i];
                yield return new WaitForSecondsRealtime(actualDelay);
            }
        }

        if (clone != null)
        {
            Destroy(clone, 0.5f);
        }
    }
}