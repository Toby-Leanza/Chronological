using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    [Header("Clone Settings")]
    public GameObject clonePrefab;
    public KeyRecorder keyRecorder;  // Cambiado de playerRecorder
    public Transform player;
    public PlayerMovement playerMovement;

    [Header("Clone Limit")]
    public int maxClones = 3;

    [Header("Debug")]
    public List<CloneController> clones = new List<CloneController>();

    void Start()
    {
        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (keyRecorder == null && player != null)
            keyRecorder = player.GetComponent<KeyRecorder>();
    }

    public void SpawnClone()
    {
        if (keyRecorder == null || keyRecorder.recordedKeyFrames.Count == 0)
        {
            Debug.LogWarning("No hay KeyFrames grabados para crear un clon");
            return;
        }

        if (clones.Count >= maxClones)
            DestroyOldestClone();

        Vector3 spawnPos = GetSpawnPosition();
        Quaternion spawnRot = player != null ? player.rotation : Quaternion.identity;

        GameObject cloneObj = Instantiate(clonePrefab, spawnPos, spawnRot);
        CloneController cloneCtrl = cloneObj.GetComponent<CloneController>();

        if (cloneCtrl != null)
        {
            // Asignar referencias actualizadas
            Living.keyRecorder = keyRecorder;
            cloneCtrl.playerMovement = playerMovement;

            clones.Add(cloneCtrl);

            Debug.Log($"Clon creado. KeyFrames disponibles: {keyRecorder.recordedKeyFrames.Count}");
        }
        else
        {
            Debug.LogError("El prefab del clon no tiene CloneController");
            Destroy(cloneObj);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (player == null) return Vector3.zero;

        // Posición de spawn con offset para evitar colisión inmediata
        Vector3 spawnOffset = Vector3.right * 2f;
        return player.position + spawnOffset;
    }

    private void DestroyOldestClone()
    {
        if (clones.Count > 0)
        {
            clones.RemoveAll(c => c == null);
            if (clones.Count > 0)
            {
                CloneController oldest = clones[0];
                clones.RemoveAt(0);
                if (oldest != null)
                    Destroy(oldest.gameObject);
            }
        }
    }

    public void DestroyAllClones()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            if (clones[i] != null)
                Destroy(clones[i].gameObject);
        }

        clones.Clear();
        Debug.Log("Todos los clones destruidos");
    }

    public int GetActiveCloneCount()
    {
        clones.RemoveAll(c => c == null);
        return clones.Count;
    }

    public bool CanSpawnClone()
    {
        clones.RemoveAll(c => c == null);
        return clones.Count < maxClones;
    }

    // Método para obtener todos los KeyFrames grabados del jugador
    public List<KeyFrameData> GetPlayerKeyFrames()
    {
        if (keyRecorder != null)
            return new List<KeyFrameData>(keyRecorder.recordedKeyFrames);
        return new List<KeyFrameData>();
    }

    // Método para forzar grabación de nuevos KeyFrames
    public void StartNewRecording()
    {
        if (keyRecorder != null)
        {
            keyRecorder.ClearRecording();
            keyRecorder.record = true;
        }
    }

    // Método para pausar/reanudar grabación
    public void SetRecording(bool recording)
    {
        if (keyRecorder != null)
            keyRecorder.record = recording;
    }

    private void Update() => clones.RemoveAll(c => c == null);

    // Debug info
    public void PrintCloneInfo()
    {
        Debug.Log($"Clones activos: {GetActiveCloneCount()}");
        if (keyRecorder != null)
        {
            Debug.Log($"KeyFrames grabados: {keyRecorder.recordedKeyFrames.Count}");
        }
    }
}