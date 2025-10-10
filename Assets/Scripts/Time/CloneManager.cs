using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    [Header("Clone Settings")]
    public GameObject clonePrefab;
    public PlayerRecorder playerRecorder;
    public Transform player;
    public PlayerMovement playerMovement; // Nueva referencia

    [Header("Clone Limit")]
    public int maxClones = 3;

    [Header("Debug")]
    public List<CloneController> clones = new List<CloneController>();

    void Start()
    {
        // Obtener referencia al PlayerMovement si no está asignada
        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void SpawnClone()
    {
        if (playerRecorder == null) return;

        if (clones.Count >= maxClones)
        {
            DestroyOldestClone();
        }

        PlayerFrameData lastFrame = playerRecorder.GetLastFrame();
        GameObject cloneObj = Instantiate(clonePrefab, lastFrame.position, lastFrame.rotation);
        CloneController cloneCtrl = cloneObj.GetComponent<CloneController>();

        if (cloneCtrl != null)
        {
            cloneCtrl.playerRecorder = playerRecorder;
            cloneCtrl.playerTransform = player;
            cloneCtrl.playerMovement = playerMovement; // Pasar referencia
            clones.Add(cloneCtrl);
        }
    }

    // ... resto del código sin cambios
    private void DestroyOldestClone()
    {
        if (clones.Count > 0)
        {
            clones.RemoveAll(c => c == null);
            if (clones.Count > 0)
            {
                CloneController oldestClone = clones[0];
                clones.RemoveAt(0);
                Destroy(oldestClone.gameObject);
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

    private void Update()
    {
        clones.RemoveAll(c => c == null);
    }
}